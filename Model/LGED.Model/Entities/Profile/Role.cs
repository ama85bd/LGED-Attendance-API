using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LGED.Model.Entities.Profile
{
    [Serializable]
    public class Role: IdentityRole<Guid>
    {
        public Role()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public sealed override Guid Id { get; set; }
        public int Code { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public Guid InsertedBy { get; set; }
        public DateTime InsertedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}