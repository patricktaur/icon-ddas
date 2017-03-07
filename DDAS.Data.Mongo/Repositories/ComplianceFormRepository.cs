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

        public bool UpdateReviewCompleted(Guid formId, int investigatorId, SiteEnum siteEnum, bool ReviewCompleted)
        {
            var form = FindById(formId);
            if (form != null)
            {

            }
            return true;
        }



        public bool UpdateInvestigator(Guid formId, InvestigatorSearched Investigator)
        {


            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
           
            

            var filter = Builders<ComplianceForm>.Filter.Where(x => x.RecId == formId);
            //var inv = new List<InvestigatorSearched>();
            //inv.Add(Investigator);
            var update = Builders<ComplianceForm>.Update.AddToSet(x => x.InvestigatorDetails, Investigator);
            //var update1 = Builders<ComplianceForm>.Update.Set(x => x.InvestigatorDetails, inv);
            var result = collection.UpdateOneAsync(filter, update).Result;
            if (result.IsAcknowledged && result.ModifiedCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        public bool UpdateFindings(UpdateFindigs updateFindings)
        {

            //Separate update operations, a single update is preferred.

            //Clear all IsMatchedRecord == false Findigs 
            //IsMatchedRecord cannot be deleted as it contains Record Details derived from Extraction Process.
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);

            //var filter = Builders<ComplianceForm>.Filter.Where(x => x.RecId == updateFindings.FormId);
            //var update = Builders<ComplianceForm>.Update.PullFilter(x => x.Findings,
            //    f => f.SiteEnum == updateFindings.SiteEnum
            //    && f.InvestigatorSearchedId == updateFindings.InvestigatorSearchedId
            //    && f.IsMatchedRecord == false
            //    );
            //var result = collection.UpdateOneAsync(filter, update).Result;

            var filter1 = Builders<ComplianceForm>.Filter.Where(x => x.RecId == updateFindings.FormId);
            var update1 = Builders<ComplianceForm>.Update.AddToSetEach(x => x.Findings, updateFindings.Findings);
            //CarRentalContext.Cars.Update(Query.EQ("_id", ObjectId.Parse(updateCarViewModel.Id)), Update.Replace(modifiedCar), UpdateFlags.Upsert);
            //collection.UpdateOne(filter1, update1)
            var result1 = collection.UpdateOneAsync(filter1, update1).Result;


            return true;


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

        public bool UpdateExtractionQueStart(Guid id, DateTime? dateValue)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var update = Builders<ComplianceForm>.Update
            .Set("ExtractionQueStart", dateValue)
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
