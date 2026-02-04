using AutoMapper;
using H.Core.Factories.Animals.Dairy;
using H.Core.Models.Animals.Dairy;

namespace H.Core.Mappers;

/// <summary>
/// AutoMapper profile for mapping between DairyComponent domain model and DairyComponentDto
/// </summary>
public class DairyComponentToDtoMapper : Profile
{
    public DairyComponentToDtoMapper()
    {
        // Map from DairyComponent to DairyComponentDto
        CreateMap<DairyComponent, DairyComponentDto>()
            .IncludeBase<H.Core.Models.Animals.AnimalComponentBase, H.Core.Factories.Animals.AnimalComponentDto>();
        
        // Map from DairyComponentDto to DairyComponent
        CreateMap<DairyComponentDto, DairyComponent>()
            .IncludeBase<H.Core.Factories.Animals.AnimalComponentDto, H.Core.Models.Animals.AnimalComponentBase>();
    }
}
