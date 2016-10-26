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
            BsonClassMap.RegisterClassMap<FDADebarPageSiteData>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ClinicalInvestigatorInspectionSiteData>
                (map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<FDAWarningLettersSiteData>
                (map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<AdequateAssuranceListSiteData>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ClinicalInvestigatorDisqualificationSiteData>
                (map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ERRProposalToDebarPageSiteData>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<CBERClinicalInvestigatorInspectionSiteData>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<PHSAdministrativeActionListingSiteData>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<ExclusionDatabaseSearchPageSiteData>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<CorporateIntegrityAgreementListSiteData>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SystemForAwardManagementPageSiteData>
            (map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SpeciallyDesignatedNationalsListSiteData>
                (map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<SaveSearchResult>
                (map =>
                {
                    map.AutoMap();
                    map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                });

            #region Identity
            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.UserId).SetIdGenerator(GuidGenerator.Instance);
            });

            BsonClassMap.RegisterClassMap<Role>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RoleId).SetIdGenerator(GuidGenerator.Instance);
            });
            #endregion
        }

    }
}

