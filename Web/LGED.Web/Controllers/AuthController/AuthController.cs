using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.Admin.Company;
using LGED.Domain.Commands.Admin.UserProfile;
using LGED.Domain.Commands.Attendance;
using LGED.Model.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers.AuthController
{
    [ApiController]
    public class AuthController : BaseController
    {
        public AuthController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan) : base(logger, mediator, context, userMan)
        {
        }
        
        /// <summary>
        /// Get Company List
        /// </summary>
        /// <returns>List of Company </returns>
        [HttpGet("GetAllCompanies/")]
        public async Task<IActionResult> GetAllCompanys()
        {
            var command = new GetCompanyListCommand();
            return Ok(await _mediator.Send(command));
        }


        /// <summary>
        /// Register for normal user
        /// </summary>
        /// <returns>Register for normal user</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(IApiResponse<bool>), 200)]
        public async Task<ActionResult<bool>> Create(CreateUpdateUserCommand command)
        {
            command.Id = Guid.NewGuid();
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Register for admin user
        /// </summary>
        /// <returns>Register for admin user</returns>
        [HttpPost("admin-register")]
        [ProducesResponseType(typeof(IApiResponse<bool>), 200)]
        public async Task<ActionResult<bool>> CreateAdminandCompany(CreateUpdateAdminUserRegisterCommand command)
        {
            command.Id = Guid.NewGuid();
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// User login and get token
        /// </summary>
        /// <returns>User login and get token</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        
        // /// <summary>
        // /// Image
        // /// </summary>
        // /// <returns>Bool</returns>
        // [AllowAnonymous]
        // [HttpPost("UserImage")]
        // public async Task<IActionResult> UserImage([FromForm]CreateUpdateAttendanceWithImageCommand command)
        // {
        //     return Ok(await _mediator.Send(command));
        // }
    }
}