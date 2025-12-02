using AutoMapper;
using H.Core.Models.Climate;

namespace H.Core.Mappers;

public class DailyClimateDtoToDailyClimateDtoMapper : Profile
{
    public DailyClimateDtoToDailyClimateDtoMapper()
    {
        CreateMap<DailyClimateDto, DailyClimateDto>();
    }
}