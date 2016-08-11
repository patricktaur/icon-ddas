using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities;

namespace DDAS.EF
{
    internal class ApplicationIdentityDBContext : DbContext
    {
        internal ApplicationIdentityDBContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public ApplicationIdentityDBContext()
            : base("Name=DefaultConnection")
        {
        }

        internal IDbSet<Param> Params { get; set; }
        internal IDbSet<Artist> Artists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
        }
    }
}
