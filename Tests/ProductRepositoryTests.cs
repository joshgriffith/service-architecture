using Moq;
using NUnit.Framework;
using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Models;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Tests
{
    public class ProductRepositoryTests {
        private Mock<IDatabase> _database;
        private Mock<IOrderRepository> _orders;
        private ProductRepository _sut;

        [SetUp]
        public void Setup() {
            _database = new Mock<IDatabase>();
            _orders = new Mock<IOrderRepository>();

            var products = new List<Product> {
                new() { Id = "1" },
                new() { Id = "2" },
                new() { Id = "3" },
                new() { Id = "4", DeletionDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)) }
            };

            _database.Setup(x => x.Get<Product>()).Returns(products.AsQueryable);

            _orders.Setup(x => x.Get()).Returns(new List<Order> {
                new () { Product = products[0], TotalPrice = 1, Quantity = 1 },
                new () { Product = products[0], TotalPrice = 2, Quantity = 1 },
                new () { Product = products[1], TotalPrice = 4, Quantity = 1 },
                new () { Product = products[2], TotalPrice = 8, Quantity = 4 }
            }.AsQueryable);

            _sut = new ProductRepository(_database.Object, _orders.Object);
        }

        [Test]
        public void Should_GetBestSellingProducts() {
            var products = _sut.GetBestSellingProductStats(2);
            Assert.AreEqual(2, products.Count);
            Assert.NotNull(products.FirstOrDefault(each => each.Product.Id == "3"));
            Assert.NotNull(products.FirstOrDefault(each => each.Product.Id == "2"));
        }
        
        [Test]
        public void Should_GetProductStats() {
            var products = _sut.GetProductStats();

            Assert.AreEqual(3, products.Count);

            Assert.AreEqual(3, products.First(each => each.Product.Id == "1").TotalPrice);
            Assert.AreEqual(2, products.First(each => each.Product.Id == "1").Quantity);

            Assert.AreEqual(4, products.First(each => each.Product.Id == "2").TotalPrice);
            Assert.AreEqual(1, products.First(each => each.Product.Id == "2").Quantity);

            Assert.AreEqual(8, products.First(each => each.Product.Id == "3").TotalPrice);
            Assert.AreEqual(4, products.First(each => each.Product.Id == "3").Quantity);
        }

        [Test]
        public void Should_FindProductById() {
            var product = _sut.Find("1");

            Assert.NotNull(product);
            Assert.AreEqual("1", product.Id);
        }

        [Test]
        public void Should_Not_FindDeletedProduct() {
            var product = _sut.Find("4");

            Assert.Null(product);
        }

        [Test]
        public void Should_SaveProduct() {
            var product = new Product { LastUpdatedDate = DateTime.MinValue };

            _sut.Save(product);

            Assert.AreNotEqual(DateTime.MinValue, product.LastUpdatedDate);
            _database.Verify(x => x.Save(product));
        }

        [Test]
        public void Should_DeleteProduct() {
            var product = new Product();

            _sut.Delete(product);

            Assert.NotNull(product.DeletionDate);
            _database.Verify(x => x.Delete(product));
        }
    }
}