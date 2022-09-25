using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Admin.CompanyUsers;
using LGED.Model.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.CompanyUsers
{
    public class EnableDisableCompanyUserHandler : HandlerBase<EnableDisableCompanyUserCommand, bool>
    {
        private readonly LgedUserManager _userMan;
        public EnableDisableCompanyUserHandler(IUnitOfWork unitOfWork, IUserContext context,LgedUserManager userMan) : base(unitOfWork, context)
        {
            _userMan = userMan;
        }

        public override async Task<bool> Handle(EnableDisableCompanyUserCommand command, CancellationToken cancellationToken)
        {
            
            var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId);
            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(_context.CurrentCompanyId);
            var userActive =  user.IsActive;
            var userRoleRepoComId = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == command.UserId)
                    .FirstOrDefaultAsync();
            var adminUserRoleRepo = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == _context.UserId).FirstOrDefaultAsync();
            var isAdmin = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Id == adminUserRoleRepo.RoleId)
                    .FirstOrDefault()?.Name;

            IdentityResult identifyUserResult;

            if(isAdmin != "Admin" || userRoleRepoComId.CompanyId !=_context.CurrentCompanyId 
            || userRoleRepoComId.CompanyId !=adminUserRoleRepo.CompanyId)
            {
                throw new ApiException("You have no permission for this action", (int)HttpStatusCode.BadRequest);
            }

            if(user.UserType == "Admin")
                {
                   throw new ApiException("You cannot enable or disable yourself!", (int)HttpStatusCode.BadRequest);
                }
            
            // foreach(var item in userIdInRoleRepo){
                
            // }
            
            if(user.Id == userRoleRepoComId.UserId)
            {
                user.IsActive = !userActive;

                identifyUserResult = await _userMan.UpdateAsync(user);
                
            }else
            {
                throw new ApiException("User not found", (int)HttpStatusCode.NotFound);
            }
            if (identifyUserResult.Succeeded && !identifyUserResult.Errors.Any() ) return true;
            throw new ApiException(identifyUserResult.Errors.Select(c => c.Description), (int)HttpStatusCode.NotAcceptable);
        }
    }
}