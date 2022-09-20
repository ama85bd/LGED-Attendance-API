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
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.CompanyUsers
{
    public class DeleteCompanyUserHandler : HandlerBase<DeleteCompanyUserCommand, bool>
    {
        public DeleteCompanyUserHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {

        }

        public override async Task<bool> Handle(DeleteCompanyUserCommand command, CancellationToken cancellationToken)
        {
            if (command.UserId == _context.UserId)
            {
                throw new ApiException("Cannot delete your self", (int)HttpStatusCode.Conflict);
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsync(command.UserId);
            var userRoleRepo =  _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == command.UserId)
                    .ToList();
            var userRoleRepoComId = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == command.UserId)
                    .FirstOrDefaultAsync();
            var adminUserRoleRepo = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == _context.UserId).FirstOrDefaultAsync();
            var isAdmin = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Id == adminUserRoleRepo.RoleId)
                    .FirstOrDefault()?.Name;
           
            // if(userRoleRepo != null )
            // {
            //      System.Console.WriteLine("IF IF ==================================system   ");

            // }else{
            //      System.Console.WriteLine("ELSE ELSE ==================================system   ");

            // }
            if(isAdmin != "Admin" || userRoleRepoComId.CompanyId !=_context.CurrentCompanyId 
            || userRoleRepoComId.CompanyId !=adminUserRoleRepo.CompanyId)
            {
                throw new ApiException("You cannot delete this user", (int)HttpStatusCode.NotFound);
            }

            if (user != null && userRoleRepo != null)
            {
                _unitOfWork.UserRepository.Remove( user);
                foreach (var r in userRoleRepo)
                {
                    _unitOfWork.UserRolesRepository.Remove(r);
                }
                // _unitOfWork.UserRolesRepository.Delete(_context, userRoleRepo);
            }
            else
            {
               
                throw new ApiException("User not exist on this system", (int)HttpStatusCode.NotFound);
            }

            return await _unitOfWork.CommitAsync();
        }
    }
}