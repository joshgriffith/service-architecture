using Moq;
using NUnit.Framework;
using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;
using ServiceArchitecture.Services;

namespace ServiceArchitecture.Tests {

    public class OrderServiceTests {
        private Mock<IOrderRepository> _orders;
        private Mock<IUserRepository> _users;
        private Mock<IProductRepository> _productRepository;
        private Mock<IProductService> _productService;
        private Mock<IEmailClient> _email;
        private OrderService _sut;

        [SetUp]
        public void Setup() {
            _orders = new Mock<IOrderRepository>();
            _users = new Mock<IUserRepository>();
            _productRepository = new Mock<IProductRepository>();
            _productService = new Mock<IProductService>();
            _email = new Mock<IEmailClient>();
            _users = new Mock<IUserRepository>();
            _sut = new OrderService(_orders.Object, _users.Object, _productRepository.Object, _productService.Object, _email.Object);
        }

        [Test]
        public void Should_CreateOrder() {
            _users.Setup(x => x.Find(It.IsAny<string>())).Returns(new User { Id = "1", Email = "test@test.com" });
            _productRepository.Setup(x => x.Find(It.IsAny<string>())).Returns(new Product { Id = "1" });
            _productService.Setup(x => x.CanUserPurchase(It.IsAny<User>(), It.IsAny<Product>(), It.IsAny<int>())).Returns(true);

            var order = _sut.CreateOrder("1", "1", 1);

            Assert.IsNotNull(order);
            Assert.IsNotNull(order.Product);
            Assert.IsNotNull(order.User);

            Assert.That(order.Quantity, Is.EqualTo(1));
            Assert.That(order.Product.Id, Is.EqualTo("1"));
            Assert.That(order.User.Id, Is.EqualTo("1"));

            _orders.Verify(x => x.Save(It.IsAny<Order>()));
            _productService.Verify(x => x.UpdateProductFromOrder(It.IsAny<Product>(), It.IsAny<Order>()));
            _email.Verify(x => x.SendEmail("test@test.com", It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void Should_UpdateOrderStatus_Shipped() {
            _orders.Setup(x => x.Find("1")).Returns(new Order { Id = "1", User = new User(), Product = new Product(), Status = Order.StatusTypes.Pending });
            var order = _sut.UpdateOrderStatus("1", Order.StatusTypes.Shipped);

            Assert.NotNull(order);
            Assert.AreEqual(Order.StatusTypes.Shipped, order.Status);

            _orders.Verify(x => x.Save(order));
            _email.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void Should_UpdateOrderStatus_Delivered() {
            _orders.Setup(x => x.Find("1")).Returns(new Order { Id = "1", User = new User(), Product = new Product(), Status = Order.StatusTypes.Shipped });
            var order = _sut.UpdateOrderStatus("1", Order.StatusTypes.Delivered);

            Assert.NotNull(order);
            Assert.AreEqual(Order.StatusTypes.Delivered, order.Status);

            _orders.Verify(x => x.Save(order));
        }
    }
}