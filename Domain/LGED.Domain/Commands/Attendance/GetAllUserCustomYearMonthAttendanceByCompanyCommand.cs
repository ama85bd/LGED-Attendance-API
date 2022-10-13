using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Domain.Models.Attendance;

namespace LGED.Domain.Commands.Attendance
{
    public class GetAllUserCustomYearMonthAttendanceByCompanyCommand : CommandBase<List<AttendancePerDayModel>>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string? Scheme { get; set; }
        public string? Host { get; set; }
        public string? PathBase { get; set; }
        
    }
}