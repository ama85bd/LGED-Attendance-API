using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LGED.Model.Entities.Profile
{
    public class UserRoles: IdentityUserRole<Guid>, IEntityBase
    {
        public Guid CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public Guid InsertedBy { get; set; }
        public DateTime InsertedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}