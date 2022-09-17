using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Data.Base;
using LGED.Model.Context;
using LGED.Model.Entities.Profile;

namespace LGED.Data.Repository.Profile
{
    public class UserRolesRepository : RepositoryBase<UserRoles>
    {
        public UserRolesRepository(LgedDbContext context) : base(context)
        {
        }
    }
}