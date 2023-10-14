using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository.Domain.SiteData;
//using Norm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DDAS.Models.Repository;
using System.Threading;
using MongoDB.Driver;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class ERRProposalToDebarRepository : Repository<ERRProposalToDebarPageSiteData>,
        IERRProposalToDebarRepository
    {
        private IMongoDatabase _db;
        internal ERRProposalToDebarRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public ERRProposalToDebarPageSiteData GetLatestDocument()
        {
            var collection = _db.GetCollection<ERRProposalToDebarPageSiteData>(typeof(ERRProposalToDebarPageSiteData).Name);
            var entity = collection.Find(x => x.DataExtractionSucceeded == true).SortByDescending(y => y.CreatedOn).FirstOrDefault();
            return entity;
        }
    }
}
