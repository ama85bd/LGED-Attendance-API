using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Commands.Admin.UserProfile
{
    public class UpdateUserProfileByIdCommand : CommandBase<bool>
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Designation { get; set; }
        [MaxLength(20)]
        public string ContactNumber { get; set; }
        public byte[]? UserImage { get; set; }
    }
}