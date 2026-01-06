using AutoMapper;
using H.Core.Factories;
using H.Core.Factories.Animals;
using H.Core.Models.Animals;

namespace H.Core.Mappers;

public class ManagementPeriodDtoToManagementPeriodMapper : Profile
{
    public ManagementPeriodDtoToManagementPeriodMapper()
    {
        CreateMap<ManagementPeriodDto, ManagementPeriod>();
    }
}