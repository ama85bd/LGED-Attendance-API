using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Model.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LGED.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ResponseCache(CacheProfileName = "default")]
    public class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> _logger;
        protected readonly IMediator _mediator;
        protected readonly IUserContext _context;
        protected readonly LgedUserManager _userMan;
        public BaseController(ILogger<BaseController> logger, IMediator mediator, IUserContext context, LgedUserManager userMan)
        {
            _userMan = userMan;
            _logger = logger;
            _mediator = mediator;
            //user context have values only when user logged in
            _context = context;
        }
        //Avoid return Ok(null) that can cause unhandled exception
        //Another solution is just return NotFound() after check null
        protected ActionResult Ok<T>(T content)
        {
            if (content == null) return base.Ok();
            return base.Ok(content);
        }
    }
}