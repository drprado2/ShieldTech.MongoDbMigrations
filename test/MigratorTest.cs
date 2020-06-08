using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NSubstitute;
using ShieldTech.MongoDbMigrations.Scripts;
using ShieldTech.MongoDbMigrations.Tests.Entities;
using ShieldTech.MongoDbMigrations.Tests.Scripts;
using Xunit;

namespace ShieldTech.MongoDbMigrations.Tests
{
    public class MigratorTest : IDisposable
    {
        private MongoDbFixture _mongoDbFixture;

        public MigratorTest()
        {
            _mongoDbFixture = new MongoDbFixture();
        }

        [Fact]
        public async Task Should_apply_scripts_and_revert()
        {
            var logger = Substitute.For<ILogger>();
            var expectedScript1 = new V1();
            var expectedScript2 = new V2();
            var migrator = new Migrator(MongoDbFixture.ConnectionString, _mongoDbFixture.DataBaseName, logger);
            var scriptCollection = _mongoDbFixture.MongoDatabase.GetCollection<ScriptEntity>(ScriptEntity.CollectionName);
            var customerCollection = _mongoDbFixture.MongoDatabase.GetCollection<Customer>(nameof(Customer));
            var productCollection = _mongoDbFixture.MongoDatabase.GetCollection<Product>(nameof(Product));
            
            // Migrate just 1 version
            await migrator.SetDatabaseToVersionAsync(1);

            var scripts = await scriptCollection.Find(FilterDefinition<ScriptEntity>.Empty).ToListAsync();
            scripts.Should()
                .HaveCount(1).And
                .OnlyContain(x => x.SequenceVersion == expectedScript1.SequenceVersion && x.Name == expectedScript1.Name);
            
            var customerIndexes = await customerCollection.Indexes.List().ToListAsync();
            customerIndexes.Should().HaveCount(2).And.ContainSingle(x => x["name"] == V1.IdxName);
            
            var productIndexes = await productCollection.Indexes.List().ToListAsync();
            productIndexes.Should().BeEmpty();
            
            // Migrate to the last available version
            await migrator.SetDatabaseToVersionAsync();
            
            scripts = await scriptCollection.Find(FilterDefinition<ScriptEntity>.Empty).ToListAsync();
            scripts.Should()
                .HaveCount(2).And
                .ContainSingle(x => x.SequenceVersion == expectedScript1.SequenceVersion && x.Name == expectedScript1.Name).And
                .ContainSingle(x => x.SequenceVersion == expectedScript2.SequenceVersion && x.Name == expectedScript2.Name);

            productIndexes = await productCollection.Indexes.List().ToListAsync();
            productIndexes.Should().HaveCount(2).And.ContainSingle(x => x["name"] == V2.IdxName);
            
            // Revert to 0 version
            await migrator.SetDatabaseToVersionAsync(0);
            
            scripts = await scriptCollection.Find(FilterDefinition<ScriptEntity>.Empty).ToListAsync();
            scripts.Should().BeEmpty();
            
            customerIndexes = await customerCollection.Indexes.List().ToListAsync();
            customerIndexes.Should().HaveCount(1).And.NotContain(x => x["name"] == V1.IdxName);
            
            productIndexes = await productCollection.Indexes.List().ToListAsync();
            productIndexes.Should().HaveCount(1).And.NotContain(x => x["name"] == V2.IdxName);
        }

        public void Dispose()
        {
            _mongoDbFixture?.Dispose();
        }
    }
}