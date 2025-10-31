using AutoMapper;
using H.Core.Factories;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Mappers;

public class FieldDtoToFieldComponentMapper : Profile
{
    public FieldDtoToFieldComponentMapper()
    {
        CreateMap<IFieldComponentDto, FieldSystemComponent>();
    }
}