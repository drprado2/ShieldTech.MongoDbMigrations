using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using ShieldTech.MongoDbMigrations.Tests.Entities;

namespace ShieldTech.MongoDbMigrations.Tests.Maps
{
    public class ProductMap
    {
        public ProductMap()
        {
            BsonClassMap.RegisterClassMap<Product>(mapper =>
            {
                mapper.MapIdMember(p => p.Id).SetIdGenerator(CombGuidGenerator.Instance);
                mapper.MapMember(p => p.Name);
                mapper.MapMember(p => p.Code);
                mapper.SetIgnoreExtraElements(true);
                mapper.SetDiscriminator(nameof(Product));
            });
        }
    }
}