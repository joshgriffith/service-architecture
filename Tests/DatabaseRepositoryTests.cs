using Moq;
using NUnit.Framework;
using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Tests {
    public class DatabaseRepositoryTests {

        private Mock<IDatabase> _database;
        private DatabaseRepository<User> _sut;

        [SetUp]
        public void Setup() {
            _database = new Mock<IDatabase>();

            _database.Setup(x => x.Get<User>()).Returns(new List<User> {
                new () { Id = "1" },
                new () { Id = "2" }
            }.AsQueryable);

            _sut = new DatabaseRepository<User>(_database.Object);
        }

        [Test]
        public void Should_Get() {
            var users = _sut.Get();

            Assert.AreEqual(2, users.Count());
            
            _database.Verify(x => x.Get<User>());
        }

        [Test]
        public void Should_Save() {
            var user = new User();
            _sut.Save(user);
            
            _database.Verify(x => x.Save(user));
        }

        [Test]
        public void Should_Delete() {
            var user = new User();
            _sut.Delete(user);
            
            _database.Verify(x => x.Delete(user));
        }
    }
}