using DDAS.Models.Entities.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task AddAsync(Role entity);
        Role FindByName(string RoleName);
        Task<Role> FindByNameAsync(string RoleName);
        Task<Role> FindByNameAsync(CancellationToken cancellationToken,
            string RoleName);
    }
}
