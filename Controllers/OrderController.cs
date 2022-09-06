using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;
using ServiceArchitecture.Services;

namespace ServiceArchitecture.Controllers {
    public class OrderController {
        private readonly User _currentUser;
        private readonly IOrderService _orderService;
        private readonly IOrderRepository _orderRepository;

        public OrderController(User currentUser, IOrderService orderService, IOrderRepository orderRepository) {
            _currentUser = currentUser;
            _orderService = orderService;
            _orderRepository = orderRepository;
        }

        // GET /orders
        public List<Order> GetOrders() {
            return _orderRepository.GetOrdersByUser(_currentUser.Id);
        }

        // POST /orders
        public Order CreateOrder(string productId, int quantity) {
            return _orderService.CreateOrder(_currentUser.Id, productId, quantity);
        }

        // POST /orders/{id}
        public Order UpdateOrderStatus(string orderId, Order.StatusTypes status) {
            return _orderService.UpdateOrderStatus(orderId, status);
        }
    }
}