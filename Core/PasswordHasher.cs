using System.Security.Cryptography;
using System.Text;

namespace ServiceArchitecture.Core {

    public interface IPasswordHasher {
        bool IsValid(string passwordHash, string password);
        string Hash(string password);
    }

    public class PasswordHasher : IPasswordHasher {
        private readonly string _secret;

        public PasswordHasher(string secret) {
            _secret = secret;
        }

        public bool IsValid(string passwordHash, string password) {
            return passwordHash == Hash(password);
        }

        public string Hash(string password) {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_secret + password));
            return BitConverter.ToString(hashedBytes);
        }
    }
}