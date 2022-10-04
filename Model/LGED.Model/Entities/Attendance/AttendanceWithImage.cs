using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LGED.Model.Entities.Attendance
{
    public class AttendanceWithImage : EntityBase
    {
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public DateTime? P_Date { get; set; }
        public string? InImage { get; set; }
        public string? OutImage { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string ? ImageSrc { get; set; }
    }
}