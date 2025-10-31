using AutoMapper;
using H.Core.Factories;
using H.Core.Factories.Crops;
using H.Core.Models.LandManagement.Fields;

namespace H.Core.Mappers;

public class CropDtoToCropViewItemMapper : Profile
{
    public CropDtoToCropViewItemMapper()
    {
        CreateMap<ICropDto, CropViewItem>()
            .ForMember(destinationMember: cropViewItem => cropViewItem.Yield, memberOptions: options => options.MapFrom(cropDto => cropDto.WetYield));
    }
}