using AutoMapper;
using H.Core.Factories;
using H.Core.Factories.Fields;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Mappers;

public class FieldComponentToDtoMapper : Profile
{
    public FieldComponentToDtoMapper()
    {
        CreateMap<FieldSystemComponent, FieldSystemComponentDto>();
    }
}