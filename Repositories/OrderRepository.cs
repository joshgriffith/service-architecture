using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;

namespace ServiceArchitecture.Repositories {
    public interface IOrderRepository : IRepository<Order> {
        List<Order> GetOrdersByUser(string userId);
        int CountOrdersByUser(string userId);
    }

    public class OrderRepository : DatabaseRepository<Order>, IOrderRepository {

        public OrderRepository(IDatabase database)
            : base(database) {
        }
        
        public List<Order> GetOrdersByUser(string userId) {
            return Get().Where(each => each.User.Id == userId).ToList();
        }

        public int CountOrdersByUser(string userId) {
            return Get().Count(each => each.User.Id == userId);
        }

        public override void Delete(Order order) {
            order.DeletionDate = DateTime.UtcNow;
            base.Delete(order);
        }

        public override IQueryable<Order> Get() {
            return base.Get().Where(each => each.DeletionDate == null);
        }
    }
}