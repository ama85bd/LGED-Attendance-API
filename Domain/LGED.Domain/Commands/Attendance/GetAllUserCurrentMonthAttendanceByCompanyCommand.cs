using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Domain.Models.Attendance;

namespace LGED.Domain.Commands.Attendance
{
    public class GetAllUserCurrentMonthAttendanceByCompanyCommand : CommandBase<List<AttendancePerDayModel>>
    {
        public string? Scheme { get; set; }
        public string? Host { get; set; }
        public string? PathBase { get; set; }
        
    }
}