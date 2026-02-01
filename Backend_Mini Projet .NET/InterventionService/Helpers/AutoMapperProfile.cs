using AutoMapper;
using InterventionService.Models.DTOs;
using InterventionService.Models.Entities;

namespace InterventionService.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Entity -> DTO
            CreateMap<Intervention, InterventionDTO>()
                .ForMember(dest => dest.TotalCost,
                    opt => opt.MapFrom(src => src.IsUnderWarranty ? 0 : (src.PartsCost + src.LaborCost)));

            // DTO -> Entity
            CreateMap<CreateInterventionDTO, Intervention>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<UpdateInterventionDTO, Intervention>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}