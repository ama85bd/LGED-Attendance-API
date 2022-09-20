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
using LGED.Model.Common;
using LGED.Model.Entities.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class CreateUpdateUserHandler : HandlerBase<CreateUpdateUserCommand, Guid>
    {
        private readonly LgedUserManager _userMan;
        public CreateUpdateUserHandler(IUnitOfWork unitOfWork, IUserContext context,LgedUserManager userMan, IMapper mapper) : base(unitOfWork, context,mapper)
        {
            _userMan = userMan;
        }

        public override async Task<Guid> Handle(CreateUpdateUserCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = _context.UserId;
            }
            

            var user =  _unitOfWork.UserRepository.GetQueryNoCached().Where(u => u.UserName == command.Email)
                .FirstOrDefault();
            // var user = await _unitOfWork.UserRepository.GetByIdAsync(command.Id);

            IdentityResult identifyResult;

            var useIdd = Guid.Empty;

            var companyLocationofUser = _unitOfWork.CompanyRepository.GetQueryNoCached().Where(r => r.Name == command.Department)
                    .FirstOrDefault()?.Location;

            if (user == null)
            {
                var password = command.Password;
                var autoId = Guid.NewGuid().ToString() ;
                var result = _mapper.Map<User>(command);
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
                    Designation = command.Designation,
                    ProfileImage = command.UserImage,
                    Location = companyLocationofUser,
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
                user.Designation = command.Designation;
                user.ProfileImage = command.UserImage;
                identifyResult = await _userMan.UpdateAsync(user);
                useIdd=user.Id;
            }
            

            if (identifyResult.Succeeded && !identifyResult.Errors.Any()) {
                return await AssignUserToCompany(command,useIdd);
            
            }
            throw new ApiException(identifyResult.Errors.Select(c => c.Description), (int)HttpStatusCode.NotAcceptable);
        }

        internal async Task<Guid> AssignUserToCompany( CreateUpdateUserCommand command, Guid userId)
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
                var userRoles = await _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r => r.UserId == userId)
                    .ToListAsync();
            // System.Console.WriteLine("userRoles ==================================system   "+userRoles);
                foreach (var r in userRoles)
                {
                    _unitOfWork.UserRolesRepository.Remove(r);
                }

                // assign master admin for all companies
                var companyId = _unitOfWork.CompanyRepository.GetQueryNoCached().Select(c => c.Id).ToListAsync().Result;
                
                foreach (var userRole in companyId.Select(comGuid => new UserRoles
                {
                    
                    RoleId = new Guid(roleIdofUser),
                    CompanyId = comGuid,
                    UserId = userId
                }))
                {
            // System.Console.WriteLine("userRole ==================================system   "+userRole.RoleId);
            // System.Console.WriteLine("userRole ==================================system   "+userRole.CompanyId);
            // System.Console.WriteLine("userRole ==================================system   "+userRole.UserId);
                    _unitOfWork.UserRolesRepository.Add(_context, userRole);
                }
            }
            else
            {
                var userRole = new UserRoles
                {
                    RoleId = new Guid(roleIdofUser),
                    CompanyId = new Guid(companyIdofUser),
                    UserId = userId
                };
            // System.Console.WriteLine("userRole UserId ==================================system   "+userRole.UserId);
            // System.Console.WriteLine("userRole CompanyId ==================================system   "+userRole.CompanyId);
            // System.Console.WriteLine("userRole RoleId ==================================system   "+userRole.RoleId);
                _unitOfWork.UserRolesRepository.Add(_context, userRole);
            }
            await _unitOfWork.CommitAsync();
            return userId;
        }
    }
}