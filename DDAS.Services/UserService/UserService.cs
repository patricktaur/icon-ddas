using DDAS.Models;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Identity;
using DDAS.Models.Interfaces;
using DDAS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.EMail;

namespace DDAS.Services.UserService
{
    public class UserService : IUserService
    {
        private IUnitOfWork _UOW;
        
        public UserService(IUnitOfWork uow)
        {
            _UOW = uow;
            
        }

        public bool IsUserAppAdmin(Guid UserId)
        {

            var roles = _UOW.UserRoleRepository.GetRoleValues(UserId); //getAlluserRolesViewModel(UserId);
            
            if (roles.Contains("app-admin"))
            { 
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserViewModel GetNewUser(bool IncludeAppAdminrole)
        {
            var retUser = new UserViewModel();
            retUser.Active = true;
            
            //AddRoles:
            var Roles = _UOW.RoleRepository.GetAll();
            foreach (Role role in Roles)
            {
                if (IncludeAppAdminrole != true && role.Name.ToLower()=="app-admin")
                {
                    //skip adding role
                }
                else
                {
                    var roleToAdd = new RoleViewModel();
                    roleToAdd.Name = role.Name;
                    roleToAdd.Active = false;
                    retUser.Roles.Add(roleToAdd);
                }
                
            }
            return retUser;
        }

    


        public UserViewModel GetUser(Guid? UserId, bool IncludeAppAdminrole)
        {
            var retUserViewMOdel = new UserViewModel();
            var user = _UOW.UserRepository.FindById(UserId);
           
            var userToAdd = new UserViewModel();
            retUserViewMOdel.Active = user.Active;
            retUserViewMOdel.EmailId = user.EmailId;
            retUserViewMOdel.UserFullName = user.UserFullName;
            retUserViewMOdel.UserId = user.UserId;
            retUserViewMOdel.UserName = user.UserName;
            retUserViewMOdel.Roles = getUserRolesViewModel(user.UserId, IncludeAppAdminrole);
            retUserViewMOdel.ActiveRoles = getActiveRolesText(userToAdd.Roles);
            return retUserViewMOdel;
        }

        private List<UserViewModel> GetAllUsers()
        {
            var retUsers = new List<UserViewModel>();
            var Roles = _UOW.RoleRepository.GetAll();
            var Users = _UOW.UserRepository.GetAllUsers();

            foreach (User user in Users)
            {
                var userToAdd = new UserViewModel();
                userToAdd.Active = user.Active;
                userToAdd.EmailId = user.EmailId;
                userToAdd.UserFullName = user.UserFullName;
                userToAdd.UserId = user.UserId;
                userToAdd.UserName = user.UserName;
                userToAdd.Roles =  getAlluserRolesViewModel(user.UserId);
                userToAdd.ActiveRoles = getActiveRolesText(userToAdd.Roles);
                //userToAdd.isAdmin = user.isAdmin;
                //userToAdd.isAppAdmin = user.isAppAdmin;
                retUsers.Add(userToAdd);
            }
            return retUsers;
        }

        public List<UserViewModel> GetUsers()
        {
            //var retUsers = new List<UserViewModel>();

            //foreach (UserViewModel user in GetAllUsers().ToList())
            //{
            //    if (!user.ActiveRoles.ToLower().Contains("app-admin") )
            //    {
            //        retUsers.Add(user);
            //    }
            //  }
            //return retUsers;

            return GetAllUsers().OrderBy(x => x.UserName).ToList();
        }

        public List<UserViewModel> GetAdmins()
        {
            return GetAllUsers().Where(x =>
                  x.ActiveRoles.ToLower().Contains("admin")).ToList();
        }

        public List<UserViewModel> GetAppAdmins()
        {
            return GetAllUsers().Where(x =>
                  x.ActiveRoles.ToLower().Contains("app-admin")).ToList();
        }

        public bool SaveUser(UserViewModel userViewModel)
        {
            //convert userViewModel to user:

            User userToUpdate; 
            if (userViewModel.UserId == null)
            {
                userToUpdate = new User();
                userToUpdate.UserId = Guid.NewGuid();
                userToUpdate.UserName = userViewModel.UserName.Trim();
                userToUpdate.Active = userViewModel.Active;
                userToUpdate.EmailId = userViewModel.EmailId;
                _UOW.UserRepository.Add(userToUpdate);
            }
            else
            {
                 userToUpdate = _UOW.UserRepository.FindById(userViewModel.UserId);
                if (userToUpdate == null)
                {
                    throw new Exception("User: " + userToUpdate.UserName + " could not be updated");
                }
                userToUpdate.Active = userViewModel.Active;
                userToUpdate.EmailId = userViewModel.EmailId;
                _UOW.UserRepository.UpdateUser(userToUpdate);

            }

            //Update Roles.
            var activeUserRolesValues = _UOW.UserRoleRepository.GetRoleValues(userToUpdate.UserId);
            var activeUserRoles = _UOW.UserRoleRepository.GetUserRoles(userToUpdate.UserId);

            foreach (RoleViewModel roleViewModel in userViewModel.Roles)
            {
               

                if (roleViewModel.Active == true)
                {
                    

                    if (!activeUserRolesValues.Contains(roleViewModel.Name))
                    {
                        //Add
                        var userRole = new UserRole();
                        Role role = _UOW.RoleRepository.FindByName(roleViewModel.Name);
                        userRole.RoleId = role.RoleId;
                        userRole.UserId = userToUpdate.UserId;
                        _UOW.UserRoleRepository.Add(userRole);
                    }
                }
                if (roleViewModel.Active == false)
                {
                    
                    if (activeUserRolesValues.Contains(roleViewModel.Name))
                    {
                        //Delete
                        var roleToDelete = _UOW.RoleRepository.FindByName(roleViewModel.Name);
                        var userRole = _UOW.UserRoleRepository.GetUserRole(userToUpdate.UserId, roleToDelete.RoleId);
                        if (userRole != null)
                        {
                            _UOW.UserRoleRepository.DropUserRole(userRole.Id);
                        }
                    }
                }
            }
            return true;
            //return GetUser(userToUpdate.UserId, false); //return userViewModel with roles.
        }

        public bool DeleteUser(Guid UserId)
        {
            //Delete UserRoles.
            var userRoles = _UOW.UserRoleRepository.GetUserRoles(UserId);
            foreach (UserRole userRole in userRoles)
            {
                var success = _UOW.UserRoleRepository.DropUserRole(userRole.Id);
                //In the absence of Transactions, abort if this operation fails:
                if (!success) return false;
            }

            _UOW.UserRepository.DropUser(UserId);

            return true;
        }

        public bool SetPassword(SetPasswordBindingModel model)
        {


            return true;
        }

        #region Helpers

        
        private List<RoleViewModel> getUserRolesViewModel(Guid UserId, bool IncludeAppAdminRole)
        {
          if (IncludeAppAdminRole == true)
            {
                return getAlluserRolesViewModel(UserId).ToList();
            }
            else
            {
                return getAlluserRolesViewModel(UserId).Where(x => x.Name != "app-admin").ToList();
            }
            
            // 
           
        }

        private List<RoleViewModel> getAlluserRolesViewModel(Guid UserId)
        {
            //exclude app-admin
            var retRolesViewModel = new List<RoleViewModel>();
            var Roles = _UOW.RoleRepository.GetAll();
            var activeRoles = _UOW.UserRoleRepository.GetRoleValues(UserId);
            
            foreach (Role role in Roles)
            {
                    var roleToAdd = new RoleViewModel();
                    roleToAdd.Name = role.Name;
                    if (activeRoles.Contains(role.Name))
                    {
                        roleToAdd.Active = true;
                    }

                    retRolesViewModel.Add(roleToAdd);
               
                
            }
            return retRolesViewModel;
          }

        private string getActiveRolesText(List<RoleViewModel> roles)
        {
            var activeRolesText = "";
            var comma = "";
            foreach (RoleViewModel role in roles)
            {
                if (role.Active == true)
                {
                    activeRolesText += comma + role.Name;
                    comma = ", ";
                }
             }
            return activeRolesText;
        }


        #endregion

        #region Login Details

        public bool AddLoginDetails(
            string UserName,
            string LocalIPAddress,
            string HostIPAddress,
            string PortNumber,
            bool IsLoginSuccessful,
            string ServerProtocol,
            string ServerSoftware,
            string HttpHost,
            string ServerName,
            string GatewayInterface,
            string Https)
        {
            var loginDetails = new LoginDetails();

            loginDetails.UserName = UserName;
            loginDetails.LoginAttemptTime = DateTime.Now;
            loginDetails.LocalIPAddress = LocalIPAddress;
            loginDetails.HostIPAddress = HostIPAddress;
            loginDetails.PortNumber = PortNumber;
            loginDetails.IsLoginSuccessful = IsLoginSuccessful;
            loginDetails.ServerProtocol = ServerProtocol;
            loginDetails.ServerName = ServerName;
            loginDetails.ServerSoftware = ServerSoftware;
            loginDetails.HttpHost = HttpHost;
            loginDetails.Https = Https;
            loginDetails.GatewayInterface = GatewayInterface;

            _UOW.LoginDetailsRepository.Add(loginDetails);

            return true;
        }

        public List<LoginDetails> GetAllLoginHistory()
        {
            var AllLoginHistory = _UOW.LoginDetailsRepository.GetAll();
            return AllLoginHistory;
        }


        #endregion

        #region Helpers
        //public static bool ContainsAny(this string haystack, params string[] needles)
        //{
        //    //Usage: bool anyLuck = s.ContainsAny("a", "b", "c");
        //    foreach (string needle in needles)
        //    {
        //        if (haystack.Contains(needle))
        //            return true;
        //    }

        //    return false;
        //}

        #endregion

    }
}
