using AutoMapper;
using MarketDb.Abstraction;
using MarketDb.Data;
using MarketDb.DTO;
using MarketDb.Models;
using Microsoft.Extensions.Caching.Memory;
using static MarketDb.Repo.ProductRepository;

namespace MarketDb.Repo
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IMapper _mapper;
        private readonly ProductsContext _context;

        public GroupRepository(IMapper mapper, ProductsContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public class GroupResult
        {
            public bool Success { get; set; }
            public int? GroupId { get; set; }
            public string Message { get; set; }
        }
        public GroupResult AddGroup(ProductGroupDto group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            var entityGroup = _context.ProductGroups.FirstOrDefault(x => x.Name.ToLower() == group.Name.ToLower());
            if (entityGroup != null)
            {
                return new GroupResult
                {
                    Success = false,
                    GroupId = entityGroup.Id,
                    Message = "Группа уже существует"
                };
            }
            var entity = _mapper.Map<ProductGroup>(group);
            _context.ProductGroups.Add(entity);
            _context.SaveChanges();

            return new GroupResult
            {
                Success = false,
                GroupId = entity.Id,
                Message = "Группа успешно добавлена"
            }; 
        }

        public IEnumerable<ProductGroupDto> GetGroups()
        {
            var groupsList = _context.ProductGroups
                                     .Select(x => _mapper.Map<ProductGroupDto>(x))
                                     .ToList();
            return groupsList;
        }
        public GroupResult DeleteGroup(int id)
        {
            var group = _context.ProductGroups.FirstOrDefault(p => p.Id == id);
            if (group == null)
            {
                return new GroupResult
                {
                    Success = false,
                    Message = "Группа не найдена"
                };
            }
            var hasProducts = _context.Products.Any(p => p.ProductGroupId == id);
            if (hasProducts)
            {
                return new GroupResult
                {
                    Success = false,
                    Message = "Невозможно удалить группу, так как в ней есть продукты"
                };
            }
            _context.ProductGroups.Remove(group);
            _context.SaveChanges();
            return new GroupResult
            {
                Success = true,
                Message = "Группа успешно удалена"
            };
        }
    }
}
