using Marketplace.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Data
{
    public class ProductRepository
    {
        private readonly List<Product> _products = new();

        public void AddProduct(Product product) => _products.Add(product);

        public List<Product> GetAvailableProducts() =>
            _products.Where(p => p.Status == "For Sale").ToList();

        public Product? GetProductById(int id) =>
            _products.FirstOrDefault(p => p.Id == id);
    }
}
