using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LGED.Core.Interfaces
{
    public interface IApiResponse<T>
    {
        string Version { get; set; }
        int StatusCode { get; set; }
        string Message { get; set; }
        T Result { get; set; }
        int Count { get; set; }
        bool IsEmptyResponse();
    }
}