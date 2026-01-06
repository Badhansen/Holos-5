using AutoMapper;
using H.Core.Factories;
using H.Core.Factories.Animals;

namespace H.Core.Mappers;

public class AnimalComponentDtoToAnimalComponentDtoMapper : Profile
{
    
    public AnimalComponentDtoToAnimalComponentDtoMapper()
    {
        CreateMap<IAnimalComponentDto, IAnimalComponentDto>();
    }
}