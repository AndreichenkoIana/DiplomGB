using MarketDb.DTO;
using static MarketDb.Repo.StoreRepository;

namespace MarketDb.Abstraction
{
    public interface IStoreRepository
    {
        public StoreResult AddStore(StoreDto product);
        public StoreResult DeleteStore(int id);
        public IEnumerable<StoreDto> GetStores();
    }
}
