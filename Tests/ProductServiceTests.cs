using Moq;
using NUnit.Framework;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;
using ServiceArchitecture.Services;

namespace ServiceArchitecture.Tests {

    public class ProductServiceTests {
        private Mock<IOrderRepository> _orders;
        private Mock<IProductRepository> _products;
        private ProductService _sut;

        [SetUp]
        public void Setup() {
            _orders = new Mock<IOrderRepository>();
            _products = new Mock<IProductRepository>();
            _sut = new ProductService(_products.Object, _orders.Object);
        }

        [Test]
        public void Should_AllowPurchase() {
            _orders.Setup(x => x.CountOrdersByUser(It.IsAny<string>())).Returns(3);

            var user = new User();
            var product = new Product { QuantityAvailable = 5, PurchaseLimit = 5 };

            Assert.IsTrue(_sut.CanUserPurchase(user, product, 2));
        }

        [Test]
        public void Should_Not_AllowPurchaseToExceedQuantity() {
            _orders.Setup(x => x.CountOrdersByUser(It.IsAny<string>())).Returns(2);

            var user = new User();
            var product = new Product { QuantityAvailable = 5 };

            Assert.IsFalse(_sut.CanUserPurchase(user, product, 6));
        }

        [Test]
        public void Should_Not_AllowPurchaseToExceedLimit() {
            _orders.Setup(x => x.CountOrdersByUser(It.IsAny<string>())).Returns(2);

            var user = new User();
            var product = new Product { QuantityAvailable = 5, PurchaseLimit = 3 };

            Assert.IsFalse(_sut.CanUserPurchase(user, product, 2));
        }

        [Test]
        public void Should_UpdateProductFromOrder() {
            var product = new Product { QuantityAvailable = 5 };
            var order = new Order() { Quantity = 3 };

            _sut.UpdateProductFromOrder(product, order);

            Assert.AreEqual(2, product.QuantityAvailable);

            _products.Verify(x => x.Save(product));
        }
    }
}