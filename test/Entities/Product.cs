using System;

namespace ShieldTech.MongoDbMigrations.Tests.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
    }
}