using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Services {

    public interface IOrderService {
        Order UpdateOrderStatus(string orderId, Order.StatusTypes status);
        Order CreateOrder(string userId, string productId, int quantity);
    }

    public class OrderService : IOrderService {
        private readonly IOrderRepository _orders;
        private readonly IUserRepository _users;
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;
        private readonly IEmailClient _email;

        public OrderService(IOrderRepository orders, IUserRepository users, IProductRepository productRepository, IProductService productService, IEmailClient email) {
            _orders = orders;
            _users = users;
            _productRepository = productRepository;
            _productService = productService;
            _email = email;
        }

        public Order CreateOrder(string userId, string productId, int quantity) {
            var user = _users.Find(userId);

            if (user == null)
                throw new ArgumentException("Invalid user: " + userId);

            var product = _productRepository.Find(productId);

            if(product == null)
                throw new ArgumentException("Invalid product id: " + productId);

            var order = new Order {
                User = user,
                Product = product,
                CreationDate = DateTime.UtcNow,
                Quantity = quantity,
                TotalPrice = product.Price * quantity
            };

            var canPurchase = _productService.CanUserPurchase(user, product, quantity);

            if (!canPurchase)
                throw new InvalidOperationException("Unable to complete purchase.");
            
            _orders.Save(order);

            _productService.UpdateProductFromOrder(product, order);

            _email.SendEmail(user.Email, "Thank you for your purchase", $"You purchased {order.Quantity} {product.Name}. Your total price is {order.TotalPrice}.");

            return order;
        }

        public Order UpdateOrderStatus(string orderId, Order.StatusTypes status) {
            var order = _orders.Find(orderId);

            if (order == null)
                throw new ArgumentException("Invalid order: " + orderId);

            if (status == Order.StatusTypes.Pending || order.Status == status)
                return order;

            if(status == Order.StatusTypes.Shipped)
                OrderShipped(order);
            else if (status == Order.StatusTypes.Delivered)
                OrderDelivered(order);

            _orders.Save(order);

            return order;
        }

        public TimeSpan GetEstimatedDeliveryDate(string orderId) {
            var order = _orders.Find(orderId);

            if (order == null)
                throw new ArgumentException("Invalid order: " + orderId);

            if (order.Status == Order.StatusTypes.Delivered)
                return TimeSpan.Zero;

            var estimate = TimeSpan.FromDays(order.Quantity);

            if (order.Status == Order.StatusTypes.Shipped)
                estimate -= (order.ShippedDate.Value - DateTime.UtcNow);

            return estimate;
        }

        private void OrderShipped(Order order) {
            order.Status = Order.StatusTypes.Shipped;
            order.ShippedDate = DateTime.UtcNow;
            
            _email.SendEmail(order.User.Email, "Your order has shipped!", $"Your {order.Product.Name} has shipped.");
        }

        private void OrderDelivered(Order order) {
            order.Status = Order.StatusTypes.Delivered;
        }
    }
}