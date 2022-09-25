using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using LGED.Core.Interfaces;
using LGED.Core.Model;
using LGED.Data.Base;
using LGED.Domain.Base;
using LGED.Domain.Commands.Admin.UserProfile;
using LGED.Domain.Models.Profile;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class GetApproveByMasterAdminHandler : HandlerBase<GetApproveByMasterAdminCommand, List<GetApproveByMasterAdminModel>>
    {
        public GetApproveByMasterAdminHandler(IUnitOfWork unitOfWork, IUserContext context, IMapper mapper) : base(unitOfWork, context, mapper)
        {
        }

        public override async Task<List<GetApproveByMasterAdminModel>> Handle(GetApproveByMasterAdminCommand command, CancellationToken cancellationToken)
        {
            
           var isAdmin = _unitOfWork.UserRepository.GetQueryNoCached().Where(r => r.Id == _context.UserId)
                    .FirstOrDefault()?.UserType;


            if(isAdmin != "Master Admin" )
            {
                throw new ApiException("You have no permission for this action", (int)HttpStatusCode.BadRequest);
            }
            
            var result = await (from user in _unitOfWork.UserRepository.GetQueryNoCached() 
                        join userRole in _unitOfWork.UserRolesRepository.GetQueryNoCached() on user.Id equals userRole.UserId  
                        join comp in _unitOfWork.CompanyRepository.GetQueryNoCached()  on userRole.CompanyId equals comp.Id                     
                        where user.IsActive == false && comp.IsActive == false
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