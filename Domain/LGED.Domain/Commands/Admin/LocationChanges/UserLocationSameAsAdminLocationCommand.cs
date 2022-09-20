using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Commands.Admin.LocationChanges
{
    public class UserLocationSameAsAdminLocationCommand: CommandBase<bool>
    {
        // public Guid ComId { get; set; }
        public Guid UserId { get; set; }
    }
}