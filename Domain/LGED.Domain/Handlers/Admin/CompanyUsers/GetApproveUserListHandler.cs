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
using LGED.Domain.Models.Profile;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.CompanyUsers
{
    public class GetApproveUserListHandler : HandlerBase<GetApproveUserListCommand, List<GetApproveByMasterAdminModel>>
    {
        public GetApproveUserListHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
        }

        public override async Task<List<GetApproveByMasterAdminModel>> Handle(GetApproveUserListCommand command, CancellationToken cancellationToken)
        {
            // var userRoleRepoComId = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == command.UserId)
            //         .FirstOrDefaultAsync();
            var adminUserRoleRepo = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == _context.UserId).FirstOrDefaultAsync();
            var isAdmin = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Id == adminUserRoleRepo.RoleId)
                    .FirstOrDefault()?.Name;
            

            if(isAdmin != "Admin" || adminUserRoleRepo.CompanyId !=_context.CurrentCompanyId )
            {
                throw new ApiException("You have no permission for this action", (int)HttpStatusCode.BadRequest);
            }

            var result = await (from user in _unitOfWork.UserRepository.GetQueryNoCached() 
                        join userRole in _unitOfWork.UserRolesRepository.GetQueryNoCached() on user.Id equals userRole.UserId  
                        join comp in _unitOfWork.CompanyRepository.GetQueryNoCached()  on userRole.CompanyId equals comp.Id                     
                        where user.IsActive == true && comp.Id == _context.CurrentCompanyId && comp.IsActive == true
                          orderby user.DisplayName
                          select new GetApproveByMasterAdminModel
                          {
                              UserId = user.Id,
                              CompanyId = comp.Id,
                              UserName = user.DisplayName,
                              UserType = user.UserType,
                              UserEmail = user.Email,
                              CompanyName = comp.Name

                          }).Distinct().ToListAsync(cancellationToken);

                          return result;
        }
    }
}