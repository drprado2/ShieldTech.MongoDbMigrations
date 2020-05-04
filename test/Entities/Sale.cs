using System;
using System.Collections.Generic;
using System.Linq;

namespace ShieldTech.MongoDbMigrations.Tests.Entities
{
    public class Sale
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public List<SaleProduct> Products { get; set; }
        public decimal TotalValue => Products?.Sum(x => x.TotalValue) ?? 0;
        public DateTime Date { get; set; }
    }
}