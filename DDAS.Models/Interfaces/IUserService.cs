﻿using DDAS.Models.Entities.Domain;
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
        List<UserViewModel> GetAdmins();
        List<UserViewModel> GetAppAdmins();
        UserViewModel SaveUser(UserViewModel user);
        bool DeleteUser(Guid UserId);

        bool AddLoginDetails(
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
            string Https);

        List<LoginDetails> GetAllLoginHistory();
    }
}
