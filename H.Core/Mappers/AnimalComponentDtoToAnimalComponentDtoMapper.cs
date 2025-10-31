using AutoMapper;
using H.Core.Factories;

namespace H.Core.Mappers;

public class AnimalComponentDtoToAnimalComponentDtoMapper : Profile
{
    
    public AnimalComponentDtoToAnimalComponentDtoMapper()
    {
        CreateMap<IAnimalComponentDto, IAnimalComponentDto>();
    }
}