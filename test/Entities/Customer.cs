using System;

namespace ShieldTech.MongoDbMigrations.Tests.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Extra { get; set; }
    }
}