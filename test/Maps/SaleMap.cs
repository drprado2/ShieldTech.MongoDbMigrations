using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using ShieldTech.MongoDbMigrations.Tests.Entities;

namespace ShieldTech.MongoDbMigrations.Tests.Maps
{
    public class SaleMap
    {
        public SaleMap()
        {
            BsonClassMap.RegisterClassMap<Sale>(mapper =>
            {
                mapper.MapIdMember(p => p.Id).SetIdGenerator(CombGuidGenerator.Instance);
                mapper.MapMember(p => p.Customer);
                mapper.MapMember(p => p.Products);
                mapper.MapMember(p => p.TotalValue);
                mapper.MapMember(p => p.Date);
                mapper.SetIgnoreExtraElements(true);
                mapper.SetDiscriminator(nameof(Sale));
            });
        }
    }
}