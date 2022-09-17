using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Commands.Admin.UserProfile
{
    public class CreateUpdateAdminUserRegisterCommand: CommandBase<bool>
    {
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Department { get; set; }
        public string Password { get; set; }
        // [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        public string UserType { get; set; }
        [MaxLength(20)]
        public string ContactNumber { get; set; }
    }
}