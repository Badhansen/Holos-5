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
            .ForMember(dest => dest.MeanDailyEvapotranspiration, opt => opt.MapFrom(src => src.MeanDailyPET))
            .ForMember(dest => dest.MeanDailyPrecipitation, opt => opt.MapFrom(src => src.MeanDailyPrecipitation))
            .ForMember(dest => dest.Latitude, opt => opt.Ignore())
            .ForMember(dest => dest.Longitude, opt => opt.Ignore())
            .ForMember(dest => dest.MonthlyPPT, opt => opt.Ignore());
    }
}