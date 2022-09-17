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
using LGED.Model.Entities.Profile;

namespace LGED.Domain.Handlers.Admin.UserProfile
{
    public class CreateUpdateRoleHandler : HandlerBase<CreateUpdateRoleCommand, bool>
    {
        public CreateUpdateRoleHandler(IUnitOfWork unitOfWork, IUserContext context) : base(unitOfWork, context)
        {
        }
        public override async Task<bool> Handle(CreateUpdateRoleCommand command, CancellationToken cancellationToken)
        {
            var e = new Role
            {
                Id = command.Id,
                Name = command.Name,
                Description = command.Discription,
                NormalizedName = command.Name.Trim().ToLower(),
                Code = command.Code

            };

            var roleInDb = _unitOfWork.RoleRepository.GetById(command.Id);

            if (roleInDb == null) { _unitOfWork.RoleRepository.Add(_context, e); }
            else
            {
                if (roleInDb.Name == command.Name)
                {
                    throw new ApiException($"The Role '{command.Name}' already exist on system ", (int)HttpStatusCode.Conflict);
                }
                _unitOfWork.RoleRepository.Update(_context, e);

            }

            return await _unitOfWork.CommitAsync();
        }
    }
}