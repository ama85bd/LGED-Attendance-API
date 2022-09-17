using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using LGED.Core.Interfaces;

namespace LGED.Core.Model
{
    [DataContract]
    public class ApiResponse<T> : IApiResponse<T>
    {
        [DataMember(Name = "statusCode")] public int StatusCode { get; set; }

        [DataMember(Name = "result")] public T Result { get; set; }

        [DataMember(Name = "message")] public string Message { get; set; }

        [DataMember(Name = "version")] public string Version { get; set; }

        [DataMember(Name = "count")] public int Count { get; set; }

        public bool IsEmptyResponse()
        {
            return StatusCode == 0 && Result == null && Version == null && Message == null;
        }

         //required to deserialize
        public ApiResponse()
        {
        }
        
        //default constructor
        public ApiResponse(int statusCode, [NotNull] T result, string message = "", int count = 0,
            string version = "1.0")
        {
            StatusCode = statusCode;
            Message = message;
            Result = result;
            Version = version;
            Count = count;
        }
        
        //paged response with count
        public ApiResponse([NotNull] T result, int count, string message = "", string version = "1.0")
        {
            StatusCode = 200;
            Result = result;
            Version = version;
            Message = message;
            Count = count;
        }
    }
}