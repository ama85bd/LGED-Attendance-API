using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Attendance;
using LGED.Domain.Models.Attendance;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Attendance
{
    public class GetAllUserDailyAttendanceByCompanyHandler : HandlerBase<GetAllUserDailyAttendanceByCompanyCommand, List<AttendancePerDayModel>>
    {
        public GetAllUserDailyAttendanceByCompanyHandler(IUnitOfWork unitOfWork, IUserContext context, IMapper mapper) : base(unitOfWork, context, mapper)
        {
        }

        public override async Task<List<AttendancePerDayModel>> Handle(GetAllUserDailyAttendanceByCompanyCommand command, CancellationToken cancellationToken)
        {
            

            var todayDateOnly = DateTime.Now.ToShortDateString();

            var dailyAttendance = await _unitOfWork.AttendanceWithImageRepository.GetQueryNoCached().Where(u => u.CompanyId == _context.CurrentCompanyId 
                    && u.P_Date == DateTime.Parse(todayDateOnly) )
                    .Distinct()
                    .OrderByDescending(x => x.P_Date)
                    .ToListAsync(cancellationToken);

            if (dailyAttendance == null) throw new ApiException("Company Not Found!", (int)HttpStatusCode.Gone);

            var companyName =  _unitOfWork.CompanyRepository.GetQueryNoCached().Where(r => r.Id == _context.CurrentCompanyId)
                    .FirstOrDefault()?.Name;
            string CurrentYear = DateTime.Now.Year.ToString();
            string CurrentMonth = DateTime.Now.ToString("MMMM");
            var attendancePerDay = new List<AttendancePerDayModel>();
            foreach(var dailyAttend in dailyAttendance)
            {
                 attendancePerDay.Add(new AttendancePerDayModel
            {
                UserId = dailyAttend.UserId,
                InTime = dailyAttend.InTime?.ToLongTimeString(),
                OutTime = dailyAttend.OutTime?.ToLongTimeString(),
                P_Date = dailyAttend.P_Date?.ToString("dd/MM/yyyy"),
                InImage = String.Format("{0}://{1}{2}/Images/{3}/{4}/{5}/In/{6}",command.Scheme,command.Host,command.PathBase,companyName,CurrentYear,CurrentMonth,dailyAttend.InImage),
                OutImage = String.Format("{0}://{1}{2}/Images/{3}/{4}/{5}/Out/{6}",command.Scheme,command.Host,command.PathBase,companyName,CurrentYear,CurrentMonth,dailyAttend.OutImage)
            });

            
            }

            return attendancePerDay;
        }
    }
}