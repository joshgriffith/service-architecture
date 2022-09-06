using Moq;
using NUnit.Framework;
using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;
using ServiceArchitecture.Services;

namespace ServiceArchitecture.Tests
{
    public class UserServiceTests {
        private Mock<IEmailClient> _email;
        private Mock<IPasswordHasher> _validator;
        private Mock<IUserRepository> _users;
        private UserService _sut;

        [SetUp]
        public void Setup() {
            _email = new Mock<IEmailClient>();
            _validator = new Mock<IPasswordHasher>();
            _users = new Mock<IUserRepository>();
            _sut = new UserService(_users.Object, _validator.Object, _email.Object);
        }

        private void SetupFindUserByEmail() {
            _users.Setup(x => x.FindUserByEmail(It.IsAny<string>())).Returns<string>(email => new User {
                Id = "1",
                Email = email,
                PasswordHash = "hash"
            });
        }

        [Test]
        public void Should_RegisterUser() {
            _validator.Setup(x => x.Hash(It.IsAny<string>())).Returns("passwordhash");
            
            var user = _sut.Register("test@test.com", "password123");

            Assert.IsNotNull(user);
            Assert.That(user.PasswordHash, Is.EqualTo("passwordhash"));

            _users.Verify(x => x.Save(user));
            _email.Verify(x => x.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void Should_FailRegistration_UserAlreadyExists() {
            SetupFindUserByEmail();
            
            Assert.Throws<InvalidOperationException>(() => {
                _sut.Register("test@test.com", "password123");
            });
        }

        [Test]
        public void Should_Login() {
            _validator.Setup(x => x.IsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            SetupFindUserByEmail();

            var user = _sut.Login("test@test.com", "password123");

            Assert.IsNotNull(user);
        }

        [Test]
        public void Should_FailLogin_InvalidEmail() {
            _users.Setup(x => x.FindUserByEmail(It.IsAny<string>())).Returns<string>(null);
            
            var user = _sut.Login("test@test.com", "password123");
            Assert.IsNull(user);
        }

        [Test]
        public void Should_FailLogin_InvalidPassword() {
            SetupFindUserByEmail();

            _validator.Setup(x => x.IsValid(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var user = _sut.Login("test@test.com", "password123");
            Assert.IsNull(user);
        }
    }
}