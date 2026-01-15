using AutoMapper;
using H.Core.Factories;
using H.Core.Factories.Fields;

namespace H.Core.Mappers;

public class FieldDtoToFieldDtoMapper : Profile
{
    public FieldDtoToFieldDtoMapper()
    {
        CreateMap<FieldSystemComponentDto, FieldSystemComponentDto>();
    }
}