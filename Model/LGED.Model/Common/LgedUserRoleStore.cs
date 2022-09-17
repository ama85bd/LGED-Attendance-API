using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LGED.Model.Context;
using LGED.Model.Entities.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LGED.Model.Common
{
    public class LgedUserRoleStore : UserStore<User, Role, LgedDbContext, Guid, IdentityUserClaim<Guid>,
        UserRoles, IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>
    {
        private readonly LgedDbContext _dbContext;
        public LgedUserRoleStore(LgedDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            _dbContext = context;
        }

         /// <summary>
        /// Adds the given <paramref name="normalizedRoleName"/> to the specified <paramref name="user"/>.
        /// The added user-role will have ComponentId based on UserContext
        /// </summary>
        /// <param name="user">The user to add the role to.</param>
        /// <param name="normalizedRoleName">The role to add.</param>
        /// <param name="companyId">The company's id along with the role.</param>
        /// <param name="parentUserId">The Id of the user who have permission to add user to role</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task AddToRoleAsync(User user, string normalizedRoleName, Guid companyId, Guid parentUserId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }

            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);

            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Role {0} does not exist.", normalizedRoleName));
            }

            var currentDt = DateTime.Now;
            var currentUid = parentUserId;
            var ur = new UserRoles()
            {
                UserId = user.Id,
                RoleId = roleEntity.Id,
                CompanyId = companyId,
                InsertedAt = currentDt,
                InsertedBy = currentUid,
                UpdatedAt = currentDt,
                UpdatedBy = currentUid
            };

            _dbContext.UserRoles.Add(ur);
            _dbContext.SaveChanges();
        }

         //region Customize FindRoleAsync with filtering IsDeleted

        protected override Task<Role> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return _dbContext.Roles.SingleOrDefaultAsync(
                r => r.NormalizedName == normalizedRoleName && r.IsDeleted == false, cancellationToken);
        }

         //region Customize GetUsersInRoleAsync with CompanyId

        /// <summary>
        /// Search list of user by role name and company's id
        /// </summary>
        /// <param name="normalizedRoleName"></param>
        /// <param name="companyId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IList<User>> GetUsersInRoleAsync(string normalizedRoleName, Guid companyId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);

            if (role != null)
            {
                var query = from ur in _dbContext.UserRoles
                            join user in _dbContext.Users on ur.UserId equals user.Id
                            join c in _dbContext.Company on ur.CompanyId equals c.Id
                            where ur.RoleId.Equals(role.Id) && c.IsDeleted == false && ur.CompanyId.Equals(companyId)
                            select user;

                return await query.ToListAsync(cancellationToken);
            }

            return new List<User>();
        }

        //region Customize GetRoleAsync with CompanyId

        public async Task<IList<string>> GetRolesAsync(User user, Guid companyId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userId = user.Id;
            var query = from ur in _dbContext.UserRoles
                        join r in _dbContext.Roles on ur.RoleId equals r.Id
                        where ur.UserId.Equals(userId) && r.IsDeleted == false
                        select new { r.Name, ur.CompanyId };

            //filter by company but get all for admin
            if (companyId != Guid.Empty)
            {
                query = query.Where(c =>
                    c.CompanyId.Equals(companyId));
            }

            var names = query.Select(c => c.Name).Distinct();

            return await names.ToListAsync(cancellationToken);
        }


        //region Get list of RoleId (Guid)

        public async Task<IList<Guid>> GetRoleIdsAsync(User user, Guid companyId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userId = user.Id;

            var query = from ur in _dbContext.UserRoles
                        join r in _dbContext.Roles on ur.RoleId equals r.Id
                        where ur.UserId.Equals(userId) && r.IsDeleted == false
                        select new { r.Id, ur.CompanyId };

            if (companyId != Guid.Empty)
            {
                query = query.Where(c => c.CompanyId.Equals(companyId));
            }

            var ids = query.Select(c => c.Id).Distinct();

            return await ids.ToListAsync(cancellationToken);
        }


        //region Customize FindUserRoleAsync with combination of UserId, RoleId and CompanyId

        /// <summary>
        /// Return a user role for the userId, roleId and companyId if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="roleId">The role's id.</param>
        /// <param name="companyId">The company's id</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user role if it exists.</returns>
        public async Task<UserRoles> FindUserRoleAsync(Guid userId, Guid roleId, Guid companyId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.UserRoles.FindAsync(new object[] { userId, roleId, companyId }, cancellationToken)
                .AsTask();
        }


          //region Customize IsInRoleAsync with CompanyId

        public async Task<bool> IsInRoleAsync(User user, string normalizedRoleName, Guid companyId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, role.Id, companyId, cancellationToken);
                return userRole != null;
            }

            return false;
        }


         //region Customize RemoveFromRoleAsync with CompanyId

        public async Task RemoveFromRoleAsync(User user, string normalizedRoleName, Guid companyId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }

            var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (roleEntity != null)
            {
                var userRole = await FindUserRoleAsync(user.Id, roleEntity.Id, companyId, cancellationToken);
                if (userRole != null)
                {
                    _dbContext.UserRoles.Remove(userRole);
                }
            }
        }

    }
}