using DDAS.Models.Entities;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace DDAS.Data.Mongo.Maps
{
    public static class MongoMaps //MongoConfigurationMap
    {
        public static void Initialize()
        {
            BsonClassMap.RegisterClassMap<SiteDataItemBase>(map =>
            {
                map.AutoMap();
                //Patrick 08Oct2016:
                map.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ComplianceForm>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
  
                BsonClassMap.RegisterClassMap<InvestigatorSearched>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
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
    

            });

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

            BsonClassMap.RegisterClassMap<ClinicalInvestigatorInspectionSiteData>
                (map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<ClinicalInvestigator>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<FDAWarningLettersSiteData>
                (map =>
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
            });

            BsonClassMap.RegisterClassMap<ClinicalInvestigatorDisqualificationSiteData>
                (map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ERRProposalToDebarPageSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<CBERClinicalInvestigatorInspectionSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
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

            BsonClassMap.RegisterClassMap<CorporateIntegrityAgreementListSiteData>(map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SystemForAwardManagementPageSiteData>
            (map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SpeciallyDesignatedNationalsListSiteData>
                (map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SaveSearchResult>
                (map =>
                {
                    map.AutoMap();
                    map.SetIgnoreExtraElements(true);
                    map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                });

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
            #endregion
        }

    }
}

