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
            var userIdInRoleRepo = _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.CompanyId == company.Id)
                    .FirstOrDefault()?.UserId;

            IdentityResult identifyUserResult;
            
            if(user.Id == userIdInRoleRepo)
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