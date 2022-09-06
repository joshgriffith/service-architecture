using ServiceArchitecture.Domain;

namespace ServiceArchitecture.Models {
    public class ProductStats {
        public Product Product { get; set; }
        public float Quantity { get; set; }
        public float TotalPrice { get; set; }
        public int TotalPendingOrders { get; set; }
        public int TotalShippedOrders { get; set; }
    }
}