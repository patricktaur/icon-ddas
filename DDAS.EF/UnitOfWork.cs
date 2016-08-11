using DDAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository.Domain;
using System.Threading;
using DDAS.EF;
using DDAS.EF.Repositories;
using DDAS.EF.Repositories.Domain;
using DDAS.Models.Interfaces;

namespace DDAS.EF
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationIdentityDBContext _context;
        private IArtistRepository _ArtistRepository;

        #region Constructor
        public UnitOfWork(string nameOrConnectionString)
        {
            _context = new ApplicationIdentityDBContext(nameOrConnectionString);
        }

        #endregion

        #region IUnitOfWork Members

        public IArtistRepository ArtistRepository
        {
            get
            {
                return _ArtistRepository ?? (_ArtistRepository = new ArtistRepository(_context));
            }
        }

        #endregion
        
        #region Methods


        public void UpdateAuditFields()
        {
            var modifiedEntries = _context.ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditEntity
                && (x.State == System.Data.Entity.EntityState.Added || x.State == System.Data.Entity.EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                IAuditEntity entity = entry.Entity as IAuditEntity;
                if (entity != null)
                {
                    string identityName = Thread.CurrentPrincipal.Identity.Name;
                    DateTime now = DateTime.UtcNow;

                    if (entry.State == System.Data.Entity.EntityState.Added)
                    {

                        entity.CreatedBy = identityName;
                        entity.CreatedOn = now;
                    }
                    else
                    {
                        _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                        _context.Entry(entity).Property(x => x.CreatedOn).IsModified = false;
                    }

                    entity.UpdatedBy = identityName;
                    entity.UpdatedOn = now;
                }
            }

        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            UpdateAuditFields();
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            UpdateAuditFields();
            return _context.SaveChangesAsync();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        #endregion
    }
}
