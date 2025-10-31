using AutoMapper;
using H.Core.Factories;
using H.Core.Models.Animals;

namespace H.Core.Mappers;

public class ManagementPeriodToManagementPeriodDtoMapper : Profile
{
    public ManagementPeriodToManagementPeriodDtoMapper()
    {
        CreateMap<ManagementPeriod, ManagementPeriodDto>();
    }
}