using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;

namespace LGED.Domain.Models.Attendance
{
    public class AttendancePerDayModel : ViewModelBase
    {
        public Guid? UserId { get; set; }
        public Guid? CompanyId { get; set; }
        public string? InTime { get; set; }
        public string? OutTime { get; set; }
        public string? P_Date { get; set; }
        public string? InImage { get; set; }
        public string? OutImage { get; set; }

        [NotMapped]
        public string ? ImageSrc { get; set; }
    }
}