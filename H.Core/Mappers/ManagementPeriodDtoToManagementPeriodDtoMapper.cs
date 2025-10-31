using AutoMapper;
using H.Core.Factories;

namespace H.Core.Mappers;

public class ManagementPeriodDtoToManagementPeriodDtoMapper : Profile
{
    public ManagementPeriodDtoToManagementPeriodDtoMapper()
    {
        CreateMap<ManagementPeriodDto, ManagementPeriodDto>();
    }
}