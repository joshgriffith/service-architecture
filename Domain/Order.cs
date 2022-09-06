using ServiceArchitecture.Core;

namespace ServiceArchitecture.Domain {
    public class Order : IEntity {
        public string Id { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
        public float TotalPrice { get; set; }
        public int Quantity { get; set; }
        public StatusTypes Status { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public Order() {
            CreationDate = DateTime.UtcNow;
        }

        public enum StatusTypes {
            Pending = 0,
            Shipped = 1,
            Delivered = 2
        }
    }
}