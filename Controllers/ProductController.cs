using ServiceArchitecture.Domain;
using ServiceArchitecture.Repositories;

namespace ServiceArchitecture.Controllers {
    public class ProductController {
        
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository repository) {
            _productRepository = repository;
        }

        // GET /products
        public List<Product> GetProducts() {
            return _productRepository.Get().ToList();
        }

        // POST /products
        public Product CreateProduct(Product product) {
            _productRepository.Save(product);
            return product;
        }

        // DELETE /products/{id}
        public void DeleteProduct(string id) {
            var product = _productRepository.Find(id);

            if (product != null)
                _productRepository.Delete(product);
        }
    }
}