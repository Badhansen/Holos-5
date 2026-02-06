using AutoMapper;
using H.Core.Factories.Rotations;
using H.Core.Models.LandManagement.Rotation;

namespace H.Core.Mappers;

public class RotationComponentDtoToRotationComponentMapper : Profile
{
    public RotationComponentDtoToRotationComponentMapper()
    {
        CreateMap<RotationComponentDto, RotationComponent>()
            .ForMember(dest => dest.FieldSystemComponent, opt => opt.Ignore());
    }
}
