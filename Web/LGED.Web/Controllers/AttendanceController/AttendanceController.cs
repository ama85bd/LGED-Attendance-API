using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Domain.Commands.Attendance;
using LGED.Domain.Models.Attendance;
using LGED.Model.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers.AttendanceController
{
    [Authorize]
    [ApiController]
    public class AttendanceController : BaseController
    {
        public AttendanceController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan) : base(logger, mediator, context, userMan)
        {
        }
        
        /// <summary>
        /// User attendance with image
        /// </summary>
        /// <returns>Bool</returns>
        [HttpPost("UserAttendance")]
        public async Task<IActionResult> UserImage([FromForm]CreateUpdateAttendanceWithImageCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// Get User attendance with image by user's id
        /// </summary>
        /// <param name="userid"> User Guid</param>
        /// <returns>List of User attendance with image</returns>
        
        [HttpGet("CurrentDateOneUser/{userid}")]
        [ProducesResponseType(typeof(IApiResponse<AttendancePerDayModel>), 200)]
        public async Task<IActionResult> GetUserAttendancePerDay(Guid userid)
        {
            var command = new GetUserDailyAttendanceCommand { UserId = userid, Scheme = Request.Scheme, Host=Request.Host.ToString(), PathBase = Request.PathBase };
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// Get All company User attendance with image
        /// </summary>
        /// <returns>List of All company User attendance with image</returns>
        
        [HttpGet("CurrentDateAllCompanyUser")]
        [ProducesResponseType(typeof(IApiResponse<AttendancePerDayModel>), 200)]
        public async Task<IActionResult> GetAllCompanyUserAttendancePerDay()
        {
            var command = new GetAllUserDailyAttendanceByCompanyCommand { Scheme = Request.Scheme, Host=Request.Host.ToString(), PathBase = Request.PathBase };
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// Get current month user attendance with image by user's id
        /// </summary>
        /// <param name="userid"> user Guid</param>
        /// <returns>List of current month user attendance with image</returns>
        
        [HttpGet("CurrentMonthOneUser/{userid}")]
        [ProducesResponseType(typeof(IApiResponse<AttendancePerDayModel>), 200)]
        public async Task<IActionResult> GetUserAttendanceCurrentMonth(Guid userid)
        {
            var command = new GetUserCurrentMonthAttendanceCommand { UserId = userid, Scheme = Request.Scheme, Host=Request.Host.ToString(), PathBase = Request.PathBase };
            return Ok(await _mediator.Send(command));
        }

         
        /// <summary>
        /// Get all company user current month attendance with image
        /// </summary>
        /// <returns>List of all company user current month attendance with image</returns>
        
        [HttpGet("CurrentMonthAllCompanyUser")]
        [ProducesResponseType(typeof(IApiResponse<AttendancePerDayModel>), 200)]
        public async Task<IActionResult> GetAllCompanyUserAttendanceCurrentMonth()
        {
            var command = new GetAllUserCurrentMonthAttendanceByCompanyCommand { Scheme = Request.Scheme, Host=Request.Host.ToString(), PathBase = Request.PathBase };
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// Get custom year month user attendance with image by user's id
        /// </summary>
        /// <param name="userid"> user Guid</param>
        /// <param name="year"> int year</param>
        /// <param name="month"> int month</param>
        /// <returns>List of custom year month user attendance with image</returns>
        
        [HttpGet("CustomYearMonthOneUser/{userid}/{year}/{month}")]
        [ProducesResponseType(typeof(IApiResponse<AttendancePerDayModel>), 200)]
        public async Task<IActionResult> GetUserAttendanceCustomYearMonth(Guid userid, int year, int month)
        {
            var command = new GetUserCustomYearMonthAttendanceCommand { UserId = userid, Year=year, Month=month, Scheme = Request.Scheme, Host=Request.Host.ToString(), PathBase = Request.PathBase };
            return Ok(await _mediator.Send(command));
        }
        
        /// <summary>
        /// Get custom year month all company user attendance with image
        /// </summary>
        /// <param name="year"> int year</param>
        /// <param name="month"> int month</param>
        /// <returns>List of custom year month all company user attendance with image</returns>
        
        [HttpGet("CustomYearMonthAllCompanyUser/{year}/{month}")]
        [ProducesResponseType(typeof(IApiResponse<AttendancePerDayModel>), 200)]
        public async Task<IActionResult> GetAllCompanyUserAttendanceCustomYearMonth( int year, int month)
        {
            var command = new GetAllUserCustomYearMonthAttendanceByCompanyCommand { Year=year, Month=month, Scheme = Request.Scheme, Host=Request.Host.ToString(), PathBase = Request.PathBase };
            return Ok(await _mediator.Send(command));
        }
    }
}