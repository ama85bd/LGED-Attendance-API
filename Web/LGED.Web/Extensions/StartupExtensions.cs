using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using API;
using LGED.Core.Constants;
using LGED.Domain.AutoMapper;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LGED.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LGED-API", Version = "v1", Description = "API for LGED attendance system" });
                    c.EnableAnnotations();

                    #region ADFS
                    //TODO: Add this for ADFS authentication
                    // var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                    //
                    // // add a swagger document for each discovered API version
                    // // note: you might choose to skip or document deprecated API versions differently
                    // foreach (var description in provider.ApiVersionDescriptions)
                    // {
                    //     options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                    // }
                    //
                    // // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                    // options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme  //ApiKeyScheme //
                    // {
                    //     Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    //     Type = SecuritySchemeType.OAuth2,
                    //     Flows = new OpenApiOAuthFlows
                    //     {
                    //         Implicit = new OpenApiOAuthFlow
                    //         {
                    //             AuthorizationUrl = new Uri($"{Config.OidcAuthority}/connect/authorize", UriKind.Absolute),
                    //             Scopes = new Dictionary<string, string>
                    //             {
                    //                 { "create", "Create access to protected resources" },
                    //                 { "read", "Read access to protected resources" },
                    //                 { "update", "Update access to protected resources" },
                    //                 { "delete", "Delete access to protected resources" },
                    //             }
                    //         }
                    //     }
                    // });
                    #endregion

                    //internal authentication
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                        In = ParameterLocation.Header,
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                    });
                    // These must be AFTER options.AddSecurityDefinition
                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                    c.OperationFilter<SwaggerDefaultValues>();
                    c.IncludeXmlComments(XmlCommentsFilePath);
                    c.EnableAnnotations();
                });
        }

         public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(AutoMapperConfigure));
        }
        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = AppContext.BaseDirectory;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";

                return Path.Combine(basePath, fileName);
            }
        }
    }

    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                return;
            }

            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach (var parameter in operation.Parameters)
            {
                var description = context.ApiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
                var routeInfo = description.RouteInfo;

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (routeInfo == null)
                {
                    continue;
                }

                parameter.Required |= !routeInfo.IsOptional;
            }

            operation.Parameters.Add(HeaderParam(AppConstants.HttpHeader.CDBM_COMPANY_ID_HEADER, false, "Current user's company id."));
        }

        private static OpenApiParameter HeaderParam(string name, bool required = true, string description = "")
        {
            return new OpenApiParameter
            {
                Name = name,
                In = ParameterLocation.Header,
                Description = description,
                Required = required,
                AllowEmptyValue = false,
                AllowReserved = false,
                Schema = new OpenApiSchema { Type = "String" }
            };
        }
    }
}