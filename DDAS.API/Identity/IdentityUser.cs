﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace DDAS.API.Identity
{
    public class IdentityUser:IUser<Guid>
    {
        public IdentityUser()
        {
            this.Id = Guid.NewGuid();
        }

        public IdentityUser(string userName)
            : this()
        {
            this.UserName = userName;
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
    }
}
