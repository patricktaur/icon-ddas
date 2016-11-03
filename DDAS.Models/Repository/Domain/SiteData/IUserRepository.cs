using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDAS.Models.Repository.Domain.SiteData
{
    public interface IUserRepository : IRepository<User>
    {
        Task AddAsync(User entity);
        Task UpdateUserAsync(string UserName);
        User FindByUserName(string UserName);
        Task<User> FindByUserNameAsync(string UserName);
        Task<User> FindByUserNameAsync(CancellationToken cancellationToken,
            string UserName);
        //Patrick 30Oct2016
        Task UpdateUser(User entity);
    }
}
