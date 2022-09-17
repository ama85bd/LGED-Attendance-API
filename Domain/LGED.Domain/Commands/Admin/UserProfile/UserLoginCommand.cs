using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Domain.Models.Profile;

namespace LGED.Domain.Commands.Admin.UserProfile
{
    public class UserLoginCommand: CommandBase<LoginCredentialModel>
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}