using System;
using AutoMapper;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.Climate;
using H.Core.Providers.Climate;
using Prism.Ioc;

namespace H.Core.Factories.Climate;

/// <summary>
/// A class used to create new <see cref="DailyClimateDto"/> and <see cref="DailyClimateData"/> instances. The class will provide basic initialization of a new instance before returning the result to the caller.
/// </summary>
public class DailyClimateDataFactory : IDailyClimateDataFactory
{
    #region Fields

    private readonly IMapper _dailyClimateDataToDtoMapper;
    private readonly IMapper _dailyClimateDtoToDtoMapper;
    private readonly IMapper _dailyClimateDtoToDataMapper;

    #endregion

    #region Constructors

    public DailyClimateDataFactory(IContainerProvider containerProvider)
    {
        if (containerProvider != null)
        {
            _dailyClimateDataToDtoMapper = containerProvider.Resolve<IMapper>(nameof(DailyClimateDataToDailyClimateDtoMapper));
            _dailyClimateDtoToDtoMapper = containerProvider.Resolve<IMapper>(nameof(DailyClimateDtoToDailyClimateDtoMapper));
            _dailyClimateDtoToDataMapper = containerProvider.Resolve<IMapper>(nameof(DailyClimateDtoToDailyClimateDataMapper));
        }
        else
        {
            throw new ArgumentNullException(nameof(containerProvider));
        }
    }

    #endregion

    #region Public Methods

    public DailyClimateDto CreateDto()
    {
        return new DailyClimateDto();
    }

    public DailyClimateDto CreateDto(Farm farm)
    {
        // Create a basic DTO with default climate data for the farm
        var dto = new DailyClimateDto
        {
            Year = DateTime.Now.Year
        };

        return dto;
    }

    public IDto CreateDtoFromDtoTemplate(IDto template)
    {
        var dailyClimateDto = new DailyClimateDto();

        _dailyClimateDtoToDtoMapper.Map(template, dailyClimateDto);

        return dailyClimateDto;
    }

    public DailyClimateDto CreateDto(DailyClimateData dailyClimateData)
    {
        var dto = new DailyClimateDto();

        _dailyClimateDataToDtoMapper.Map(dailyClimateData, dto);

        return dto;
    }

    public DailyClimateData CreateData(DailyClimateDto dailyClimateDto)
    {
        var data = new DailyClimateData();

        _dailyClimateDtoToDataMapper.Map(dailyClimateDto, data);

        return data;
    }

    #endregion
}