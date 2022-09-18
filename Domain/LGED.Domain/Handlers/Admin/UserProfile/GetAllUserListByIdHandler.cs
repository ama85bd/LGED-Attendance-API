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
using LGED.Domain.Models.Profile;
using Microsoft.EntityFrameworkCore;

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class GetAllUserListByIdHandler : HandlerBase<GetAllUserListByIdCommand, UserDetailModel>
    {
        public GetAllUserListByIdHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
        }

        public override async Task<UserDetailModel> Handle(GetAllUserListByIdCommand command, CancellationToken cancellationToken)
        {

            if (command.Id == null) command.Id = _context.UserId; // if null, get from current request user

            var user = await _unitOfWork.UserRepository.GetQuery().FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken: cancellationToken);

            if (user == null) throw new ApiException("User Not Found!", (int)HttpStatusCode.Gone);

            var userDetails = new UserDetailModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                ContactNumber = user.PhoneNumber,
                UserType = user.UserType,
                CompanyName = user.Remarks,
                UserImage = user.ProfileImage,
                Designation = user.Designation

                //for binding email notification settings at the UI
                //IsReceiveDataAnomEmails = user.IsReceiveDataAnomEmails,
                //IsReceiveCreepRemaningEmails = user.IsReceiveCreepRemaningEmails,
                //IsReceiveIowEmails = user.IsReceiveIowEmails
            };

            return userDetails;
        }
    }
}