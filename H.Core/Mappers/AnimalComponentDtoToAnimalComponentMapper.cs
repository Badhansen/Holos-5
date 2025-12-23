using AutoMapper;
using H.Core.Factories;
using H.Core.Factories.Animals;
using H.Core.Models.Animals;

namespace H.Core.Mappers;

public class AnimalComponentDtoToAnimalComponentMapper : Profile
{
    public AnimalComponentDtoToAnimalComponentMapper()
    {
        CreateMap<IAnimalComponentDto, AnimalComponentBase>();
    }
}