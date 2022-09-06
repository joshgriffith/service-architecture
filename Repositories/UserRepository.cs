using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;

namespace ServiceArchitecture.Repositories {

    public interface IUserRepository : IRepository<User> {
        User? FindUserByEmail(string email);
    }

    public class UserRepository : DatabaseRepository<User>, IUserRepository {
        
        public UserRepository(IDatabase database)
            : base(database) {
        }
        
        public User? FindUserByEmail(string email) {
            return Get().FirstOrDefault(each => each.Email == email);
        }
        
        public override void Delete(User user) {
            user.DeletionDate = DateTime.UtcNow;
            base.Delete(user);
        }

        public override IQueryable<User> Get() {
            return base.Get().Where(each => each.DeletionDate == null);
        }
    }
}