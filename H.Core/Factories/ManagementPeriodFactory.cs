using AutoMapper;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.Animals;
using Prism.Ioc;

namespace H.Core.Factories;

public class ManagementPeriodFactory : IManagementPeriodFactory
{
    #region Fields

    private readonly IMapper _managementPeriodDtoToManagementPeriodDtoMapper;
    private readonly IMapper _managementPeriodToManagementPeriodDtoMapper;
    private readonly IMapper _managementPeriodDtoToManagementPeriodMapper;

    #endregion

    #region Constructors

    public ManagementPeriodFactory(IContainerProvider containerProvider)
    {
        if (containerProvider == null)
        {
            throw new ArgumentNullException(nameof(containerProvider));
        }

        _managementPeriodDtoToManagementPeriodDtoMapper = containerProvider.Resolve<IMapper>(nameof(ManagementPeriodDtoToManagementPeriodDtoMapper));
        _managementPeriodToManagementPeriodDtoMapper = containerProvider.Resolve<IMapper>(nameof(ManagementPeriodToManagementPeriodDtoMapper));
        _managementPeriodDtoToManagementPeriodMapper = containerProvider.Resolve<IMapper>(nameof(ManagementPeriodDtoToManagementPeriodMapper));
    }

    #endregion

    #region Public Methods

    public IManagementPeriodDto CreateManagementPeriodDto()
    {
        var dto = new ManagementPeriodDto();
        dto.Name = "New Management Period";
        dto.Start = new DateTime(DateTime.Now.Year, 1, 1);
        dto.End = new DateTime(DateTime.Now.Year, 12, 31);
        dto.NumberOfDays = (dto.End - dto.Start).Days + 1;
        
        return dto;
    }

    public IManagementPeriodDto CreateManagementPeriodDto(IManagementPeriodDto template)
    {
        var dto = new ManagementPeriodDto();
        _managementPeriodDtoToManagementPeriodDtoMapper.Map(template, dto);
        return dto;
    }

    public IManagementPeriodDto CreateManagementPeriodDto(ManagementPeriod managementPeriod)
    {
        var dto = new ManagementPeriodDto();
        _managementPeriodToManagementPeriodDtoMapper.Map(managementPeriod, dto);
        return dto;
    }

    public ManagementPeriod CreateManagementPeriod(IManagementPeriodDto dto)
    {
        var managementPeriod = new ManagementPeriod();
        _managementPeriodDtoToManagementPeriodMapper.Map(dto, managementPeriod);
        return managementPeriod;
    }

    #endregion
}