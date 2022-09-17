using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Commands.Admin.UserProfile
{
    public class CreateUpdateRoleCommand: CommandBase<bool>
    {
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public int Code { get; set; }
    }
}