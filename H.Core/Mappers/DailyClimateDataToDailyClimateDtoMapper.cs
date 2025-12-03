using AutoMapper;
using H.Core.Models.Climate;
using H.Core.Providers.Climate;

namespace H.Core.Mappers;

public class DailyClimateDataToDailyClimateDtoMapper : Profile
{
    public DailyClimateDataToDailyClimateDtoMapper()
    {
        CreateMap<DailyClimateData, DailyClimateDto>()
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.TotalPET, opt => opt.MapFrom(src => src.MeanDailyPET))
            .ForMember(dest => dest.TotalPPT, opt => opt.MapFrom(src => src.MeanDailyPrecipitation))
            .ForMember(dest => dest.Latitude, opt => opt.Ignore())
            .ForMember(dest => dest.Longitude, opt => opt.Ignore())
            .ForMember(dest => dest.MonthlyPPT, opt => opt.Ignore());
    }
}