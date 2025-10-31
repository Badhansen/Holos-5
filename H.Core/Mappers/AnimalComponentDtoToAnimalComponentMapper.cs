using AutoMapper;
using H.Core.Factories;
using H.Core.Models.Animals;

namespace H.Core.Mappers;

public class AnimalComponentDtoToAnimalComponentMapper : Profile
{
    public AnimalComponentDtoToAnimalComponentMapper()
    {
        CreateMap<IAnimalComponentDto, AnimalComponentBase>();
    }
}