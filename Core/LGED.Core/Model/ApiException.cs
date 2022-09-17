using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LGED.Core.Model
{
    //https://medium.com/@barbyk_98266/net-core-3-1-response-wrapper-exception-middleware-765e485236d5
    public class ApiException: Exception
    {
        private int _statusCode;

        public int StatusCode
        {
            get => _statusCode;
            //https://weblog.west-wind.com/posts/2020/Feb/24/Null-API-Responses-and-HTTP-204-Results-in-ASPNET-Core
            set => _statusCode = value == 204 ? 200 : value;
        }
        
        public ApiException(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }
        
        public ApiException(Exception ex, int statusCode = 500) : base(ex.Message)
        {
            StatusCode = statusCode;
        }
        
        public ApiException(IEnumerable<string> message, int statusCode = 500) : base(string.Join(",",
            message.Distinct()))
        {
            StatusCode = statusCode;
        }
        
        public override string ToString()
        {
            if (InnerException == null)
            {
                return base.ToString();
            }

            return string.Format(CultureInfo.InvariantCulture, "{0} [See nested exception: {1}]", base.ToString(),
                InnerException);
        }
    }
}