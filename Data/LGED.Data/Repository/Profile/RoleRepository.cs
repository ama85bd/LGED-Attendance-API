using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Data.Base;
using LGED.Model.Context;
using LGED.Model.Entities.Profile;

namespace LGED.Data.Repository.Profile
{
    public class RoleRepository : RepositoryBase<Role>
    {
        public RoleRepository(LgedDbContext context) : base(context)
        {
        }
    }
}