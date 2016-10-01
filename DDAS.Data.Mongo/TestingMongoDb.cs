using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo
{
    class TestingMongoDb
    {
        public TestingMongoDb()
        {
            BsonClassMap.RegisterClassMap<TestMongo>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(u => u.RecId);
            });
        }
    }
}
