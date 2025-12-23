using AutoMapper;
using H.Core.Factories.Animals;

namespace H.Core.Mappers;

public class AnimalGroupDtoToAnimalGroupDtoMapper : Profile
{
    public AnimalGroupDtoToAnimalGroupDtoMapper()
    {
        CreateMap<IAnimalGroupDto, IAnimalGroupDto>();
    }
}