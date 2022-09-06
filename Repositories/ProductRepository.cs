using ServiceArchitecture.Core;
using ServiceArchitecture.Domain;
using ServiceArchitecture.Models;

namespace ServiceArchitecture.Repositories {

    public interface IProductRepository : IRepository<Product> {
        List<ProductStats> GetBestSellingProductStats(int top = 5);
        List<ProductStats> GetProductStats();
    }

    public class ProductRepository : DatabaseRepository<Product>, IProductRepository {
        
        private readonly IOrderRepository _orders;

        public ProductRepository(IDatabase database, IOrderRepository orders)
            : base(database) {
            _orders = orders;
        }
        
        public List<ProductStats> GetBestSellingProductStats(int top = 5) {
            return GetProductStats()
                .OrderByDescending(each => each.TotalPrice)
                .Take(top)
                .ToList();
        }

        public List<ProductStats> GetProductStats() {
            return _orders
                .Get()
                .GroupBy(order => order.Product)
                .Select(orders => new ProductStats {
                    Product = orders.Key,
                    Quantity = orders.Sum(order => order.Quantity),
                    TotalPrice = orders.Sum(order => order.TotalPrice),
                    TotalPendingOrders = orders.Count(order => order.Status == Order.StatusTypes.Pending),
                    TotalShippedOrders = orders.Count(order => order.Status == Order.StatusTypes.Shipped)
                })
                .Where(each => each.Product.DeletionDate == null)
                .ToList();
        }

        public override void Save(Product product) {
            product.LastUpdatedDate = DateTime.UtcNow;
            base.Save(product);
        }

        public override void Delete(Product product) {
            product.DeletionDate = DateTime.UtcNow;
            base.Delete(product);
        }

        public override IQueryable<Product> Get() {
            return base.Get().Where(each => each.DeletionDate == null);
        }
    }
}