using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.Interfaces
{
    public interface IUserService
    {
        UserViewModel GetNewUser();
        UserViewModel GetUser(Guid? UserId);
        List<UserViewModel> GetUsers();
        UserViewModel SaveUser(UserViewModel user);
        bool DeleteUser(Guid UserId);

    }
}
