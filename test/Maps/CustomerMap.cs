using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using ShieldTech.MongoDbMigrations.Tests.Entities;

namespace ShieldTech.MongoDbMigrations.Tests.Maps
{
    public class CustomerMap
    {
        public CustomerMap()
        {
            BsonClassMap.RegisterClassMap<Customer>(mapper =>
            {
                mapper.MapIdMember(p => p.Id)
                    .SetElementName("id")
                    .SetIdGenerator(CombGuidGenerator.Instance);
                mapper.MapMember(p => p.Name).SetElementName("name");
                mapper.SetIgnoreExtraElements(true);
                mapper.SetDiscriminator(nameof(Customer));
            });
        }
    }
}