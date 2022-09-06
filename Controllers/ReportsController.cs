using ServiceArchitecture.Models;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Controllers {
    public class ReportsController {

        private readonly IProductRepository _products;

        public ReportsController(IProductRepository products) {
            _products = products;
        }
        
        // GET /reports/products
        public List<ProductStats> GetProductStats() {
            return _products.GetProductStats();
        }

        // GET /reports/best-selling-products
        public List<ProductStats> GetBestSellingProducts() {
            return _products.GetBestSellingProductStats(5);
        }
    }
}