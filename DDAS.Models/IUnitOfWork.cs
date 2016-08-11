using DDAS.Models.Repository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DDAS.Models
{
    public interface IUnitOfWork : IDisposable
    {
        #region Properties

            IArtistRepository ArtistRepository { get; }

        #endregion

        #region Methods

            int SaveChanges();
            Task<int> SaveChangesAsync();
            Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        #endregion
    }
}
