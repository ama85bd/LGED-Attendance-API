using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LGED.Core.Helper
{
    public static class JsonHelper
    {
        /// <summary>
        /// Check if the text is valid JSON format.
        /// </summary>
        /// <param name="text">content.</param>
        /// <returns><see langword="true" /> if valid.</returns>
        public static bool ValidateJson(this string text)
        {
            text = text.Trim();
            if (text.StartsWith("{") && text.EndsWith("}") || //object
                text.StartsWith("[") && text.EndsWith("]")) //array
            {
                try
                {
                    var obj = JToken.Parse(text);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            return false;
        }

        public static bool IsValidJson(string json)
        {
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch (Exception ex) when
                (ex is JsonException || ex is ArgumentException)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Verify if the body content is JSON.
        /// </summary>
        /// <param name="text">content.</param>
        /// <returns></returns>
        public static (bool isEncoded, string parsedText) VerifyBodyContent(this string text)
        {
            try
            {
                var obj = JToken.Parse(text);
                return (true, obj.ToString());
            }
            catch (Exception)
            {
                return (false, text);
            }
        }
    }
}