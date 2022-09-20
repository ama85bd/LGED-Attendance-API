using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LGED.Core.Constants;
using LGED.Core.Interfaces;
using LGED.Core.Security;
using Microsoft.AspNetCore.Http;

namespace LGED.Model.Context
{
    public class UserContext: IUserContext
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        private IEnumerable<Claim> GetUserClaims()
        {
            var claimsIdentity = (ClaimsIdentity)HttpContextAccessor.HttpContext.User.Identity;
            return claimsIdentity.Claims;
        }

        public override Guid UserId
        {
            get
            {
                var userId = Guid.Empty;
                var userIdClaim = GetUserClaims().SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                }

                return userId;
            }
        }

         public override string UserName
        {
            get
            {
                var userName = "";
                var userNameClaim = GetUserClaims().SingleOrDefault(c => c.Type == ClaimTypes.Name);
                if (userNameClaim != null)
                {
                    userName = userNameClaim.Value;
                }

                return userName;
            }
        }

        public override Guid CurrentCompanyId
        {
            get
            {
                var companyId = Guid.Empty;
                var compId = HttpContextAccessor.HttpContext.Request
                    .Headers[AppConstants.HttpHeader.LGED_COMPANY_ID_HEADER]
                    .FirstOrDefault();
                if (compId != null)
                {
                    if (Guid.TryParse(compId, out var guidOutput))
                    {
                        companyId = guidOutput;
                    }
                }

                return companyId;
            }
        }

        public override string UserType
        {
            get
            {
                var userType = "";
                var userTypeClaim = GetUserClaims().SingleOrDefault(c => c.Type == AppClaimTypes.UserType);
                if (userTypeClaim != null)
                {
                    userType = userTypeClaim.Value;
                }

                return userType;
            }
        }
    }
}