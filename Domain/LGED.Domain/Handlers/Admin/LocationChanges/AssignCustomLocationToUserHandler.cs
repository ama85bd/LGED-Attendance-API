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
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace LGED.Domain.Handlers.Admin.LocationChanges
{
    public class AssignCustomLocationToUserHandler : HandlerBase<AssignCustomLocationToUserCommand, bool>
    {
        public AssignCustomLocationToUserHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
        }

        public override async Task<bool> Handle(AssignCustomLocationToUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId);
            

            var adminUserRoleRepo = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == _context.UserId).FirstOrDefaultAsync();
            var isAdmin = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Id == adminUserRoleRepo.RoleId)
                    .FirstOrDefault()?.Name;
            

            if(isAdmin != "Admin" || adminUserRoleRepo.CompanyId !=_context.CurrentCompanyId )
            {
                throw new ApiException("You have no permission for this action", (int)HttpStatusCode.BadRequest);
            }
            
            if(user != null)
            {
                    user.Location = new Point(command.Longitude, command.Latitude) { SRID = 4326 };
                    
                    _unitOfWork.UserRepository.Update(_context, user);
                   
                    return await _unitOfWork.CommitAsync();
             }else
             {
                throw new ApiException("The user not found", (int)HttpStatusCode.NotFound);
             }
                    
            
        
        }
    }
}