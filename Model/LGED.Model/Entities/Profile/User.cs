using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;

namespace LGED.Model.Entities.Profile
{
    public class User: IdentityUser<Guid>, IEntityBase
    {
        public User()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public sealed override Guid Id { get; set; }

        public string UserId { get; set; }  // nguyenhai.nam
        public string? FirstName { get; set; } //Nguyen
        public string? LastName { get; set; } //Hai Nam
        //public override string Email { get; set; } // using email from IdentityUser
        public string DisplayName { get; set; } // Nguyen Hai Nam
        //public int LoginFailConsecutive { get; set; } // => use AccessFailedCount of IdentityUser instead
        public DateTime? LastLoggedInAt { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string UserType { get; set; } // Internal / External
        public string? StaffId { get; set; } //C0000026
        public string? Culture { get; set; } //Vietnam
        public string? Designation { get; set; } //Vietnam
        public byte[]? ProfileImage { get; set; } //Vietnam
        
        
        [JsonIgnore]
        public Point ? Location { get; set; }
        public DateTime? LastSyncFromAd { get; set; } //last date sync from active directory, each 7 days from new login
        public bool IsDeleted { get; set; }
        public Guid InsertedBy { get; set; }
        public DateTime InsertedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsReceiveIowEmails { get; set; }
        public bool IsReceiveDataAnomEmails { get; set; }
        public bool IsReceiveCreepRemaningEmails { get; set; }
    }
}