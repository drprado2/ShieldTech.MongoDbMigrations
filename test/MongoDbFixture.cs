using System;
using MongoDB.Bson;
using MongoDB.Driver;
using ShieldTech.MongoDbMigrations.Tests.Maps;

namespace ShieldTech.MongoDbMigrations.Tests
{
    public class MongoDbFixture : IDisposable
    {
        public const string ConnectionString = "mongodb://localhost:28051,localhost:28052/?replicaSet=rs-shard-1&readPreference=primary&appname=MigrationsTest&ssl=false";
        public string DataBaseName = Guid.NewGuid().ToString();

        public MongoDbFixture()
        {
            RegisterMaps();
            MongoClient = new MongoClient(ConnectionString);
            MongoDatabase = MongoClient.GetDatabase(DataBaseName);
            MongoSession = MongoClient.StartSession();
        }

        private void RegisterMaps()
        {
            new CustomerMap();
            new ProductMap();
            new SaleMap();
            new SaleProductMap();
        }

        public IMongoClient MongoClient { get; private set; }
        public IMongoDatabase MongoDatabase { get; private set; }
        public IClientSessionHandle MongoSession { get; private set; }

        public void Dispose()
        {
            MongoSession.Client.DropDatabase(DataBaseName);
            MongoSession?.Dispose();
        }
    }
}