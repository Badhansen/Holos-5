using AutoMapper;
using H.Core.Models.Climate;
using H.Core.Providers.Climate;

namespace H.Core.Mappers;

public class DailyClimateDtoToDailyClimateDataMapper : Profile
{
    public DailyClimateDtoToDailyClimateDataMapper()
    {
        CreateMap<DailyClimateDto, DailyClimateData>()
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
            .ForMember(dest => dest.MeanDailyPET, opt => opt.MapFrom(src => src.TotalPET))
            .ForMember(dest => dest.MeanDailyPrecipitation, opt => opt.MapFrom(src => src.TotalPPT))
            .ForMember(dest => dest.JulianDay, opt => opt.Ignore())
            .ForMember(dest => dest.MeanDailyAirTemperature, opt => opt.Ignore())
            .ForMember(dest => dest.RelativeHumidity, opt => opt.Ignore())
            .ForMember(dest => dest.SolarRadiation, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.Ignore());
    }
}