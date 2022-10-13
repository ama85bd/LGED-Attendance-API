using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Domain.Models.Attendance;

namespace LGED.Domain.Commands.Attendance
{
    public class GetUserDailyAttendanceCommand: CommandBase<List<AttendancePerDayModel>>
    {
        public Guid? UserId { get; set; }
        public string? Scheme { get; set; }
        public string? Host { get; set; }
        public string? PathBase { get; set; }
    }
}