using ServiceArchitecture.Core;

namespace ServiceArchitecture.Domain {
    public class Product : IEntity {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int QuantityAvailable { get; set; }
        public int PurchaseLimit { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public Product() {
            CreationDate = DateTime.UtcNow;
        }
    }
}