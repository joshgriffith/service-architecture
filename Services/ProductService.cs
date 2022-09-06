using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Services {

    public interface IProductService {
        bool CanUserPurchase(User user, Product product, int quantity);
        void UpdateProductFromOrder(Product product, Order order);
    }

    public class ProductService : IProductService {

        private readonly IProductRepository _products;
        private readonly IOrderRepository _orders;

        public ProductService(IProductRepository products, IOrderRepository orders) {
            _products = products;
            _orders = orders;
        }
        
        public bool CanUserPurchase(User user, Product product, int quantity) {
            if (product.QuantityAvailable < quantity)
                return false;

            var orderCount = _orders.CountOrdersByUser(user.Id);

            if (product.PurchaseLimit > 0 && product.PurchaseLimit < (orderCount + quantity))
                return false;

            return true;
        }

        public void UpdateProductFromOrder(Product product, Order order) {
            product.QuantityAvailable -= order.Quantity;
            product.LastPurchaseDate = DateTime.UtcNow;
            _products.Save(product);
        }
    }
}