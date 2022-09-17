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

namespace LGED.Web.Controllers.AdminController
{
    [Authorize]
    [ApiController]
    public class AdminController : BaseController
    {
        public AdminController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan) : base(logger, mediator, context, userMan)
        {
        }

        /// <summary>
        /// Add user role
        /// </summary>
        /// <returns>Add User role</returns>

        [HttpPost("add-role")]
        public async Task<IActionResult> GetProcessDataPagedList(CreateUpdateRoleCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}