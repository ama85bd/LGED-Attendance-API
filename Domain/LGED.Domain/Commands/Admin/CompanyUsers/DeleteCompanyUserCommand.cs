using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Commands.Admin.CompanyUsers
{
    public class DeleteCompanyUserCommand : CommandBase<bool>
    {
        public Guid UserId { get; set; }
    }
}