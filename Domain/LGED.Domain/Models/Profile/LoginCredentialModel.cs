using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Models.Profile
{
    public class LoginCredentialModel: ViewModelBase
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}