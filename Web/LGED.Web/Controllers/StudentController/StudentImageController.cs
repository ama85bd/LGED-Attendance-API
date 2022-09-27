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
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '_');
            imageName = imageName+DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine( _hostEnvironment.ContentRootPath, "Images", imageName);
            using ( var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}