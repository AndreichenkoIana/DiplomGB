using MarketDb.DTO;
using MarketDb.Models;
using static MarketDb.Repo.ProductRepository;

namespace MarketDb.Abstraction
{
    public interface IProductRepository
    {
        public ProductResult AddProduct (ProductDto product);
        public ProductResult DeleteProduct(int id);
        public IEnumerable<ProductDto> GetProducts();
        public IEnumerable<ProductDto> GetProductsByGroup(int groupid);
        public IEnumerable<ProductDto> GetProductsByStore(int storeid);
        public ProductResult UpdateProduct(int id, UpdateProductDto product); 
    }
}
