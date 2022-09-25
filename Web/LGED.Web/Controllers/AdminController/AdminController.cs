using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.Admin.CompanyUsers;
using LGED.Domain.Commands.Admin.LocationChanges;
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
        /// Get Approved User List
        /// </summary>
        /// <returns>Get Approved User List</returns>
        
        [HttpGet("GetApproveUserList")]
        public async Task<IActionResult> GetApproveUserList()
        {
            var command = new GetApproveUserListCommand();
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Request Access List
        /// </summary>
        /// <returns>Request Access List</returns>
        
        [HttpGet("RequestAccessList")]
        public async Task<IActionResult> GetRequestAccessList()
        {
            var command = new GetListForApproveByAdminCommand();
            return Ok(await _mediator.Send(command));
        }

        
        /// <summary>
        /// Update User to enable or disable
        /// </summary>
        /// <returns>Update User to enable or disable</returns>
        [HttpPut("EnableDisableUser")]
        public async Task<IActionResult> EnableDisableUser(EnableDisableCompanyUserCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        
        /// <summary>
        /// Delete user
        /// </summary>
        /// <returns>Delete user</returns>
        [HttpDelete("user")]
        [ProducesResponseType(typeof(IApiResponse<bool>), 200)]
        public async Task<ActionResult<bool>> DeleteUser(DeleteCompanyUserCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// Assign Custom Location To User
        /// </summary>
        /// <returns>Assign Custom Location To User</returns>

        [HttpPost("AssignCustomLocationToUser")]
        public async Task<IActionResult> AssignCustomLocationToUser(AssignCustomLocationToUserCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// User Location Assign To Admin Location
        /// </summary>
        /// <returns>User Location Assign To Admin Location</returns>

        [HttpPost("AdminLocationAssignToUserLocation")]
        public async Task<IActionResult> UserLocationAssignToAdminLocation(UserLocationSameAsAdminLocationCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// Update Company's location
        /// </summary>
        /// <returns>Update Company's location</returns>
        [HttpPut("updateCompanyLocation")]
        public async Task<IActionResult> UpdateCompanyLocation(UpdateCompanyLocationCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}