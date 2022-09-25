using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Admin.UserProfile;
using LGED.Model.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class ActiveUserCompanyHandler : HandlerBase<ActiveUserCompanyCommand, bool>
    {
        private readonly LgedUserManager _userMan;
        public ActiveUserCompanyHandler(IUnitOfWork unitOfWork, IUserContext context,LgedUserManager userMan) : base(unitOfWork, context)
        {
            _userMan = userMan;
        }

        public override async Task<bool> Handle(ActiveUserCompanyCommand command, CancellationToken cancellationToken)
        {
            if (command.UserId == Guid.Empty)
            {
                command.UserId = _context.UserId;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId);
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(command.CompanyId);
            var userActive =  user.IsActive;
            var companyActive =  company.IsActive;
           

            IdentityResult identifyUserResult;
            IdentityResult identifyCompanyResult;

            
            // var adminUserRoleRepo = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == _context.UserId).FirstOrDefaultAsync();
            var isAdmin = _unitOfWork.UserRepository.GetQueryNoCached().Where(r => r.Id == _context.UserId)
                    .FirstOrDefault()?.UserType;
            // System.Console.WriteLine("adminUserRoleRepo =========================Id   "+adminUserRoleRepo);
            // System.Console.WriteLine("isAdmin =========================Id   "+isAdmin);



            if(isAdmin != "Master Admin" )
            {
                throw new ApiException("You have no permission for this action", (int)HttpStatusCode.BadRequest);
            }

            if (user != null)
            {
                user.IsActive = !userActive;

                identifyUserResult = await _userMan.UpdateAsync(user);
            }
            else
            {
                throw new ApiException("User not exist", (int)HttpStatusCode.NotFound);
            }

            if (company != null)
            {
                company.IsActive = !companyActive;

                identifyCompanyResult = await _userMan.UpdateAsync(user);
            }
            else
            {
                throw new ApiException("Company not exist", (int)HttpStatusCode.NotFound);
            }

            if (identifyUserResult.Succeeded && !identifyUserResult.Errors.Any() && identifyCompanyResult.Succeeded && !identifyCompanyResult.Errors.Any()) return true;
            throw new ApiException(identifyUserResult.Errors.Select(c => c.Description), (int)HttpStatusCode.NotAcceptable);
        }
    }
}