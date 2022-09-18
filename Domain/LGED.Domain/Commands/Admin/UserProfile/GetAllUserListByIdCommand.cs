using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Domain.Models.Profile;

namespace LGED.Domain.Commands.Admin.UserProfile
{
    public class GetAllUserListByIdCommand: CommandBase<UserDetailModel>
    {
        public Guid? Id { get; set; }
    }
}