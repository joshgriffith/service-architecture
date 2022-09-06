using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Services {
    
    public class UserService {
        private readonly IUserRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IEmailClient _email;

        public UserService(IUserRepository users, IPasswordHasher hasher, IEmailClient email) {
            _users = users;
            _hasher = hasher;
            _email = email;
        }

        public User Register(string email, string password) {
            if (!IsEmailValid(email))
                throw new ArgumentException("You must provide a valid email.", nameof(email));

            if (!IsPasswordValid(password))
                throw new ArgumentException("Your password must be at least 8 characters long.", nameof(password));

            var user = _users.FindUserByEmail(email);

            if (user != null)
                throw new InvalidOperationException("User already exists.");

            user = new User {
                Email = email,
                PasswordHash = _hasher.Hash(password)
            };

            _users.Save(user);

            _email.SendEmail(user.Email, "Welcome!", "Thank you for registering.");

            return user;
        }

        public User? Login(string email, string password) {
            var user = _users.FindUserByEmail(email);

            if (user != null && _hasher.IsValid(user.PasswordHash, password)) {
                user.LastLoginDate = DateTime.UtcNow;
                return user;
            }

            return null;
        }

        public bool IsEmailValid(string email) {
            return !string.IsNullOrEmpty(email) && email.Contains("@") && email.Length > 5;
        }

        public bool IsPasswordValid(string password) {
            return !string.IsNullOrEmpty(password) && password.Length >= 8;
        }
    }
}