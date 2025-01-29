using MarketDb.DTO;
using MarketDb.Models;
using static MarketDb.Repo.GroupRepository;

namespace MarketDb.Abstraction
{
    public interface IGroupRepository
    {
        public GroupResult AddGroup(ProductGroupDto group);
        public GroupResult DeleteGroup(int id);
        public IEnumerable<ProductGroupDto> GetGroups();
    }
}
