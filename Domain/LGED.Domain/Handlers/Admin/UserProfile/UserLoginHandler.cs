using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using LGED.Core.Helper;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Core.Security;
using LGED.Core.Utils;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Admin.UserProfile;
using LGED.Domain.Models.Profile;
using LGED.Model.Common;
using LGED.Model.Entities.Profile;
using Microsoft.Extensions.Configuration;

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class UserLoginHandler : HandlerBase<UserLoginCommand, LoginCredentialModel>
    {
        private readonly IConfiguration _configuration;
        private readonly LgedUserManager _userManager;
        public UserLoginHandler(IUnitOfWork unitOfWork, IUserContext context, IConfiguration configuration, LgedUserManager userMan) : base(unitOfWork, context)
        {
            _userManager = userMan;
            _configuration = configuration;
        }

         public override async Task<LoginCredentialModel> Handle(UserLoginCommand command, CancellationToken cancellationToken)
        {
            User user;
            if (EmailUtils.CheckEmailFormat(command.Email))
            {

                user = await _userManager.FindByEmailAsync(command.Email.ToUpper());
            }
            else
            {
                user = await _userManager.FindByNameAsync(command.Email);
            }
            
            // Get all role
            if (user != null)
            {
                var isAssigned = _unitOfWork.UserRolesRepository.Exist(where: r => r.UserId == user.Id);
                if (!isAssigned) throw new ApiException("You don't have permission to access LGED attendance system.", (int)HttpStatusCode.Unauthorized);
            }

           
            //check database credentials
            
                if (user == null)
                {
                    throw new ApiException("Wrong username or email.", (int)HttpStatusCode.Unauthorized);
                }
                //not null
                if (await _userManager.CheckPasswordAsync(user, command.Password))
                {
                    return await GetCredentialModel(user);
                }
                throw new ApiException("Wrong password.", (int)HttpStatusCode.Unauthorized);
            
        }

        private async Task<LoginCredentialModel> GetCredentialModel(User user)
        {
            //user claims
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            //role claims - to use with [Authorization(Roles)] or Context.User.IsInRole which is using the roles issued in the jwt token
            //we also provide the permission check using HasPermission which has called to database
            //see PermissionAuthorizationHandler at the Web layer for more detail
            //also can use UserManager.IsInRole, which is used database call to check role as well
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            //check whether master admin
            if (userRoles.Contains("Master Administrator"))
            {
                authClaims.Add(new Claim(AppClaimTypes.IsMasterAdmin, "true"));
            }

            //generate token
            var token = AuthHelper.GenerateJwtToken(_configuration, authClaims);

            return new LoginCredentialModel
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }
    }
}