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
        IList<string> GetRoles(User user);
        IList<string> GetRoles(Guid UserId);

    }
}
