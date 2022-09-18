using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Models.Profile
{
    public class UserDetailModel : ViewModelBase
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string UserType { get; set; }
        public string CompanyName { get; set; }
        public string? Designation { get; set; }
        public byte[] UserImage { get; set; }
    }
}