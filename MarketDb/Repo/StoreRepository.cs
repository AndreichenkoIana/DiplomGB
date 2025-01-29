using AutoMapper;
using MarketDb.Abstraction;
using MarketDb.Data;
using MarketDb.DTO;
using MarketDb.Models;

namespace MarketDb.Repo
{
    public class StoreRepository : IStoreRepository
    {
        private readonly IMapper _mapper;
        private readonly ProductsContext _context;

        public StoreRepository(IMapper mapper, ProductsContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public class StoreResult
        {
            public bool Success { get; set; }
            public int? ProductId { get; set; }
            public string Message { get; set; }
        }
        public StoreResult AddStore(StoreDto store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            var entityProduct = _context.Products.FirstOrDefault(x => x.Name.ToLower() == store.Name.ToLower());
          

            if (entityProduct != null)
            {
                return new StoreResult
                {
                    Success = false,
                    ProductId = entityProduct.Id,
                    Message = "Магазин уже существует"
                };
            }
            var entity = _mapper.Map<Store>(store);
            _context.Stores.Add(entity);
            _context.SaveChanges();
            return new StoreResult
            {
                Success = true,
                ProductId = entity.Id,
                Message = "Магазин успешно создан"
            };
        }
        public IEnumerable<StoreDto> GetStores()
        {

            var storesList = _context.Stores
                                     .Select(x => _mapper.Map<StoreDto>(x))
                                     .ToList();
            return storesList;
        }
        public StoreResult DeleteStore(int id)
        {
            var store = _context.Stores.FirstOrDefault(p => p.Id == id);
            if (store == null)
            {
                return new StoreResult
                {
                    Success = false,
                    Message = "Магазин не найден"
                };
            }
            var hasProducts = _context.Products.Any(p => p.StoreID == id);
            if (hasProducts)
            {
                return new StoreResult
                {
                    Success = false,
                    Message = "Невозможно удалить магазин, так как в нем есть продукты"
                };
            }
            _context.Stores.Remove(store);
            _context.SaveChanges();
            return new StoreResult
            {
                Success = true,
                Message = "Магазин успешно удален"
            };
        }
    }
}
