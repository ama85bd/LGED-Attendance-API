using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.Admin.UserProfile;
using LGED.Domain.Models.Profile;
using LGED.Model.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers.UserController
{
    [Authorize]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan) : base(logger, mediator, context, userMan)
        {
        }


        /// <summary>
        /// Get user info by user's id
        /// </summary>
        /// <param name="id"> User Guid</param>
        /// <returns> User Info details include user ID, User email, list of Company with Role</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IApiResponse<UserDetailModel>), 200)]
        public async Task<IActionResult> GetUserInfoByUserId(Guid id)
        {
            var command = new GetAllUserListByIdCommand { Id = id };
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Update User by id
        /// </summary>
        /// <returns>Update User by id</returns>
        [HttpPut("UpdateUserById")]
        public async Task<IActionResult> UpdateUserById(UpdateUserProfileByIdCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

    }
}