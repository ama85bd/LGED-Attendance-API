using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace LGED.Model.Entities.Profile
{
    public class Company: EntityBase
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; }
        
        [JsonIgnore]
        public Point ? Location { get; set; }
        
        public string? ShortName { get; set; }
        public string Description { get; set; }
        public string? RegistrationNumber { get; set; }
        public string ContactNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? Url { get; set; }
        public string Department { get; set; }
        public string ?State { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string ?PostCode { get; set; }
        public string? Country { get; set; }
        // Division or Company
        public string Type { get; set; }
        // Self-join
        // Division may has company Id or not
        public Guid? CompanyId { get; set; }


    }
}