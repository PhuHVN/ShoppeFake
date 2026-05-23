using AutoMapper;
using ShoppeFake.Application.DTOs.AccountDtos;
using ShoppeFake.Application.DTOs.AttributeDtos;
using ShoppeFake.Application.DTOs.CategoryDtos;
using ShoppeFake.Application.DTOs.ProductDtos;
using ShoppeFake.Application.DTOs.ValueDtos;
using ShoppeFake.Application.DTOs.VariantDtos;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Application.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Config Mapper Paging
            CreateMap(typeof(BasePaginatedList<>), typeof(BasePaginatedList<>))
                .ConvertUsing(typeof(BasePaginatedListConverter<,>));

            //
            CreateMap<Account, AccountResponse>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<Domain.Entities.Attribute, AttributeResponse>();
            CreateMap<AttributeValue, ValueResponse>();
            CreateMap<AttributeValue, ValueResponseV1>();
            CreateMap<ProductVariant, VariantResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.VariantAttributes, opt => opt.MapFrom(src => src.VariantAttributeValues));
            CreateMap<VariantAttributeValue, VariantAttributeValueResponse>()
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.Attribute.Name))
                .ForMember(dest => dest.AttributeCode, opt => opt.MapFrom(src => src.Attribute.Code))
                .ForMember(dest => dest.Values, opt => opt.MapFrom(src => src.Attribute.AttributeValues));


        }
        public class BasePaginatedListConverter<TSource, TDestination> : ITypeConverter<BasePaginatedList<TSource>, BasePaginatedList<TDestination>>
        {
            public BasePaginatedList<TDestination> Convert(
                BasePaginatedList<TSource> source,
                BasePaginatedList<TDestination> destination,
                ResolutionContext context)
            {
                var mappedItems = context.Mapper.Map<List<TDestination>>(source.Items);

                return new BasePaginatedList<TDestination>(
                    mappedItems,
                    source.TotalItems,
                    source.PageIndex,
                    source.PageSize
                );
            }
        }
    }
}
