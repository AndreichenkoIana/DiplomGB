using AutoMapper;
using MarketDb.DTO;
using MarketDb.Models;

namespace MarketDb.Repo
{
    public class MappingProFile:Profile
    {
        public MappingProFile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<ProductGroup, ProductGroupDto>();
            CreateMap<ProductGroupDto, ProductGroup>();
            CreateMap<Store, StoreDto>();
            CreateMap<StoreDto, Store>();
        }
    }
}
