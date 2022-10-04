using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Domain.Base;
using LGED.Domain.Models.Attendance;
using Microsoft.AspNetCore.Http;

namespace LGED.Domain.Commands.Attendance
{
    public class CreateUpdateAttendanceWithImageCommand : CommandBase<AttendancePerDayModel>
    {
        public IFormFile? ImageFile { get; set; }
    }
}