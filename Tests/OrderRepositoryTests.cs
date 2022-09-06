using Moq;
using NUnit.Framework;
using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Tests
{
    public class OrderRepositoryTests {
        private Mock<IDatabase> _database;
        private OrderRepository _sut;

        [SetUp]
        public void Setup() {
            _database = new Mock<IDatabase>();

            _database.Setup(x => x.Get<Order>()).Returns(new List<Order> {
                new () { User = new User { Id = "1"} },
                new () { User = new User { Id = "1"} },
                new () { User = new User { Id = "2"} },
                new () { User = new User { Id = "1"}, DeletionDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)) }
            }.AsQueryable);

            _sut = new OrderRepository(_database.Object);
        }
        
        [Test]
        public void Should_GetOrders() {
            Assert.AreEqual(3, _sut.Get().Count());
        }

        [Test]
        public void Should_GetOrdersByUser() {
            var orders = _sut.GetOrdersByUser("1");
            Assert.AreEqual(2, orders.Count);
        }

        [Test]
        public void Should_CountOrders() {
            var orders = _sut.CountOrdersByUser("1");
            Assert.That(orders, Is.EqualTo(2));
        }

        [Test]
        public void Should_SaveOrder() {
            var order = new Order();
            _sut.Save(order);
            _database.Verify(x => x.Save(order));
        }
    }
}