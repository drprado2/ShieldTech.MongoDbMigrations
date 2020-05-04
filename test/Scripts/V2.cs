using System.Threading.Tasks;
using MongoDB.Driver;
using ShieldTech.MongoDbMigrations.Scripts;
using ShieldTech.MongoDbMigrations.Tests.Entities;

namespace ShieldTech.MongoDbMigrations.Tests.Scripts
{
    public class V2 : ScriptBase
    {
        public const string IdxName = "idx_product_code";
            
        public V2() : base(2, "Create Product indexes")
        {
        }

        public override async Task<bool> ApplyAsync(IMongoDatabase mongoDatabase)
        {
            var indexKeys = Builders<Product>.IndexKeys.Ascending(_ => _.Code);
            await mongoDatabase.GetCollection<Product>(nameof(Product)).Indexes
                .CreateOneAsync(new CreateIndexModel<Product>(indexKeys, new CreateIndexOptions {Name = IdxName, Unique = true}));
            return true;
        }

        public override async Task<bool> RevertAsync(IMongoDatabase mongoDatabase)
        {
            await mongoDatabase.GetCollection<Product>(nameof(Product)).Indexes.DropOneAsync(IdxName);
            return true;
        }
    }
}