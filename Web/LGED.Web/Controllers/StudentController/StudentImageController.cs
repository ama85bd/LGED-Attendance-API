using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.StudentCommand;
using LGED.Model.Common;
using LGED.Model.Context;
using LGED.Model.Entities.Student;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LGED.Web.Controllers.StudentController
{
    [ApiController]
    public class StudentImageController : BaseController
    {
        private readonly LgedDbContext _dbContext;
        private readonly IWebHostEnvironment _hostEnvironment;
        public StudentImageController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan, LgedDbContext dbContext, IWebHostEnvironment hostEnvironment) : base(logger, mediator, context, userMan)
        {
            _hostEnvironment = hostEnvironment;
            _dbContext = dbContext;
        }
                
        /// <summary>
        /// Get student Image
        /// </summary>
        /// <returns>Get student Image student</returns>
        [HttpGet("getstudentimage")]
        public async Task<ActionResult<IEnumerable<StudentImage>>> GetStudentImages()
        {
             string CurrentYear = DateTime.Now.Year.ToString();
            string CurrentMonth = DateTime.Now.ToString("MMMM");
            int month = 11;
            int year = 2017;

            DateTime date = new DateTime(year, month, DateTime.Now.Day);

            var now = DateTime.Now; // get the current DateTime 

            //Get the number of days in the current month
            int daysInMonth = DateTime.DaysInMonth (year, month); 
                
            //First day of the month is always 1
            var firstDay = new DateTime(year,month,1).ToShortDateString(); 
                
            //Last day will be similar to the number of days calculated above
            var lastDay = new DateTime(year,month,daysInMonth).ToShortDateString();

            System.Console.WriteLine("date ========================= "+date.ToShortDateString());
            System.Console.WriteLine("firstDay ========================= "+firstDay);
            System.Console.WriteLine("lastDay ========================= "+lastDay);

            return await _dbContext.StudentImage
            .Select(x => new StudentImage(){
                EmpId = x.EmpId,
                EmpName = x.EmpName,
                ImageName =x.ImageName,
                ImageSrc = String.Format("{0}://{1}{2}/Images/{3}/{4}/In/{5}",Request.Scheme,Request.Host,Request.PathBase,CurrentYear,CurrentMonth,x.ImageName)
                
            })
            .ToListAsync();
        }
        
        /// <summary>
        /// Add student Image
        /// </summary>
        /// <returns>Add student Image student</returns>

        [HttpPost("addstudentimage")]
        public async Task<IActionResult> GetProcessDataPagedList([FromForm]StudentImage studentImage)
        {
            studentImage.ImageName = await SaveImage(studentImage.ImageFile);
            _dbContext.StudentImage.Add(studentImage);
            System.Console.WriteLine("studentImage ========================= "+studentImage);
            System.Console.WriteLine("studentImage ImageName========================= "+studentImage.ImageName);

            await _dbContext.SaveChangesAsync();
            return Ok(StatusCode(201));
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string CurrentYear = DateTime.Now.Year.ToString();
            string CurrentMonth = DateTime.Now.ToString("MMMM");
            if(!Directory.Exists("E:\\Images\\" + CurrentYear))
            {
                Directory.CreateDirectory("E:\\Images\\" + CurrentYear); 
                
            }
            if(!Directory.Exists("E:\\Images\\" + CurrentYear + "\\" + CurrentMonth))
            {
                Directory.CreateDirectory("E:\\Images\\" + CurrentYear + "\\" + CurrentMonth);
            }
            if(!Directory.Exists("E:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In"))
            {
                Directory.CreateDirectory("E:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In");
            }
            if(!Directory.Exists("E:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out"))
            {
                Directory.CreateDirectory("E:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out");
            }
            
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '_');
            imageName = DateTime.Now.ToString("ddMMyyyy") + Path.GetExtension(imageFile.FileName);
            // imageName = imageName+DateTime.Now.ToString("yymmdd") + Path.GetExtension(imageFile.FileName);
            // var imagePath = Path.Combine( _hostEnvironment.ContentRootPath,  "F:\\Images", imageName);
            var imagePath = Path.Combine( _hostEnvironment.ContentRootPath, string.Concat("E:\\Images\\" , CurrentYear , "\\" , CurrentMonth + "\\" + "In") , imageName);

            System.Console.WriteLine("CurrentYear ==================================CurrentYear   "+CurrentYear);
            System.Console.WriteLine("CurrentMonth ==================================CurrentMonth   "+CurrentMonth);
            System.Console.WriteLine("imagePath ==================================imagePath   "+imagePath);
            
            using ( var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}