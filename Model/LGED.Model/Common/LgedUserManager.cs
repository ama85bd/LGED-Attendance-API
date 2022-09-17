using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LGED.Core.Interfaces;
using LGED.Model.Entities.Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LGED.Model.Common
{
    //Required to add scoped LgedUserRoleStore at Startup for overload IUserStore<User>
    /// <summary>
    /// Customized UserManager which enabled to manage user roles with company's id
    /// </summary>
    /// <param name="store"></param>
    /// <param name="optionsAccessor"></param>
    /// <param name="passwordHasher"></param>
    /// <param name="userValidators"></param>
    /// <param name="passwordValidators"></param>
    /// <param name="keyNormalizer"></param>
    /// <param name="errors"></param>
    /// <param name="services"></param>
    /// <param name="logger"></param>
    /// <param name="userContext"></param>
    public class LgedUserManager : UserManager<User>
    {
         private IUserContext _userContext;
        public LgedUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, 
        IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, 
        IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, 
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,IUserContext userContext) 
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            //by default will get from _userContext
            _userContext = userContext;
        }

        /// <summary>
        /// Add the specified <paramref name="user"/> to the named role.
        /// </summary>
        /// <param name="user">The user to add to the named role.</param>
        /// <param name="role">The name of the role to add the user to.</param>
        /// <param name="companyId">The company's id along with the role.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public async Task<IdentityResult> AddToRoleAsync(User user, string role, Guid companyId)
        {
            await ((LgedUserRoleStore)Store).AddToRoleAsync(user, role, companyId, _userContext.UserId,
                CancellationToken);
            return await UpdateUserAsync(user);
        }

        
        /// <summary>
        /// Add the specified <paramref name="user"/> to the named role.
        /// Use the company's id provided by UserContext.
        /// </summary>
        /// <param name="user">The user to add to the named role.</param>
        /// <param name="role">The name of the role to add the user to.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public override async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            var companyId = _userContext.CurrentCompanyId;
            return await AddToRoleAsync(user, role, companyId);
        }

        /// <summary>
        /// Returns a list of users from the user store who are members of the specified <paramref name="roleName"/> and <paramref name="companyId"/>.
        /// </summary>
        /// <param name="roleName">The name of the role whose users should be returned.</param>
        /// <param name="companyId">The company's id along with the role.</param>
        /// <returns>
        /// A list of users who are members of the specified role.
        /// </returns>
        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, Guid companyId)
        {
            return await ((LgedUserRoleStore)Store).GetUsersInRoleAsync(roleName, companyId, CancellationToken);
        }

        
        /// <summary>
        /// Returns a list of users from the user store who are members of the specified <paramref name="roleName"/>.
        /// Use the company's id provided by UserContext.
        /// </summary>
        /// <param name="roleName">The name of the role whose users should be returned.</param>
        /// <returns>
        /// A list of users who are members of the specified role.
        /// </returns>
        public override async Task<IList<User>> GetUsersInRoleAsync(string roleName)
        {
            var companyId = _userContext.CurrentCompanyId;
            return await GetUsersInRoleAsync(roleName, companyId);
        }
        
        /// <summary>
        /// Gets a list of role names the specified <paramref name="user"/> and belongs to.
        /// </summary>
        /// <param name="user">The user whose role names to retrieve.</param>
        /// <param name="companyId">The company's id along with the role.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
        public async Task<IList<string>> GetRolesAsync(User user, Guid companyId)
        {
            return await ((LgedUserRoleStore)Store).GetRolesAsync(user, companyId, CancellationToken);
        }

        
        /// <summary>
        /// Gets a list of role names the specified <paramref name="user"/> belongs to.
        /// Use the company's id provided by UserContext.
        /// </summary>
        /// <param name="user">The user whose role names to retrieve.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
        public override async Task<IList<string>> GetRolesAsync(User user)
        {
            var companyId = _userContext.CurrentCompanyId;
            return await GetRolesAsync(user, companyId);
        }

        
        //only use for login purpose
        public async Task<IList<Guid>> GetRoleIdsAsync(User user)
        {
            var companyId = _userContext.CurrentCompanyId;
            return await ((LgedUserRoleStore)Store).GetRoleIdsAsync(user, companyId, CancellationToken);
        }

        
        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the given named role.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="role">The name of the role to be checked.</param>
        /// <param name="companyId">The company's id along with the role.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user"/> is
        /// a member of the named role.
        /// </returns>
        public async Task<bool> IsInRoleAsync(User user, string role, Guid companyId)
        {
            return await ((LgedUserRoleStore)Store).IsInRoleAsync(user, role, companyId, CancellationToken);
        }

        
        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the given named role.
        /// Use the company's id provided by UserContext.
        /// </summary>
        /// <param name="user">The user whose role membership should be checked.</param>
        /// <param name="role">The name of the role to be checked.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user"/> is
        /// a member of the named role.
        /// </returns>
        public override Task<bool> IsInRoleAsync(User user, string role)
        {
            var companyId = _userContext.CurrentCompanyId;
            return IsInRoleAsync(user, role, companyId);
        }

        
        /// <summary>
        /// Removes the specified <paramref name="user"/> from the named role.
        /// </summary>
        /// <param name="user">The user to remove from the named role.</param>
        /// <param name="role">The name of the role to remove the user from.</param>
        /// <param name="companyId">The company's id along with the role.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role, Guid companyId)
        {
            await ((LgedUserRoleStore)Store).RemoveFromRoleAsync(user, role, companyId, CancellationToken);
            return await UpdateUserAsync(user);
        }

        
        /// <summary>
        /// Removes the specified <paramref name="user"/> from the named role.
        /// Use the company's id provided by UserContext.
        /// </summary>
        /// <param name="user">The user to remove from the named role.</param>
        /// <param name="role">The name of the role to remove the user from.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public override Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
        {
            var companyId = _userContext.CurrentCompanyId;
            return RemoveFromRoleAsync(user, role, companyId);
        }

    }
}