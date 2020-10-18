using System;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;

using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using DDAS.Models.Enums;
using MongoDB.Bson;


namespace DDAS.Data.Mongo.Repositories
{
    internal class ComplianceFormArchiveRepository : Repository<ComplianceFormArchive>,
        IComplianceFormArchiveRepository
    {
        private IMongoDatabase _db;
        public ComplianceFormArchiveRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }


        public List<ComplianceFormArchive> FindComplianceForms(ComplianceFormArchiveFilter CompFormFilter)
        {

            var builder = Builders<ComplianceFormArchive>.Filter;
            var filter = builder.Empty;
            //-------------------


            if (CompFormFilter.InvestigatorName != null &&
                CompFormFilter.InvestigatorName != "")
            {

                //filter = filter & builder.Where(x => x.InvestigatorDetails.Any(y => y.Name.ToLower().Contains(CompFormFilter.InvestigatorName.ToLower())));
                filter = filter & builder.Where(x => x.ComplianceForm.InvestigatorDetails.Any(y => y.Name.ToLower().Contains(CompFormFilter.InvestigatorName.ToLower())));
            }



            if (CompFormFilter.ProjectNumber != null &&
                CompFormFilter.ProjectNumber != "")
            {
                filter = filter & builder.Where(
                    x => x.ProjectNumber == CompFormFilter.ProjectNumber
                    || x.ProjectNumber2 == CompFormFilter.ProjectNumber);

                
            }


            if (CompFormFilter.SponsorProtocolNumber != null &&
                CompFormFilter.SponsorProtocolNumber != "")
            {
                filter = filter & builder.Where(
                    x => x.SponsorProtocolNumber.ToLower() == CompFormFilter.SponsorProtocolNumber.ToLower()
                    || x.SponsorProtocolNumber2.ToLower() == CompFormFilter.SponsorProtocolNumber.ToLower());

                
            }


            if (CompFormFilter.SearchedOnFrom != null)
            {
                DateTime startDate;
                startDate = CompFormFilter.SearchedOnFrom.Value.Date;

                filter = filter & builder.Where(
                    x => x.SearchStartedOn >= startDate);

        
            }


            if (CompFormFilter.SearchedOnTo != null)
            {
                DateTime endDate;
                endDate = CompFormFilter.SearchedOnTo.Value.Date.AddDays(1);

                filter = filter & builder.Where(x => x.SearchStartedOn < endDate);

                
            }

            if (CompFormFilter.ArchivedOnFrom != null)
            {
                DateTime startDate;
                startDate = CompFormFilter.ArchivedOnFrom.Value.Date;

                filter = filter & builder.Where(
                    x => x.ArchivedOn >= startDate);


            }


            if (CompFormFilter.ArchivedOnTo != null)
            {
                DateTime endDate;
                endDate = CompFormFilter.ArchivedOnTo.Value.Date.AddDays(1);

                filter = filter & builder.Where(x => x.ArchivedOn < endDate);


            }






            if (CompFormFilter.Country != null &&
                CompFormFilter.Country != "")
            {
                filter = filter & builder.Where(x => x.ComplianceForm.Country.ToLower() == CompFormFilter.Country.ToLower());
            }

            if (CompFormFilter.AssignedTo != null &&
                
                CompFormFilter.AssignedTo != "-1")
            {
                //filter = filter & builder.Where(x => x.AssignedTo.ToLower() == CompFormFilter.AssignedTo.ToLower());
                filter = filter & builder.Where(x => x.AssignedToFullName.ToLower() == CompFormFilter.AssignedTo.ToLower());
            }

            

            var collection = _db.GetCollection<ComplianceFormArchive>(typeof(ComplianceFormArchive).Name);
            var entity = collection.Find(filter).ToList();



            return entity;
        }

        public bool DropComplianceForm(object ComplianceFormId)
        {
            var filter = Builders<ComplianceFormArchive>.Filter.Eq("_id", ComplianceFormId);
            var collection = _db.GetCollection<ComplianceFormArchive>(typeof(ComplianceFormArchive).Name);
            var entity = collection.DeleteOne(filter);
            return true;
        }

        public ComplianceFormArchive FindByComplianceFormId(string RecId)
        {
            var builder = Builders<ComplianceFormArchive>.Filter;
            var filter = builder.Empty;
            var Id = Guid.Parse(RecId);
            filter = builder.Where(x => x.ComplianceForm.RecId == Id);
            var collection = _db.GetCollection<ComplianceFormArchive>(typeof(ComplianceFormArchive).Name);
            var entity = collection.Find(filter).ToList();

            return entity.FirstOrDefault();
        }
    }
}
