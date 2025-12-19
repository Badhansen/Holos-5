using AutoMapper;
using H.Core.Factories.Crops;

namespace H.Core.Mappers;

public class CropDtoToCropDtoMapper : Profile
{
    public CropDtoToCropDtoMapper()
    {
        CreateMap<CropDto, CropDto>();
    }
}