using AutoMapper;
using H.Core.Factories;
using H.Core.Factories.Animals;

namespace H.Core.Mappers;

public class ManagementPeriodDtoToManagementPeriodDtoMapper : Profile
{
    public ManagementPeriodDtoToManagementPeriodDtoMapper()
    {
        CreateMap<ManagementPeriodDto, ManagementPeriodDto>();
    }
}