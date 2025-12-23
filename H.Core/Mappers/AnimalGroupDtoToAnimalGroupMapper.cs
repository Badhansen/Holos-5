using AutoMapper;
using H.Core.Factories.Animals;
using H.Core.Models.Animals;

namespace H.Core.Mappers;

public class AnimalGroupDtoToAnimalGroupMapper : Profile
{
    public AnimalGroupDtoToAnimalGroupMapper()
    {
        CreateMap<IAnimalGroupDto, AnimalGroup>();
    }
}