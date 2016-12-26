using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task AddAsync(UserRole userRole);
        IList<Guid> GetRoleId(User user);
        IList<string> GetRoleValues(User user);
        IList<string> GetRoleValues(Guid UserId);
        IList<UserRole> GetUserRoles(Guid UserId);
        UserRole GetUserRole(Guid UserId, Guid RoleId);
        bool DropUserRole(Guid UserRoleId);

    }
}
