using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Attendance;
using LGED.Domain.Models.Attendance;
using LGED.Model.Entities.Attendance;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LGED.Domain.Handlers.Attendance
{
    public class CreateUpdateAttendanceWithImageHandler : HandlerBase<CreateUpdateAttendanceWithImageCommand, bool>
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        public CreateUpdateAttendanceWithImageHandler(IUnitOfWork unitOfWork, IUserContext context, IWebHostEnvironment hostEnvironment) : base(unitOfWork, context)
        {
            _hostEnvironment = hostEnvironment;
        }
        
         public override async Task<bool> Handle(CreateUpdateAttendanceWithImageCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(_context.UserId);
            var companyId =  _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == _context.UserId)
                    .FirstOrDefault()?.CompanyId;
            var companyName =  _unitOfWork.CompanyRepository.GetQueryNoCached().Where(r => r.Id == companyId)
                    .FirstOrDefault()?.Name;

            var dateOnly = DateTime.Now.ToString("ddMMyyyy");
            var todayDateOnly = DateTime.Now.ToShortDateString();

            var inImageName = "In"+ "--" + _context.UserId + "--"+ _context.CurrentCompanyId +"--"+ dateOnly + Path.GetExtension(command.ImageFile.FileName);

            var todayEntry = _unitOfWork.AttendanceWithImageRepository.GetQueryNoCached().Where(r => r.P_Date == DateTime.Parse(todayDateOnly) && r.UserId == _context.UserId && r.InImage == inImageName).FirstOrDefault();

            string currentTime = DateTime.Now.ToLongTimeString();

            string CurrentYear = DateTime.Now.Year.ToString();
            string CurrentMonth = DateTime.Now.ToString("MMMM");
            
            if(!Directory.Exists("C:\\Images\\" + companyName))
            {
                Directory.CreateDirectory("C:\\Images\\" + companyName);  
            }

            if(!Directory.Exists("C:\\Images\\" + companyName + "\\" + CurrentYear))
            {
                Directory.CreateDirectory("C:\\Images\\" + companyName + "\\" + CurrentYear);  
            }
            if(!Directory.Exists("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth))
            {
                Directory.CreateDirectory("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth);
            }
            if(!Directory.Exists("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In"))
            {
                Directory.CreateDirectory("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In");
            }
            if(!Directory.Exists("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out"))
            {
                Directory.CreateDirectory("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out");
            }

            if(companyId == _context.CurrentCompanyId)
            {
                if(todayEntry == null)
                    {
                        AttendanceWithImage attendanceWithImage = new AttendanceWithImage
                        {
                            UserId = _context.UserId,
                            CompanyId = _context.CurrentCompanyId,
                            InTime = DateTime.Parse(currentTime),
                            P_Date = DateTime.Parse(todayDateOnly),
                            InImage = await SaveInImage(command.ImageFile,companyName,CurrentYear,CurrentMonth),
                        };
                        _unitOfWork.AttendanceWithImageRepository.Add(_context, attendanceWithImage);
                    }else{
                    
                            todayEntry.OutTime = DateTime.Parse(currentTime);
                            todayEntry.OutImage = await SaveOutImage(command.ImageFile,companyName,CurrentYear,CurrentMonth);
                        
                        _unitOfWork.AttendanceWithImageRepository.Update(_context, todayEntry);
                    }

            
            }
            else
            {
                throw new ApiException("Company not found", (int)HttpStatusCode.NotFound);
            }
            return await _unitOfWork.CommitAsync();
        }

        public async Task<string> SaveInImage(IFormFile imageFile, string companyName, string CurrentYear, string CurrentMonth)
        {
            
            
           string imageName = "In"+ "--" + _context.UserId + "--"+ _context.CurrentCompanyId +"--"+ DateTime.Now.ToString("ddMMyyyy") + Path.GetExtension(imageFile.FileName);
            // imageName = imageName+DateTime.Now.ToString("yymmdd") + Path.GetExtension(imageFile.FileName);
            // var imagePath = Path.Combine( _hostEnvironment.ContentRootPath,  "F:\\Images", imageName);
            var imagePath = Path.Combine( _hostEnvironment.ContentRootPath, string.Concat("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In") , imageName);

            using ( var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }

        public async Task<string> SaveOutImage(IFormFile imageFile, string companyName, string CurrentYear, string CurrentMonth)
        {
            
            
           string imageName = "Out" + "--" + _context.UserId + "--"+ _context.CurrentCompanyId +"--"+ DateTime.Now.ToString("ddMMyyyy") + Path.GetExtension(imageFile.FileName);
            // imageName = imageName+DateTime.Now.ToString("yymmdd") + Path.GetExtension(imageFile.FileName);
            // var imagePath = Path.Combine( _hostEnvironment.ContentRootPath,  "F:\\Images", imageName);
            var imagePath = Path.Combine( _hostEnvironment.ContentRootPath, string.Concat("C:\\Images\\" + companyName + "\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out") , imageName);

            using ( var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}