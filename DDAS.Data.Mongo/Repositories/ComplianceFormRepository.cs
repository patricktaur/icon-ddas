using System;
using System.Threading.Tasks;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System.Collections.Generic;
using DDAS.Models.Enums;

namespace DDAS.Data.Mongo.Repositories
{
    internal class ComplianceFormRepository : Repository<ComplianceForm>,
        IComplianceFormRepository
    {
        private IMongoDatabase _db;
        public ComplianceFormRepository(IMongoDatabase db) : base(db)
        {
            _db = db;
        }

        public ComplianceForm FindComplianceFormIdByNameToSearch(string NameToSearch)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("NameToSearch", NameToSearch);
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(filter).FirstOrDefault();
            return entity;
        }

        public List<ComplianceForm> FindActiveComplianceForms(bool value)
        {
            var Filter = Builders<ComplianceForm>.Filter.Eq("Active", value);
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(Filter).ToList();
            return entity;
        }

        public Task UpdateCollection(ComplianceForm form)
        {
            return _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name).
                ReplaceOneAsync(CompForm => CompForm.RecId == form.RecId, form);
        }

        public bool DropComplianceForm(object ComplianceFormId)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", ComplianceFormId);
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.DeleteOne(filter);
            return true;
        }

        public bool UpdateAssignedTo(Guid id, string AssignedTo)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var update = Builders<ComplianceForm>.Update
            .Set("AssignedTo", AssignedTo)
            .CurrentDate("UpdatedOn");
            var result =  collection.UpdateOne(filter, update);
            if (result.IsAcknowledged && result.ModifiedCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
         }

        public bool UpdateComplianceForm(Guid id, ComplianceForm form)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var update = Builders<ComplianceForm>.Update
            .Set("ProjectNumber", form.ProjectNumber)
             .Set("SponsorProtocolNumber", form.SponsorProtocolNumber)
              .Set("Institute", form.Institute)
               .Set("Address", form.Address)
                .Set("Country", form.Country)
                .Set("InvestigatorDetails", form.InvestigatorDetails)
                .Set("SiteSources", form.SiteSources)
            .CurrentDate("UpdatedOn")
            ;

            var result = collection.UpdateOne(filter, update);
            if (result.IsAcknowledged && result.ModifiedCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       
        public bool UpdateFindings(Guid id,  SiteEnum siteEnum, int InvestigatorId, List<Finding> Findings)
        {
            //Require not in query to delere items not found in incoming collection.


            var builder = Builders<ComplianceForm>.Filter;
            var filter = builder.Eq("RecId", id) & builder.Eq("Findings.SiteEnum", siteEnum) & builder.Eq("Findings.InvestigatorSearchedId", InvestigatorId);
            var update = Builders<ComplianceForm>.Update.AddToSet("Findings", Findings);
            //var update1 = Builders<ComplianceForm>.Update.PullFilter("Findings", Builders<Finding>.Filter.Not();

            //        var update = Builders<Person>.Update.PullFilter("followerList",
            //Builders<Follower>.Filter.Eq("follower", "fethiye"));

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var result = collection.UpdateOne(filter, update);
            if (result.IsAcknowledged && result.ModifiedCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateExtractionEstimatedCompletion(Guid id, DateTime dateValue)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var update = Builders<ComplianceForm>.Update
            .Set("ExtractionEstimatedCompletion", dateValue)
            .CurrentDate("UpdatedOn");
            var result = collection.UpdateOne(filter, update);
            if (result.IsAcknowledged && result.ModifiedCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }

    }
}
