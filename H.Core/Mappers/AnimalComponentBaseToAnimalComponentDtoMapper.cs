using AutoMapper;
using H.Core.Factories;
using H.Core.Models.Animals;

namespace H.Core.Mappers;

public class AnimalComponentBaseToAnimalComponentDtoMapper : Profile
{
    public AnimalComponentBaseToAnimalComponentDtoMapper()
    {
        CreateMap<AnimalComponentBase, IAnimalComponentDto>();
    }
}