using ServiceArchitecture.Domain;
using ServiceArchitecture.Services;

namespace ServiceArchitecture.Controllers {
    public class UserController {

        private readonly UserService _users;

        public UserController(UserService users) {
            _users = users;
        }

        // POST /users
        public User RegisterUser(string email, string password) {
            return _users.Register(email, password);
        }

        // POST /users/login
        public User? Login(string email, string password) {
            return _users.Login(email, password);
        }
    }
}