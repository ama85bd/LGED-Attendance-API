using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Attendance;
using LGED.Domain.Models.Attendance;

namespace LGED.Domain.Handlers.Attendance
{
    public class CreateUpdateAttendanceWithImageHandler : HandlerBase<CreateUpdateAttendanceWithImageCommand, AttendancePerDayModel>
    {
        public CreateUpdateAttendanceWithImageHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
        }
        
         public override async Task<AttendancePerDayModel> Handle(CreateUpdateAttendanceWithImageCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(_context.UserId);
            var companyId =  _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.Id == _context.UserId)
                    .FirstOrDefault()?.CompanyId;
            var companyName =  _unitOfWork.CompanyRepository.GetQueryNoCached().Where(r => r.Id == companyId)
                    .FirstOrDefault()?.Name;

            var dateOnly = DateTime.Now.ToString("dd/MM/yyyy");
            // var dateOnly = DateOnly.FromDateTime(DateTime.Now);

            var todayEntry = _unitOfWork.AttendanceWithImageRepository.GetQueryNoCached().Where(r => r.P_Date.ToString() == dateOnly && r.UserId == _context.UserId).FirstOrDefault();

            string time = DateTime.Now.ToShortTimeString();

            string CurrentYear = DateTime.Now.Year.ToString();
            string CurrentMonth = DateTime.Now.ToString("MMMM");

            if(!Directory.Exists("E:\\Images\\" + companyName))
            {
                Directory.CreateDirectory("E:\\Images\\" + companyName);  
            }

            if(!Directory.Exists("E:\\Images\\" + companyName + "\\" + CurrentYear))
            {
                Directory.CreateDirectory("E:\\Images\\" + companyName + "\\" + CurrentYear);  
            }
            if(!Directory.Exists("E:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth))
            {
                Directory.CreateDirectory("E:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth);
            }
            if(!Directory.Exists("E:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In"))
            {
                Directory.CreateDirectory("E:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In");
            }
            if(!Directory.Exists("E:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out"))
            {
                Directory.CreateDirectory("E:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out");
            }
            
        }
    }
}