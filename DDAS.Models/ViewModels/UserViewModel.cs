using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Models.ViewModels
{
    public class UserViewModel
    {
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string UserFullName { get; set; }
        public bool Active { get; set; }
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public string ActiveRoles { get; set; }

   
        public bool isAppAdmin
        {
            get
            {
                if (ActiveRoles.ToLower().Contains("app-admin"))
                {
                    return true;
                }

                return false;
            }
        }

        //public bool isAdmin { get; set; }
        //public bool isAppAdmin { get; set; }
 
    }

    public class RoleViewModel
    {
        public bool Active { get; set; }
        public string Name { get; set; }
    }
}
