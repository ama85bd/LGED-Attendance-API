using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.Admin.UserProfile;
using LGED.Model.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers.UserController
{
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan) : base(logger, mediator, context, userMan)
        {
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(IApiResponse<bool>), 200)]
        public async Task<ActionResult<bool>> Create(CreateUpdateUserCommand command)
        {
            command.Id = Guid.NewGuid();
            return Ok(await _mediator.Send(command));
        }

        [HttpPost("admin-register")]
        [ProducesResponseType(typeof(IApiResponse<bool>), 200)]
        public async Task<ActionResult<bool>> Create(CreateUpdateAdminUserRegisterCommand command)
        {
            command.Id = Guid.NewGuid();
            return Ok(await _mediator.Send(command));
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}