using Moq;
using NUnit.Framework;
using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Tests
{
    public class UserRepositoryTests {
        private Mock<IDatabase> _database;
        private UserRepository _sut;

        [SetUp]
        public void Setup() {
            _database = new Mock<IDatabase>();

            _database.Setup(x => x.Get<User>()).Returns(new List<User> {
                new () { Email = "test1@test.com" },
                new () { Email = "test2@test.com" },
                new () { Email = "deleted@test.com", DeletionDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)) }
            }.AsQueryable);

            _sut = new UserRepository(_database.Object);
        }
        
        [Test]
        public void Should_GetUsers() {
            var users = _sut.Get().ToList();
            Assert.That(users.Count, Is.EqualTo(2));
        }

        [Test]
        public void Should_FindUserByEmail() {
            var user = _sut.FindUserByEmail("test2@test.com");
            Assert.IsNotNull(user);

            user = _sut.FindUserByEmail("test3@test.com");
            Assert.IsNull(user);
        }

        [Test]
        public void Should_SaveUser() {
            var user = new User {
                Email = "test3@test.com"
            };

            _sut.Save(user);

            _database.Verify(x => x.Save(user));
        }

        [Test]
        public void Should_DeleteUser() {
            var user = new User {
                Id = "1",
                Email = "test3@test.com"
            };

            _sut.Delete(user);

            _database.Verify(x => x.Delete(user));
        }
    }
}