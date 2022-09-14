using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.StudentCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers.StudentController
{
    public class CreateUpdateStudentController : BaseController
    {
        public CreateUpdateStudentController(ILogger<BaseController> logger, IMediator mediator, IUserContext context) : base(logger, mediator, context)
        {
        }

        [HttpPost("addstudent")]
        public async Task<IActionResult> GetProcessDataPagedList(CreateUpdateStudentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}