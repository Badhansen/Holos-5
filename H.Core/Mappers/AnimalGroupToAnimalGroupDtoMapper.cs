using AutoMapper;
using H.Core.Factories.Animals;
using H.Core.Models.Animals;

namespace H.Core.Mappers;

/// <summary>
/// AutoMapper profile for mapping from AnimalGroup domain model to AnimalGroupDto
/// </summary>
public class AnimalGroupToAnimalGroupDtoMapper : Profile
{
    public AnimalGroupToAnimalGroupDtoMapper()
    {
        // Map from domain model to DTO
        CreateMap<AnimalGroup, AnimalGroupDto>()
            .ForMember(dest => dest.GroupType, opt => opt.MapFrom(src => src.GroupType))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid));
        
        // Map from DTO to domain model
        CreateMap<AnimalGroupDto, AnimalGroup>()
            .ForMember(dest => dest.GroupType, opt => opt.MapFrom(src => src.GroupType))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid))
            .ForMember(dest => dest.ManagementPeriods, opt => opt.Ignore()); // Don't map management periods here
    }
}
