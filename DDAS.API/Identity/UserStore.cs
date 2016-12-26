using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using DDAS.Models.Entities.Domain;
using DDAS.Models;
//using Entities = DDAS.Models.Entities.Domain;

namespace DDAS.API.Identity
{
    public class UserStore : IUserStore<IdentityUser, Guid>,IDisposable,IUserPasswordStore<IdentityUser, Guid>,IUserRoleStore<IdentityUser, Guid>
    {
        private IUnitOfWork _UOW;

        public UserStore(IUnitOfWork uow)
        {
            _UOW = uow;
        }

        public Task CreateAsync(IdentityUser user)
        {

            if (user == null)
                throw new ArgumentNullException("user");

            var u = getUser(user);

            return _UOW.UserRepository.AddAsync(u);
           // return  _UOW.UserRepository.Add(u);
            
            
        }

        public Task DeleteAsync(IdentityUser identityUser)
        {
            throw new NotImplementedException();
            
        }
        public Task UpdateAsync(IdentityUser user)
        {

            return _UOW.UserRepository.UpdateUserAsync(user.UserName);
            


        }
        public Task<IdentityUser> FindByIdAsync(Guid userId)
        {
            
            User entity = _UOW.UserRepository.FindById(userId);
            return Task.FromResult<IdentityUser>(getIdentityUser((entity)));
        }

        public Task<IdentityUser> FindByNameAsync(string userName)
        {
            
            var entity = _UOW.UserRepository.FindByUserName(userName);
            return Task.FromResult<IdentityUser>(getIdentityUser((entity)));
      
        }
        private IdentityUser getIdentityUser(User user)
        {
            if (user == null)
                return null;

            var identityUser = new IdentityUser();
            identityUser.Id = user.UserId;
            identityUser.UserName = user.UserName;
            identityUser.PasswordHash = user.PasswordHash;
            identityUser.SecurityStamp = user.SecurityStamp;
             return identityUser;
        }

       #region Password
        public Task<string> GetPasswordHashAsync(IdentityUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<string>(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            return Task.FromResult<bool>(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        //Patrick -- review code:
        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash)
        {

            User updateUser = _UOW.UserRepository.FindById(user.Id);
            if (updateUser != null)
            {
                updateUser.PasswordHash = passwordHash;
                _UOW.UserRepository.UpdateUser(updateUser);
            }

            return Task.FromResult(0);
        }
       #endregion


        public void Dispose()
        {
            // Dispose does nothing since we want Unity to manage the lifecycle of our Unit of Work
        }
        #region IUserRoleStore<IdentityUser, Guid> Members
        public Task AddToRoleAsync(IdentityUser user, string roleName)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("Argument cannot be null, empty, or whitespace: roleName.");

            var u = _UOW.UserRepository.FindById(user.Id);
            if (u == null)
                throw new ArgumentException("IdentityUser does not correspond to a User entity.", "user");
            var r  = _UOW.RoleRepository.FindByName(roleName);
            if (r == null)
                throw new ArgumentException("roleName does not correspond to a Role entity.", "roleName");

            u.Roles.Add(r);
            UserRole userRole = new UserRole();
            userRole.UserId = u.UserId;
            userRole.RoleId = r.RoleId;

            return _UOW.UserRoleRepository.AddAsync(userRole);
        }

        public Task<IList<string>> GetRolesAsync(IdentityUser user)
        {

            return Task.FromResult < IList < string >>(_UOW.UserRoleRepository.GetRoleValues(user.Id));
            /*

            IList<string> rolenames = new List<string>();
            rolenames.Add("Admin");
            //IdentityContext Set = new IdentityContext();
            //var u = Set.Users.FirstOrDefault(x => x.UserId == user.Id);
            //return Task.FromResult<IList<string>>(u.Roles.Select(x => x.Name).ToList());

            //MongoClient conn = new MongoClient("mongodb://localhost:27017");
            //var db = conn.GetDatabase("DDAS");
            //var filter = Builders<User>.Filter.Eq("UserId", user.Id);
            //var collection = db.GetCollection<User>("User");
            var entity = _UOW.UserRepository.FindById(user.Id);
            return Task.FromResult<IList<string>>(rolenames);

            //throw new NotImplementedException();
            */
        }

        public Task<bool> IsInRoleAsync(IdentityUser user, string roleName)
        {
             var roles = GetRolesAsync(user).Result;
            if (roles.Contains(roleName) == true)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }

        }

        public Task RemoveFromRoleAsync(IdentityUser user, string roleName)
        {
            //IdentityContext Set = new IdentityContext();
            //var u = Set.Users.FirstOrDefault(x => x.UserId == user.Id);
            //return Set.SaveChangesAsync();

            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods
        private User getUser(IdentityUser identityUser)
        {
            if (identityUser == null)
                return null;

            var user = new User();
            populateUser(user, identityUser);

            return user;
        }

        private void populateUser(User user, IdentityUser identityUser)
        {
            user.UserId = identityUser.Id;
            user.UserName = identityUser.UserName;
            user.PasswordHash = identityUser.PasswordHash;
            user.SecurityStamp = identityUser.SecurityStamp;
        }

       

        private void populateIdentityUser(IdentityUser identityUser, User user)
        {
            identityUser.Id = user.UserId;
            identityUser.UserName = user.UserName;
            identityUser.PasswordHash = user.PasswordHash;
            identityUser.SecurityStamp = user.SecurityStamp;
        }
        #endregion

    }


}

