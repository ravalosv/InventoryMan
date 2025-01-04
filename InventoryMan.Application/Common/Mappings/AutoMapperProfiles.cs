using AutoMapper;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Core.Entities;
using System.Linq.Dynamic.Core;

namespace InventoryMan.Application.Common.Mappings
{
    public class AutoMapperProfiles : AutoMapper.Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(p => p.CategoryName, opc => opc.MapFrom(p => p.Category.Name));

            CreateMap<Core.Entities.Test, TestDto>();

            CreateMap<PagedResult<Product>, List<ProductDto>>()
                .ConvertUsing<PagedResultToListConverter<Product, ProductDto>>();
        }
    }

    public class PagedResultToListConverter<TSource, TDestination> : ITypeConverter<PagedResult<TSource>, List<TDestination>>
    {
        public List<TDestination> Convert(PagedResult<TSource> source, List<TDestination> destination, ResolutionContext context)
        {
            return source.Queryable.Select(item => context.Mapper.Map<TDestination>(item)).ToList();
        }
    }
}
