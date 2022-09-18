using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Commands.Admin.UserProfile
{
    public class ActiveUserCompanyCommand : CommandBase<bool>
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }
    }
}