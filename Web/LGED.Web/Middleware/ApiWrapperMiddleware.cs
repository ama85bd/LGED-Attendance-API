using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LGED.Core.Extensions;
using LGED.Core.Helper;
using LGED.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LGED.Web.Middleware
{
    public class ApiWrapperMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiWrapperMiddleware> _logger;
          private readonly ApiWrapperMiddlewareOptions _options;
        public ApiWrapperMiddleware(RequestDelegate next, ILogger<ApiWrapperMiddleware> logger,
      
            ApiWrapperMiddlewareOptions options)
        {
            _options = options;
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //exclude response wrap with ApiResponse<T> for these endpoints
            //return Json(ApiResponse<T>) at the action instead in case want to return the same Json result like wrapped endpoints
            if (IsExclude(context, _options.ExcludePaths) || IsIgnore(context))
                await _next(context);
            else
            {
                var originalResponseBodyStream = context.Response.Body;

                using var memoryStream = new MemoryStream();
                try
                {
                    context.Response.Body = memoryStream;
                    await _next.Invoke(context);

                    if (context.Response.HasStarted)
                    {
                        _logger.Log(LogLevel.Warning, "Response already started, skip response wrapper.");
                        return;
                    }

                    var bodyAsText = await FormatResponse(memoryStream);

                    context.Response.Body = originalResponseBodyStream;

                    await HandleRequestAsync(context, bodyAsText, context.Response.StatusCode);
                }
                catch (Exception ex)
                {
                    if (context.Response.HasStarted)
                    {
                        _logger.Log(LogLevel.Warning, "Response already started, skip response wrapper.");
                        return;
                    }

                    await HandleExceptionAsync(context, ex);

                    //copy back to the original stream to return error response
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await memoryStream.CopyToAsync(originalResponseBodyStream);
                }
            }
        }

        /// <summary>
        /// Exclude paths declared in configuration.
        /// </summary>
        /// <param name="context">HttpContext.</param>
        /// <param name="paths">List of the paths with be excluded.</param>
        /// <returns></returns>
        private static bool IsExclude(HttpContext context, List<string> paths)
        {
            if (paths == null || !paths.Any())
            {
                return false;
            }

            return paths.Any(c => context.Request.Path.StartsWithSegments(c));
        }

        /// <summary>
        /// Exclude endpoint if using ApiWrapperIgnore attribute.
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns></returns>
        private bool IsIgnore(HttpContext context)
        {
            var endpoint = context.GetEndpoint() as RouteEndpoint;
            if (endpoint?.Metadata?.GetMetadata<ApiWrapperIgnoreAttribute>() != null)
            {
                _logger.Log(LogLevel.Information, "Excluded wrap response for the endpoint: " + context.Request.Path);
                return true;
            }

            return false;
        }

         private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var version = _options.Version ?? "1.0";
            string errorMsg, resultMsg = "";
            int code;

            if (exception is ApiException ex)
            {
                _logger.LogError(ex, ">>>>>>>>> API ERROR");
                errorMsg = ex.Message;
                code = ex.StatusCode;
            }
            else if (exception is UnauthorizedAccessException)
            {
                _logger.LogError(exception, ">>>>>>>>> AUTH ERROR");
                errorMsg = "Unauthorized";
                code = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception is System.Data.SqlClient.SqlException ||
                     exception is Microsoft.Data.SqlClient.SqlException)
            {
                _logger.LogError(exception, ">>>>>>>>> DATABASE ERROR");
                errorMsg = "Unable to retrieve data from database";
                //bind error info for references, the content length is not more than 255
                resultMsg = exception.Message;
                if (resultMsg.Length > 255)
                {
                    resultMsg = resultMsg.Substring(0, 255);
                }
                code = (int)HttpStatusCode.InternalServerError;
            }
            else
            {
                _logger.LogError(exception, ">>>>>>>>> SERVER ERROR");
                errorMsg = exception.GetBaseException().Message;
                code = (int)HttpStatusCode.InternalServerError;
            }


            //bind response object
            var apiResponse = new ApiResponse<object>(code, "", errorMsg, 0, version) { Result = resultMsg };
            var jsonString = JsonConvert.SerializeObject(apiResponse);

            //write wrapped response to http context
            context.Response.StatusCode = code;
            context.Response.ContentType = "application/json;charset=UTF-8";
            //.net core 3.1: need to set content length after rewrite the new response body
            //to avoid wrong content length caused by WriteAsync 
            context.Response.ContentLength = Encoding.UTF8.GetByteCount(jsonString);
            await context.Response.WriteAsync(jsonString, Encoding.UTF8);
        }

        //https://github.com/proudmonkey/RESTApiResponseWrapper.Core/blob/master/VMD.RESTApiResponseWrapper.Core/APIResponseMiddleware.cs
        private async Task HandleRequestAsync(HttpContext context, object body, int code)
        {
            #region Handle Error Code

            var version = _options.Version ?? "1.0";
            string message = code switch
            {
                //200
                (int)HttpStatusCode.OK => "Success",
                //404
                (int)HttpStatusCode.NotFound => "Resource not found",
                //204
                (int)HttpStatusCode.NoContent => "Resource empty",
                //409
                (int)HttpStatusCode.Conflict => "Resource conflict",
                //403
                (int)HttpStatusCode.Forbidden => "Forbidden",
                //401
                (int)HttpStatusCode.Unauthorized => "Unauthorized",
                _ => "Invalid request",
            };

            #endregion

            #region Handle Response Body

            string jsonString = "";
            ApiResponse<object> apiResponse;

            var bodyText = !body.ToString().ValidateJson() ? JsonConvert.SerializeObject(body) : body.ToString();

            dynamic bodyContent = JsonConvert.DeserializeObject<dynamic>(bodyText!);

            Type type = bodyContent?.GetType();

            #endregion

            #region Rewrite Response with Message

            //check if the body content is json object
            if (type == typeof(JObject))
            {
                //return with ApiResponse object
                apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(bodyText);
                //check empty response
                if (!apiResponse.IsEmptyResponse())
                {
                    if (apiResponse.StatusCode != code || apiResponse.Result != null)
                    {
                        //replace with optional version
                        if (apiResponse.Version != null && apiResponse.Version != version)
                        {
                            var originalEncodedVer = $"\"version\": \"{apiResponse.Version}\"";
                            var optionalEncodedVer = $"\"version\": \"{version}\"";
                            //try to replace the last occurence of "version" attribute, which belong to the ApiResponse
                            jsonString = bodyText.ReplaceLastOccurrence(originalEncodedVer, optionalEncodedVer);
                        }
                        else
                        {
                            jsonString = bodyText;
                        }
                    }
                }
                //not ApiResponse object
                else
                {
                    //model state errors object
                    if (bodyText.Contains("errors"))
                    {
                        //check if model state invalid
                        var validationProblem = JsonConvert.DeserializeObject<ValidationProblemDetails>(bodyText);
                        if (validationProblem.Errors.Any())
                        {
                            //bind error
                            var errors = new List<ValidationError>();
                            foreach (var modelErrorEntry in validationProblem.Errors)
                            {
                                errors.AddRange(modelErrorEntry.Value.Select(c => new ValidationError
                                { Name = modelErrorEntry.Key, Description = c }));
                            }

                            // throw new ApiException(validationProblem.Title, validationProblem.Status ?? 500);
                            apiResponse = new ApiResponse<object>(validationProblem.Status ?? 500, errors,
                                validationProblem.Title, errors.Count, version);
                            jsonString = JsonConvert.SerializeObject(apiResponse);
                        }
                    }
                    //normal object
                    else
                    {
                        apiResponse = new ApiResponse<object>(code, bodyContent, message, 1, version);
                        jsonString = JsonConvert.SerializeObject(apiResponse);
                    }
                }
            }
            //return with an array
            else if (type == typeof(JArray))
            {
                //parse to array to get result count
                var arr = JArray.Parse(bodyText);

                apiResponse = new ApiResponse<object>(code, bodyContent, message, arr.Count, version);
                jsonString = JsonConvert.SerializeObject(apiResponse);
            }
            //return with type value (boolean, string or null)
            else
            {
                //validate & return raw value instead of string for boolean & number
                var validated = ValidateTypeValue(bodyContent);
                object result = validated.Item1 ? validated.Item2 : bodyContent;

                apiResponse = new ApiResponse<object>(code, result, message, 1, version);
                jsonString = JsonConvert.SerializeObject(apiResponse);
            }

            #endregion

            context.Response.ContentType = "application/json;charset=UTF-8";
            context.Response.ContentLength = Encoding.UTF8.GetByteCount(jsonString);
            await context.Response.WriteAsync(jsonString, Encoding.UTF8);
        }
        private static async Task<string> FormatResponse(Stream bodyStream)
        {
            bodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(bodyStream).ReadToEndAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);

            var (isEncoded, parsedText) = responseBody.VerifyBodyContent();

            return isEncoded ? parsedText : responseBody;
        }


          private static (bool, object) ValidateTypeValue(object value)
        {
            var result = value.ToString();
            if (result.IsWholeNumber())
            {
                return (true, result.ToInt64());
            }

            if (result.IsDecimalNumber())
            {
                return (true, result.ToDecimal());
            }

            if (result.IsBoolean())
            {
                return (true, result.ToBoolean());
            }

            return (false, value);
        }
    }
    
    

    //just blank attribute for using with IsIgnore()
    public class ApiWrapperIgnoreAttribute : Attribute
    {
        public ApiWrapperIgnoreAttribute()
        {
        }
    }

    public class ApiWrapperMiddlewareOptions
    {
        public string Version { get; set; }
        public List<string> ExcludePaths { get; set; } = new List<string>();
    }

    
    [Serializable]
    public class ValidationError
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

     public static class ApiWrapperMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiWrapperMiddleware(this IApplicationBuilder builder,
            ApiWrapperMiddlewareOptions options = default)
        {
            options ??= new ApiWrapperMiddlewareOptions();
            return builder.UseMiddleware<ApiWrapperMiddleware>(options);
        }
    }
}