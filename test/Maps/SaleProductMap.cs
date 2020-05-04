using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using ShieldTech.MongoDbMigrations.Tests.Entities;

namespace ShieldTech.MongoDbMigrations.Tests.Maps
{
    public class SaleProductMap
    {
        public SaleProductMap()
        {
            BsonClassMap.RegisterClassMap<SaleProduct>(mapper =>
            {
                mapper.MapIdMember(p => p.Id).SetIdGenerator(CombGuidGenerator.Instance);
                mapper.MapMember(p => p.Product);
                mapper.MapMember(p => p.Quantity).SetDefaultValue(0);
                mapper.MapMember(p => p.Value).SetDefaultValue(0);
                mapper.MapMember(p => p.TotalValue);
                mapper.SetIgnoreExtraElements(true);
                mapper.SetDiscriminator(nameof(SaleProduct));
            });
        }
    }
}