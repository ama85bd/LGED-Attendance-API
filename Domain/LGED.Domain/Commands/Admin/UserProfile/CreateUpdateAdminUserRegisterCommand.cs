using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LGED.Domain.Base;
using NetTopologySuite.Geometries;
using static LGED.Model.Entities.Profile.Company;

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
        public string Designation { get; set; }
        public byte[]? UserImage { get; set; }
        [MaxLength(20)]
        public string ContactNumber { get; set; }
        // [JsonIgnore]
        // public GeoLocation? Location { get; set; }

        public double Latitude {get; set;}
        public double Longitude {get; set;}
        // public int SRID {get; set;}

    }
}