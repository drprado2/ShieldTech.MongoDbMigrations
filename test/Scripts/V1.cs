using System.Threading.Tasks;
using MongoDB.Driver;
using ShieldTech.MongoDbMigrations.Scripts;
using ShieldTech.MongoDbMigrations.Tests.Entities;

namespace ShieldTech.MongoDbMigrations.Tests.Scripts
{
    public class V1 : ScriptBase
    {
        public V1() : base(1, "Create Customer Indexes")
        {
        }

        public const string IdxName = "idx_customer_name_text";

        public override async Task<bool> ApplyAsync(IMongoDatabase mongoDatabase)
        {
            var indexKeys = Builders<Customer>.IndexKeys.Text(_ => _.Name);
            await mongoDatabase.GetCollection<Customer>(nameof(Customer)).Indexes
                .CreateOneAsync(new CreateIndexModel<Customer>(indexKeys, new CreateIndexOptions {Name = IdxName}));
            return true;
        }

        public override async Task<bool> RevertAsync(IMongoDatabase mongoDatabase)
        {
            await mongoDatabase.GetCollection<Customer>(nameof(Customer)).Indexes.DropOneAsync(IdxName);
            return true;
        }
    }
}