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

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class UpdateUserProfileByIdHandler : HandlerBase<UpdateUserProfileByIdCommand, bool>
    {
        private readonly LgedUserManager _userMan;
        public UpdateUserProfileByIdHandler(IUnitOfWork unitOfWork, IUserContext context,LgedUserManager userMan) : base(unitOfWork, context)
        {
            _userMan = userMan;
        }

        public override async Task<bool> Handle(UpdateUserProfileByIdCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == Guid.Empty)
            {
                command.Id = _context.UserId;
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(command.Id);
            

            IdentityResult identifyUserResult;

            if (user != null)
            {
                user.DisplayName = command.DisplayName;
                user.Designation = command.Designation;
                user.PhoneNumber = command.ContactNumber;
                user.ProfileImage = command.UserImage;

                identifyUserResult = await _userMan.UpdateAsync(user);
            }
            else
            {
                throw new ApiException("User not exist", (int)HttpStatusCode.NotFound);
            }

            

            if (identifyUserResult.Succeeded && !identifyUserResult.Errors.Any()) return true;
            throw new ApiException(identifyUserResult.Errors.Select(c => c.Description), (int)HttpStatusCode.NotAcceptable);
        }
    }
}