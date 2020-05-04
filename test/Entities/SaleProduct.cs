using System;

namespace ShieldTech.MongoDbMigrations.Tests.Entities
{
    public class SaleProduct
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; } = 0;
        public decimal Value { get; set; } = 0;
        public decimal TotalValue => Quantity * Value;
    }
}