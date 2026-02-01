using AutoMapper;
using ArticleService.Models;
using ArticleService.Models.DTOs;

namespace ArticleService.Profiles
{
    public class ArticleMappingProfile : Profile
    {
        public ArticleMappingProfile()
        {
            // Mappings pour Article
            CreateMap<ArticleCreateDto, Article>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.DateMiseEnStock,
                    opt => opt.MapFrom(src => src.DateMiseEnStock == default ? DateTime.Now : src.DateMiseEnStock));

            CreateMap<Article, ArticleResponseDto>()
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

            // Mappings pour Category
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
            CreateMap<Category, CategoryDto>();
        }
    }
}