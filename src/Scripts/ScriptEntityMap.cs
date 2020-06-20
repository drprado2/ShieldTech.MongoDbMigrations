using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace ShieldTech.MongoDbMigrations.Scripts
{
    public class ScriptEntityMap
    {
        public ScriptEntityMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(ScriptEntity)))
                BsonClassMap.RegisterClassMap<ScriptEntity>(mapper =>
                {
                    mapper.MapIdMember(p => p.Id).SetIdGenerator(CombGuidGenerator.Instance);
                    mapper.MapMember(p => p.Name);
                    mapper.MapMember(p => p.SequenceVersion);
                    mapper.MapMember(p => p.DateApplied);
                    mapper.SetIgnoreExtraElements(true);
                    mapper.SetDiscriminator(ScriptEntity.CollectionName);
                    mapper.MapCreator(p => new ScriptEntity(p.Id, p.SequenceVersion, p.Name, p.DateApplied));
                });
        }
    }
}