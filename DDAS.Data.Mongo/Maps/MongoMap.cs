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

            //BsonClassMap.RegisterClassMap<AdequateAssuranceListSiteData>(map =>
            //{
            //    map.AutoMap();
            //    map.MapIdProperty(u => u.RecId);
            //});

            //BsonClassMap.RegisterClassMap<ERRProposalToDebarPageSiteData>(map =>
            //{
            //    map.AutoMap();
            //    map.MapIdProperty(u => u.RecId);
            //});

            BsonClassMap.RegisterClassMap<PHSAdministrativeActionListingSiteData>(map =>
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
        }

    }
}

