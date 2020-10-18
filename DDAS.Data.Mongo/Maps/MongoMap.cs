using DDAS.Models.Entities;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Repository;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace DDAS.Data.Mongo.Maps
{
    public static class MongoMaps //MongoConfigurationMap
    {
        public static void Initialize()
        {
            BsonSerializer.RegisterSerializer(
                typeof(DateTime), 
                DateTimeSerializer.LocalInstance
                );

            BsonClassMap.RegisterClassMap<SitesToSearch>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => 
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Country>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SponsorProtocol>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<DefaultSite>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ExceptionLogger>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Log>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Audit>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<AssignmentHistory>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<LogWSDDAS>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<LogWSISPRINT>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u =>
                u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            #region BaseClasses
            BsonClassMap.RegisterClassMap<SiteDataItemBase>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<BaseSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
            });
            #endregion

            #region ComplianceForm
            BsonClassMap.RegisterClassMap<ComplianceForm>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
  
                BsonClassMap.RegisterClassMap<InvestigatorSearched>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);

                    BsonClassMap.RegisterClassMap<SiteSearchStatus>(child1 =>
                    {
                        child1.AutoMap();
                        child1.SetIgnoreExtraElements(true);

                    });

                });

                BsonClassMap.RegisterClassMap<SiteSource>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });

                BsonClassMap.RegisterClassMap<Finding>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });

                BsonClassMap.RegisterClassMap<Review>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                    child.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                });
            });

            BsonClassMap.RegisterClassMap<ComplianceFormArchive>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                //map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                


                

                
            });
            #endregion

            #region SiteDataClasses
            BsonClassMap.RegisterClassMap<FDADebarPageSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);

                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                //Patrick 08Oct2016:
                BsonClassMap.RegisterClassMap<DebarredPerson>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<ClinicalInvestigatorInspectionSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ClinicalInvestigator>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<FDAWarningLettersSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<FDAWarningLetter>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<AdequateAssuranceListSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<AdequateAssuranceList>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });

            });

            BsonClassMap.RegisterClassMap<ClinicalInvestigatorDisqualificationSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<DisqualifiedInvestigator>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<ERRProposalToDebarPageSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<ProposalToDebar>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<CBERClinicalInvestigatorInspectionSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<CBERClinicalInvestigator>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<PHSAdministrativeActionListingSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<PHSAdministrativeAction>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<ExclusionDatabaseSearchPageSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ExclusionDatabaseSearchList>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<CorporateIntegrityAgreementListSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<CIAList>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<SystemForAwardManagementPageSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SystemForAwardManagement>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SAMSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SpeciallyDesignatedNationalsListSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                //BsonClassMap.RegisterClassMap<SDNList>(child =>
                //{
                //    child.AutoMap();
                //    child.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                //    child.SetIgnoreExtraElements(true);
                //});
            });

            BsonClassMap.RegisterClassMap<SDNList>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            #endregion

            #region Identity
            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.UserId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Role>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RoleId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<UserRole>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.Id).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<LoginDetails>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            #endregion
        }

    }
}

