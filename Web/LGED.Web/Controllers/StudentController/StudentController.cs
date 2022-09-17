using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.StudentCommand;
using LGED.Model.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers.StudentController
{ 
    [Authorize]
    [ApiController]
    public class StudentController : BaseController
    {
        public StudentController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan) : base(logger, mediator, context, userMan)
        {
        }

        /// <summary>
        /// Add student List
        /// </summary>
        /// <returns>List of Search student</returns>

        [HttpPost("addstudent")]
        public async Task<IActionResult> GetProcessDataPagedList(CreateUpdateStudentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Get student List
        /// </summary>
        /// <returns>List of student List</returns>
        [HttpGet("GetAllStudents/")]
        public async Task<IActionResult> GetAllStudents()
        {
            var command = new GetStudentListCommand();
            return Ok(await _mediator.Send(command));
        }
    }
}