using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Models.Profile
{
    public class GetApproveByMasterAdminModel: ViewModelBase
    {
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? UserName { get; set; }
        public string? UserType { get; set; }
        public string? UserEmail { get; set; }
        public string? CompanyName { get; set; }
    }
}