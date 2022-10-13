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
using NetTopologySuite.Geometries;

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
            var compannyIdGet = _unitOfWork.CompanyRepository.GetQueryNoCached().Where(r => r.Name == command.Department)
                    .FirstOrDefault()?.Id;
            var roleId = _unitOfWork.RoleRepository.GetQueryNoCached().Where(r => r.Name == "Admin")
                    .FirstOrDefault()?.Id.ToString();
            if(compannyIdGet != null)
            {
            var isAdminHas = _unitOfWork.UserRolesRepository.GetQueryNoCached().Where(r =>  r.CompanyId == compannyIdGet)
            .FirstOrDefault()?.RoleId.ToString().Contains(roleId);

            if( (bool)isAdminHas){
                throw new ApiException("One admin user already exist in this company", (int)HttpStatusCode.Conflict);
            }
            }

            if(compannyName == null && command.UserType == "Admin")
            {
                var company = new Model.Entities.Profile.Company
                {
                    Name = command.Department,
                    Code = Guid.NewGuid().ToString(),
                    Description = command.Department,
                    ContactNumber = command.ContactNumber,
                    Department = command.Department,
                    Latitude = command.Latitude !=0 ? command.Latitude : 23.77746215446655,
                    Longitude = command.Longitude !=0 ? command.Longitude : 90.37791764232794,
                    SRID = 4326,
                    Location = new Point(command.Longitude !=0 ? command.Longitude : 90.37791764232794,
                     command.Latitude !=0 ? command.Latitude : 23.77746215446655) { SRID = 4326 },
                    
                    Type = "N/A",
                    IsActive = false
                };
                System.Console.WriteLine("company location ==================================system   "+company.Location);
                compannyIdd=company.Id;
                _unitOfWork.CompanyRepository.Add(_context, company);
            }else
            {
                throw new ApiException("The company already exist", (int)HttpStatusCode.Conflict);
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
                    Designation = command.Designation,
                    ProfileImage = command.UserImage,
                    Location = new Point(command.Longitude !=0 ? command.Longitude : 90.37791764232794,
                     command.Latitude !=0 ? command.Latitude : 23.77746215446655) { SRID = 4326 },
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