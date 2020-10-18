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

        //Not used:
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


        public List<ComplianceForm> FindComplianceForms(string AssignedTo, ReviewStatusEnum ReviewStatus)
        {
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(
                x => x.AssignedTo == AssignedTo
                && x.Reviews.Any(y => y.Status == ReviewStatus)).ToList();

            return entity;
        }

        public List<ComplianceForm> FindComplianceForms(string AssignedTo)
        {
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(
                x => x.AssignedTo == AssignedTo
                ).ToList();

            return entity;
        }

        public List<ComplianceForm> FindComplianceForms( ReviewStatusEnum ReviewStatus)
        {
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(
                 x => x.Reviews.Any(y => y.Status == ReviewStatus)).ToList();
            return entity;
        }

        public List<ComplianceForm> FindComplianceForms(ComplianceFormFilter CompFormFilter)
        {

            var builder = Builders<ComplianceForm>.Filter;
            var filter = builder.Empty;
            //-------------------


            if (CompFormFilter.InvestigatorName != null &&
                CompFormFilter.InvestigatorName != "")
            {
                //var filter = Builders<ComplianceForm>.Filter
                //   .Where(x => x.InvestigatorDetails.Any(y => y.Name.ToLower().Contains(CompFormFilter.InvestigatorName.ToLower())));

                filter = filter & builder.Where(x => x.InvestigatorDetails.Any(y => y.Name.ToLower().Contains(CompFormFilter.InvestigatorName.ToLower())));
                //Filter1 = Filter1.FindAll(x =>
                //x.InvestigatorDetails.FirstOrDefault().Name.ToLower()
                //.Contains(
                //    CompFormFilter.InvestigatorName.ToLower()));
            }



            if (CompFormFilter.ProjectNumber != null &&
                CompFormFilter.ProjectNumber != "")
            {
                filter = filter & builder.Where(
                    x => x.ProjectNumber == CompFormFilter.ProjectNumber
                    || x.ProjectNumber2 == CompFormFilter.ProjectNumber);

                //Filter2 = Filter1.Where(x =>
                //x.ProjectNumber == CompFormFilter.ProjectNumber ||
                //x.ProjectNumber2 == CompFormFilter.ProjectNumber)
                //.ToList();
            }


            if (CompFormFilter.SponsorProtocolNumber != null &&
                CompFormFilter.SponsorProtocolNumber != "")
            {
                filter = filter & builder.Where(
                    x => x.SponsorProtocolNumber.ToLower() == CompFormFilter.SponsorProtocolNumber.ToLower()
                    || x.SponsorProtocolNumber2.ToLower() == CompFormFilter.SponsorProtocolNumber.ToLower());

                //Filter3 = Filter2.Where(x =>
                //x.SponsorProtocolNumber.ToLower() ==
                //CompFormFilter.SponsorProtocolNumber.ToLower() ||
                //x.SponsorProtocolNumber2.ToLower() ==
                //CompFormFilter.SponsorProtocolNumber.ToLower())
                //.ToList();
            }


            if (CompFormFilter.SearchedOnFrom != null)
            {
                DateTime startDate;
                startDate = CompFormFilter.SearchedOnFrom.Value.Date;

                filter = filter & builder.Where(
                    x => x.SearchStartedOn >= startDate);

                // Filter4 = Filter3.Where(x =>
                //x.SearchStartedOn >= startDate)
                //.ToList();
            }


            if (CompFormFilter.SearchedOnTo != null)
            {
                DateTime endDate;
                endDate = CompFormFilter.SearchedOnTo.Value.Date.AddDays(1);

                filter = filter & builder.Where(x => x.SearchStartedOn < endDate);

                //Filter5 = Filter4.Where(x =>
                //x.SearchStartedOn <
                //endDate)
                //.ToList();
            }

            



            if (CompFormFilter.Country != null &&
                CompFormFilter.Country != "")
            {
                filter = filter & builder.Where(x => x.Country.ToLower() == CompFormFilter.Country.ToLower());
                //Filter6 = Filter5.Where(x =>
                //x.Country.ToLower() == CompFormFilter.Country.ToLower()).ToList();
            }


            //if ((int)CompFormFilter.Status == -1)
            //{

            //    filter = filter & builder.Where(
            //        x => x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
            //    x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified);

            //    //Filter7 = Filter6.FindAll(x => x.StatusEnum ==
            //    //ComplianceFormStatusEnum.ReviewCompletedIssuesIdentified ||
            //    //x.StatusEnum == ComplianceFormStatusEnum.ReviewCompletedIssuesNotIdentified)
            //    //.ToList();
            //}
            //else if ((int)CompFormFilter.Status != -1)
            //{
            //    filter = filter & builder.Where(x => x.StatusEnum == CompFormFilter.Status);

            //    //Filter7 = Filter6.Where(x =>
            //    //x.StatusEnum == CompFormFilter.Status).ToList();
            //}

            if (CompFormFilter.AssignedTo != null &&
                //CompFormFilter.AssignedTo != "" &&
                CompFormFilter.AssignedTo != "-1")
            {
                filter = filter & builder.Where(x => x.AssignedTo.ToLower() == CompFormFilter.AssignedTo.ToLower());
            }

            if (CompFormFilter.InputSource != null && 
                CompFormFilter.InputSource.ToLower() != "all")
            {
                filter = filter & builder.Where(x => x.InputSource.ToLower() == CompFormFilter.InputSource.ToLower());
            }
            //var Filter8 = Filter7.Where(x =>
            //x.AssignedTo == AssignedTo).ToList();
            //--------------------


            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(filter).ToList();

            

            return entity;
        }

        public List<ComplianceForm> FindComplianceFormsBySearchedOn(int searchDaysGreaterThan, int limit)
        {
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var dt = DateTime.Now.AddDays(-searchDaysGreaterThan);
            var entity = collection.Find(
                x => x.SearchStartedOn < dt
                ).Limit(limit).ToList();

            return entity;
        }

        

        public List<ComplianceForm> FindQCComplianceForms(DateTime? ReviewAssignedOn, DateTime? ReviewCompletedOn)
        {
            var builder = Builders<ComplianceForm>.Filter;
            var filter = builder.Empty;

            if (ReviewAssignedOn.HasValue)
            {
                var startDate = ReviewAssignedOn.Value;
                filter = filter & builder.Where(x => x.Reviews.Any(y => y.AssignedOn >= startDate));
            }

            //if (ReviewCompletedOn.HasValue)
            //{
            //    var endDate = ReviewCompletedOn.Value.AddDays(1);
            //    filter = filter & builder.Where(x => x.Reviews.Any(y => y.CompletedOn < endDate));
            //}

            filter = filter & builder.Where(x => x.Reviews.Any(
                y =>
                     y.Status == ReviewStatusEnum.QCRequested
                     || y.Status == ReviewStatusEnum.QCInProgress
                     || y.Status == ReviewStatusEnum.QCFailed
                     || y.Status == ReviewStatusEnum.QCPassed
                     || y.Status == ReviewStatusEnum.QCCompleted
                     || (y.Status == ReviewStatusEnum.QCCorrectionInProgress 
                     && y.CompletedOn < ReviewCompletedOn.Value.AddDays(1))
                     || (y.Status == ReviewStatusEnum.QCCorrectionInProgress
                     && y.CompletedOn < ReviewCompletedOn.Value.AddDays(1))
                     )
                );
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.Find(filter).ToList();
            return entity;
        }

        public Task UpdateCollection(ComplianceForm form)
        {
            return _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name).
                ReplaceOneAsync(CompForm => CompForm.RecId == form.RecId, form);


        }

        //Patrick 21Jan2018:
        //with Concurrency Test
        public Boolean UpdateComplianceForm(ComplianceForm form)
        {
            
            var currentRowVersion = form.RowVersion;
            form.RowVersion = NewRowVersion();

            var result =  _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name).
               ReplaceOneAsync(CompForm => CompForm.RecId == form.RecId && CompForm.RowVersion == currentRowVersion, form);
           
            if (result.Result.ModifiedCount == 1)
            {
                return true;

            }
            else
            {
                return false;
            }

        }

        public bool DropComplianceForm(object ComplianceFormId)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", ComplianceFormId);
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var entity = collection.DeleteOne(filter);
            return true;
        }

        //public bool UpdateAssignedTo(Guid id, string AssignedTo)
        //{
        //    var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);

        //    var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
        //    var update = Builders<ComplianceForm>.Update
        //    .Set("AssignedTo", AssignedTo)
        //    .CurrentDate("UpdatedOn");
        //    var result =  collection.UpdateOne(filter, update);
        //    if (result.IsAcknowledged && result.ModifiedCount == 1)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        // }


        //Has Concurrency check:
        //Pending: Include Update Assignment History :
        public bool UpdateAssignedTo(Guid id, string AssignedBy, string AssignedFrom = "", string AssignedTo = "")
        {            
            //AssignedFrom = current AssignedTo. -- for Concurrency check. If compForm.AssignedTo <> AssignedFrom then throw exception.
            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var compForm = collection.Find(x => x.RecId == id).FirstOrDefault();
            if (compForm != null)
            {
                //AssignedFrom is receied from client as null:
                if (AssignedFrom == null)
                {
                    AssignedFrom = "";
                }

                if (compForm.AssignedTo == null)
                {
                    compForm.AssignedTo = "";
                }


                if ( AssignedFrom != compForm.AssignedTo)
                {
                    throw new Exception(String.Format("Comp Form has been modified by: {0} on {1}", compForm.UpdatedBy, compForm.UpdatedOn) );
                }
                 
                var Reviews = compForm.Reviews;
                var CurrentReview = Reviews.FirstOrDefault();
                
                //CurrentReview.AssigendTo
                if (CurrentReview != null)
                {
                    var CurrentReviewId = CurrentReview.RecId;
                    CurrentReview.AssignedBy = AssignedBy;
                    CurrentReview.AssigendTo = AssignedTo;
                    CurrentReview.AssignedOn = DateTime.Now;

                    var filter = Builders<ComplianceForm>.Filter;
                    var updateFilter = filter.And(
                      filter.Eq(x => x.RecId, id),
                      filter.Eq(x => x.AssignedTo, AssignedFrom)
                      );

                   var update = Builders<ComplianceForm>.Update
                         
                    .Set("AssignedTo", AssignedTo)
                    .Set ("Reviews", Reviews)      
                     .CurrentDate("UpdatedOn");
                    var result = collection.UpdateOne(updateFilter, update);
                    if (result.IsAcknowledged && result.ModifiedCount == 1)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("Update not successful");
                    }
                }
                else
                {
                    throw new Exception("No Review Record Found on Comp Form id = " + id );
                }
            }
            else
            {
                throw new Exception("Comp Form with id = " + id + " Not Found");
            }
        }

        //Not used??
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

        public bool AddFindings(Guid formId, List<Finding> findings)
        {

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var filter = Builders<ComplianceForm>.Filter.Where(x => x.RecId == formId);
            var update = Builders<ComplianceForm>.Update.AddToSetEach(x => x.Findings, findings);
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


        public bool UpdateExtractionEstimatedCompletion(Guid id, DateTime dateValue)
        {
            

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);
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

        public bool UpdateSearchStatus(Guid formId, int InvestigatorId, SiteSearchStatus siteSearchStatus)
        {
            //Not working:
            return false;

            //var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            //var item = collection.Find(x => x.RecId == id).First();

            //foreach (InvestigatorSearched inv in item.InvestigatorDetails)
            //{
            //    foreach (SiteSearchStatus ss in inv.SitesSearched)
            //    {
            //        if (ss.siteEnum == siteSearchStatus.siteEnum)
            //        {
            //            //replace values
            //            ss.DisplayPosition = siteSearchStatus.DisplayPosition;
            //            ss.ExtractedOn = siteSearchStatus.ExtractedOn;
            //            ss.ExtractionErrorMessage = siteSearchStatus.ExtractionErrorMessage;
            //            //ss.ExtractionPending ??
            //            ss.FullMatchCount = siteSearchStatus.FullMatchCount;
            //            //ss.HasExtractionError
            //            //ss.IssuesFound
            //            ss.PartialMatchCount = siteSearchStatus.PartialMatchCount;
            //            ss.ReviewCompleted = siteSearchStatus.ReviewCompleted;
            //            ss.SiteSourceUpdatedOn = siteSearchStatus.SiteSourceUpdatedOn;
            //       }
            //    }
            //}
            

           
            ////Unable to find a solution:
            ////var update = Builders<ComplianceForm>.Update
            ////.Set("ExtractionEstimatedCompletion", dateValue)
            ////.CurrentDate("UpdatedOn");
            ////var result = collection.UpdateOne(filter, update);
            ////if (result.IsAcknowledged && result.ModifiedCount == 1)
            ////{
            ////    return true;
            ////}
            ////else
            ////{
            ////    return false;
            ////}
            
        }

        public bool UpdateExtractionQueStart(Guid id, DateTime? dateValue, int QueueNumber)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var update = Builders<ComplianceForm>.Update
            .Set("ExtractionQueStart", dateValue)
            .Set("ExtractionQueue", QueueNumber)
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

        public bool UpdateExtractionQueEnd(Guid id, DateTime? dateValue)
        {
            var filter = Builders<ComplianceForm>.Filter.Eq("_id", id);

            var collection = _db.GetCollection<ComplianceForm>(typeof(ComplianceForm).Name);
            var update = Builders<ComplianceForm>.Update
            .Set("ExtractedOn", dateValue)
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

        public bool AddAttachmentsToFindings(List<Attachment> Attachments)
        {
            //var filter1 = Builders<ComplianceForm>.Filter.Eq("_id", form.Findings.
            //    Where(x => x.Id == form.RecId));

            var update = Builders<Finding>.Update
            .Set("Attachments.$.Title", "")
            .Set("Attachments.$.FileName", "")
            .Set("Attachments.$.GeneratedFileName", "");
            //.CurrentDate("UpdatedOn");

            return true;
        }


        private static string NewRowVersion()
        {
            return Guid.NewGuid().ToString()
                .Replace("-", "")
                .Replace("{", "")
                .Replace("}", "");
                //.Substring(0, 6);
        }
    }
}
