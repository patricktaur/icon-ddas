using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using DDAS.Models.Entities.Domain;
using DDAS.Models;

namespace DDAS.API.Identity
{
    public class RoleStore:IRoleStore<IdentityRole,Guid>
    {
        private IUnitOfWork _UOW;
        public RoleStore(IUnitOfWork uow)
        {
            _UOW = uow;
        }
        public Task CreateAsync(IdentityRole role)
        {
            if (role == null)
                throw new ArgumentNullException("Role");

            var u = getRole(role);

            return _UOW.RoleRepository.AddAsync(u);

            //IdentityContext context = new IdentityContext();
            //context.Roles.Add(addRole);
            //return context.SaveChangesAsync();

        }

        public Task DeleteAsync(IdentityRole role)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityRole> FindByIdAsync(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityRole> FindByNameAsync(string roleName)
        {
            var entity = _UOW.RoleRepository.FindByName(roleName);
            return Task.FromResult<IdentityRole>(getIdentityRole((entity)));
        }

        public Task UpdateAsync(IdentityRole role)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #region Private Methods
        private Role getRole(IdentityRole identityRole)
        {
            if (identityRole == null)
                return null;
            return new Role
            {
                RoleId = identityRole.Id,
                Name = identityRole.Name
            };
        }

        private IdentityRole getIdentityRole(Role role)
        {
            if (role == null)
                return null;
            return new IdentityRole
            {
                Id = role.RoleId,
                Name = role.Name
            };
        }
        #endregion
    }
}
