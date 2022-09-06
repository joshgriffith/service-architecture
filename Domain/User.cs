using ServiceArchitecture.Core;

namespace ServiceArchitecture.Domain {
    public class User : IEntity {
        public string Id { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DeletionDate { get; set; }

        public User() {
            CreationDate = DateTime.UtcNow;
        }
    }
}