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
using LGED.Model.Entities.Profile;
using Microsoft.AspNetCore.Identity;

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class CreateUpdateAdminUserRegisterHandler : HandlerBase<CreateUpdateAdminUserRegisterCommand, bool>
    {
        private readonly LgedUserManager _userMan;
        public CreateUpdateAdminUserRegisterHandler(IUnitOfWork unitOfWork, IUserContext context,LgedUserManager userMan) : base(unitOfWork, context)
        {
            _userMan = userMan;
        }

        public override async Task<bool> Handle(CreateUpdateAdminUserRegisterCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = _context.UserId;
            }

            var useIdd = Guid.Empty;
            var compannyIdd = Guid.Empty;

            var user =  _unitOfWork.UserRepository.GetQueryNoCached().Where(u => u.UserName == command.Email)
                .FirstOrDefault();
            // var user = await _unitOfWork.UserRepository.GetByIdAsync(command.Id);

            var compannyName = _unitOfWork.CompanyRepository.GetQueryNoCached().Where(r => r.Name == command.Department)
                    .FirstOrDefault()?.Name;
            
            if(compannyName == null && command.UserType == "Admin")
            {
                var company = new Model.Entities.Profile.Company
                {
                    Name = command.Department,
                    Code = Guid.NewGuid().ToString(),
                    Description = command.Department,
                    ContactNumber = command.ContactNumber,
                    Department = command.Department,
                    Location = command.Location,
                    Type = "N/A",
                    IsActive = true
                };
                compannyIdd=company.Id;
                _unitOfWork.CompanyRepository.Add(_context, company);
            }else
            {
                throw new ApiException("The company already exist", (int)HttpStatusCode.NotFound);
            }
            

            IdentityResult identifyResult;

            if (user == null)
            {
                var password = command.Password;
                var autoId = Guid.NewGuid().ToString() ;

                var newUser = new User
                {
                    UserId = autoId,
                    UserName = command.Email,
                    Email = command.Email,
                    DisplayName = command.DisplayName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = command.ContactNumber,
                    UserType = command.UserType,
                    InsertedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Remarks = command.Department,
                    IsActive = false,
                    IsReceiveIowEmails = true,
                    IsReceiveDataAnomEmails = true,
                    IsReceiveCreepRemaningEmails = true
                };

                identifyResult = await _userMan.CreateAsync(newUser, password);
                useIdd=newUser.Id;
            }
            else
            {
                user.DisplayName = command.DisplayName;
                user.Email = command.Email;
                user.PhoneNumber = command.ContactNumber;

                identifyResult = await _userMan.UpdateAsync(user);
                useIdd=user.Id;
            }
            

            if (identifyResult.Succeeded && !identifyResult.Errors.Any()) {
                return await AssignUserToCompany(command,useIdd,compannyIdd);
                // return await AssignUserToCompany( user.Id,isMasterAdmin,roleIdofUser.ToString(),companyIdofUser.ToString());
            }
            throw new ApiException(identifyResult.Errors.Select(c => c.Description), (int)HttpStatusCode.NotAcceptable);
        }

        internal async Task<bool> AssignUserToCompany( CreateUpdateAdminUserRegisterCommand command, Guid userId,  Guid compnId)
        {
                var isMasterAdmin = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Name == command.UserType)
                    .FirstOrDefault()?.Name;
            
            var roleIdofUser = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Name == command.UserType)
                    .FirstOrDefault()?.Id.ToString();
            var companyIdofUser = _unitOfWork.CompanyRepository.GetQueryNoCached().Where(r => r.Name == command.Department)
                    .FirstOrDefault()?.Id.ToString();
            // System.Console.WriteLine("isMasterAdmin ==================================system   "+isMasterAdmin);
            // System.Console.WriteLine("roleIdofUser ==================================system   "+roleIdofUser);
            // System.Console.WriteLine("companyIdofUser ==================================system   "+companyIdofUser);
            
            if (isMasterAdmin == "Master Admin")
            {
                // remove old roles
                var userRoles =  _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == userId)
                    .ToList();
                foreach (var r in userRoles)
                {
                    _unitOfWork.UserRolesRepository.Remove(r);
                }

                // assign master admin for all companies
                var companyId = _unitOfWork.CompanyRepository.GetQueryNoCached().Select(c => c.Id).ToList();
                foreach (var userRole in companyId.Select(comGuid => new UserRoles
                {
                    RoleId = new Guid(roleIdofUser),
                    CompanyId = comGuid,
                    UserId = userId
                }))
                {
                    _unitOfWork.UserRolesRepository.Add(_context, userRole);
                }
            }
            else
            {
                var userRole = new UserRoles
                {
                    RoleId = new Guid(roleIdofUser),
                    CompanyId = compnId,
                    UserId = userId
                };
            // System.Console.WriteLine("userRole UserId ==================================system   "+userRole.UserId);
            // System.Console.WriteLine("userRole CompanyId ==================================system   "+userRole.CompanyId);
            // System.Console.WriteLine("userRole RoleId ==================================system   "+userRole.RoleId);
                _unitOfWork.UserRolesRepository.Add(_context, userRole);
            }
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}