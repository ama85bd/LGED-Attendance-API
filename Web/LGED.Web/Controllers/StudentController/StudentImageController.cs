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
        /// Add student Image
        /// </summary>
        /// <returns>Add student Image student</returns>

        [HttpPost("addstudentimage")]
        public async Task<IActionResult> GetProcessDataPagedList([FromForm]StudentImage studentImage)
        {
            studentImage.ImageName = await SaveImage(studentImage.ImageFile);
            _dbContext.StudentImage.Add(studentImage);
            await _dbContext.SaveChangesAsync();
            return Ok(StatusCode(201));
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string CurrentYear = DateTime.Now.Year.ToString();
            string CurrentMonth = DateTime.Now.ToString("MMMM");
            if(!Directory.Exists("F:\\Images\\" + CurrentYear))
            {
                Directory.CreateDirectory("F:\\Images\\" + CurrentYear); 
                
            }
            if(!Directory.Exists("F:\\Images\\" + CurrentYear + "\\" + CurrentMonth))
            {
                Directory.CreateDirectory("F:\\Images\\" + CurrentYear + "\\" + CurrentMonth);
            }
            if(!Directory.Exists("F:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In"))
            {
                Directory.CreateDirectory("F:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "In");
            }
            if(!Directory.Exists("F:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out"))
            {
                Directory.CreateDirectory("F:\\Images\\" + CurrentYear + "\\" + CurrentMonth + "\\" + "Out");
            }
            
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '_');
            imageName = DateTime.Now.ToString("ddMMyyyy") + Path.GetExtension(imageFile.FileName);
            // imageName = imageName+DateTime.Now.ToString("yymmdd") + Path.GetExtension(imageFile.FileName);
            // var imagePath = Path.Combine( _hostEnvironment.ContentRootPath,  "F:\\Images", imageName);
            var imagePath = Path.Combine( _hostEnvironment.ContentRootPath, string.Concat("F:\\Images\\" , CurrentYear , "\\" , CurrentMonth) , imageName);

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