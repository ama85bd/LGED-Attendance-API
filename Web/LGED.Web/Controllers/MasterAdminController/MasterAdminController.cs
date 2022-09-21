using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.Admin.Company;
using LGED.Domain.Commands.Admin.UserProfile;
using LGED.Model.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers.MasterAdminController
{
    [Authorize]
    [ApiController]
    public class MasterAdminController : BaseController
    {
        public MasterAdminController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan) : base(logger, mediator, context, userMan)
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


        /// <summary>
        /// Request Access List
        /// </summary>
        /// <returns>Request Access List</returns>
        
        [HttpGet("RequestAccessList")]
        public async Task<IActionResult> GetRequestAccessList()
        {
            var command = new GetApproveByMasterAdminCommand();
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Update User and company to enable or disable
        /// </summary>
        /// <returns>Update User and company to enable or disable</returns>
        [HttpPut("EnableDisableUserCompany")]
        public async Task<IActionResult> EnableDisableUserCompany(ActiveUserCompanyCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
    }
}