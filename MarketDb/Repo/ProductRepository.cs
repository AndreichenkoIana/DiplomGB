using AutoMapper;
using MarketDb.Abstraction;
using MarketDb.Data;
using MarketDb.DTO;
using MarketDb.Models;
using Microsoft.Extensions.Caching.Memory;

namespace MarketDb.Repo
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMapper _mapper;
        private readonly ProductsContext _context;


        public ProductRepository(IMapper mapper, ProductsContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public class ProductResult
        {
            public bool Success { get; set; }
            public int? ProductId { get; set; }
            public string Message { get; set; }
        }
        private bool StoreExists(int storeId) =>
        _context.Stores.Any(x => x.Id == storeId);

        private bool ProductGroupExists(int groupId) =>
            _context.ProductGroups.Any(x => x.Id == groupId);

        public ProductResult AddProduct(ProductDto product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            
            var entityProduct = _context.Products.FirstOrDefault(x => x.Name.ToLower() == product.Name.ToLower());
            if (entityProduct != null)
            {
                return new ProductResult
                {
                    Success = false,
                    ProductId = entityProduct.Id,
                    Message = "Продукт уже существует"
                };
            }

            var entity = _mapper.Map<Product>(product);
            _context.Products.Add(entity);
            var store = _context.Stores.FirstOrDefault(x => x.Id == entity.StoreID);
            var group = _context.ProductGroups.FirstOrDefault(x => x.Id == entity.ProductGroupId);

            if (!StoreExists(entity.StoreID))
            {
                return new ProductResult
                {
                    Success = false,
                    Message = "Store not found."
                };
            }

            if (!ProductGroupExists(entity.ProductGroupId))
            {
                return new ProductResult
                {
                    Success = false,
                    Message = "Product group not found."
                };
            }
            _context.SaveChanges();
            store.Quantity += entity.Count;
            _context.Stores.Update(store);
            _context.SaveChanges();

            return new ProductResult
            {
                Success = true,
                ProductId = entity.Id,
                Message = "Продукт успешно добавлен"
            };
        }
        public IEnumerable<ProductDto> GetProducts()
        {
            var productsList = _context.Products
                                     .OrderBy(x => x.Id)
                                     .Select(x => _mapper.Map<ProductDto>(x))
                                     .ToList();
            return productsList;
        }
        public IEnumerable<ProductDto> GetProductsByGroup( int GroupID)
        {
            var productsList = _context.Products
                                     .Where(x => x.ProductGroupId == GroupID )
                                     .OrderBy(x => x.Id)
                                     .Select(x => _mapper.Map<ProductDto>(x))
                                     .ToList();
            return productsList;
        }
        public IEnumerable<ProductDto> GetProductsByStore(int StoreID)
        {
            var productsList = _context.Products
                                     .Where(x => x.StoreID == StoreID)
                                     .OrderBy(x => x.Id)
                                     .Select(x => _mapper.Map<ProductDto>(x))
                                     .ToList();
            return productsList;
        }
        public ProductResult DeleteProduct(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return new ProductResult
                {
                    Success = false,
                    Message = "Продукт не найден"
                };
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return new ProductResult
            {
                Success = true,
                Message = "Продукт успешно удален"
            };
        }
        public ProductResult UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return new ProductResult
                {
                    Success = false,
                    Message = "Продукт не найден"
                };
            }

            bool isUpdated = false;

            if (updateProductDto.Price.HasValue)
            {
                product.Price = updateProductDto.Price.Value;
                isUpdated = true;
            }

            if (updateProductDto.Count.HasValue)
            {
                int difference = (int)(updateProductDto.Count.Value - product.Count);

                if (difference != 0)
                {
                    var currentStore = _context.Stores.FirstOrDefault(s => s.Id == product.StoreID);
                    if (currentStore == null)
                    {
                        return new ProductResult
                        {
                            Success = false,
                            Message = "Магазин, связанный с продуктом, не найден"
                        };
                    }

                    product.Count = updateProductDto.Count.Value;
                    currentStore.Quantity += difference;
                    _context.Stores.Update(currentStore);
                    isUpdated = true;
                }
            }

            if (updateProductDto.ProductGroupId.HasValue && updateProductDto.ProductGroupId.Value != product.ProductGroupId)
            {
                product.ProductGroupId = updateProductDto.ProductGroupId.Value;
                isUpdated = true;
            }

            if (updateProductDto.StoreId.HasValue && updateProductDto.StoreId.Value != product.StoreID)
            {
                var oldStore = _context.Stores.FirstOrDefault(s => s.Id == product.StoreID);
                var newStore = _context.Stores.FirstOrDefault(s => s.Id == updateProductDto.StoreId.Value);

                if (newStore == null)
                {
                    return new ProductResult
                    {
                        Success = false,
                        Message = "Новый магазин не найден"
                    };
                }

                if (oldStore != null)
                {
                    oldStore.Quantity -= product.Count;
                    _context.Stores.Update(oldStore);
                }

                newStore.Quantity += product.Count;
                _context.Stores.Update(newStore);

                product.StoreID = updateProductDto.StoreId.Value;
                isUpdated = true;
            }

            if (!isUpdated)
            {
                return new ProductResult
                {
                    Success = false,
                    Message = "Никаких изменений не было внесено"
                };
            }

            _context.Products.Update(product);
            _context.SaveChanges();

            return new ProductResult
            {
                Success = true,
                ProductId = id,
                Message = "Продукт успешно обновлен"
            };
        }

    }
}
