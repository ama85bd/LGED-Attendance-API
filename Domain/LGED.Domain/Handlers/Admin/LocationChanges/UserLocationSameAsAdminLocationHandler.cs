using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Admin.LocationChanges;
using LGED.Model.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.LocationChanges
{
    public class UserLocationSameAsAdminLocationHandler : HandlerBase<UserLocationSameAsAdminLocationCommand, bool>
    {
        private readonly LgedUserManager _userMan;
        public UserLocationSameAsAdminLocationHandler(IUnitOfWork unitOfWork, IUserContext context,LgedUserManager userMan) : base(unitOfWork, context)
        {
            _userMan = userMan;
        }

        public override async Task<bool> Handle(UserLocationSameAsAdminLocationCommand command, CancellationToken cancellationToken)
        {
            if (command.UserId == Guid.Empty)
            {
                command.UserId = _context.UserId;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId);
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(_context.CurrentCompanyId);

            IdentityResult identifyUserResult;

            var adminUserRoleRepo = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == _context.UserId).FirstOrDefaultAsync();
            var isAdmin = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Id == adminUserRoleRepo.RoleId)
                    .FirstOrDefault()?.Name;
            

            if(isAdmin != "Admin" || adminUserRoleRepo.CompanyId !=_context.CurrentCompanyId )
            {
                throw new ApiException("You have no permission for this action", (int)HttpStatusCode.BadRequest);
            }
            
            if (company == null)
            {
                throw new ApiException("Company not exist", (int)HttpStatusCode.NotFound);
            }
            

            if (user != null)
            {
                user.Location = company.Location;

                identifyUserResult = await _userMan.UpdateAsync(user);
            }
            else
            {
                throw new ApiException("User not exist", (int)HttpStatusCode.NotFound);
            }


            if (identifyUserResult.Succeeded && !identifyUserResult.Errors.Any()) return true;
            throw new ApiException(identifyUserResult.Errors.Select(c => c.Description), (int)HttpStatusCode.NotAcceptable);
        }
    }
}