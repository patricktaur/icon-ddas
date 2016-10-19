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
                map.SetIgnoreExtraElements(true);
                map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                BsonClassMap.RegisterClassMap<PHSAdministrativeAction>(child =>
                {
                    child.AutoMap();
                    child.SetIgnoreExtraElements(true);
                });
            });

            BsonClassMap.RegisterClassMap<SaveSearchResult>
                (map =>
                {
                    map.AutoMap();
                    map.SetIgnoreExtraElements(true);
                    map.MapIdProperty(u => u.RecId).SetIdGenerator(GuidGenerator.Instance);
                });
        }

    }
}

