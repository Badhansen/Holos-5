using AutoMapper;
using Avalonia.Markup.Xaml.Templates;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Converters;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using Prism.Ioc;

namespace H.Core.Factories;

/// <summary>
/// A class used to create new <see cref="FieldSystemComponentDto"/> instances. The class will provide basic initialization of a new instance before returning the result to the caller.
/// </summary>
public class FieldFactory : IFieldFactory
{
    #region Fields

    private readonly IMapper _fieldComponentToDtoMapper;
    private readonly IMapper _fieldDtoToDtoMapper;

    #endregion

    #region Constructors

    public FieldFactory(IContainerProvider containerProvider)
    {
        if (containerProvider == null)
        {
            throw new ArgumentNullException(nameof(containerProvider));
        }

        _fieldComponentToDtoMapper = containerProvider.Resolve<IMapper>(nameof(FieldComponentToDtoMapper));
        _fieldDtoToDtoMapper = containerProvider.Resolve<IMapper>(nameof(FieldDtoToFieldDtoMapper));
    }

    #endregion

    #region Public Methods

    public IDto CreateDtoFromDtoTemplate(IDto template)
    {
        var fieldComponentDto = new FieldSystemComponentDto();

        _fieldDtoToDtoMapper.Map(template, fieldComponentDto);

        return fieldComponentDto;
    }

    /// <summary>
    /// Create a new instance with no additional configuration to a default instance.
    /// </summary>
    public FieldSystemComponentDto Create()
    {
        return new FieldSystemComponentDto();
    }

    public FieldSystemComponentDto CreateDto()
    {
        return new FieldSystemComponentDto();
    }

    public FieldSystemComponentDto CreateDto(Farm farm)
    {
        return new FieldSystemComponentDto();
    }

    public FieldSystemComponentDto CreateFieldDto(IFieldComponentDto template)
    {
        var fieldComponentDto = new FieldSystemComponentDto();

        _fieldDtoToDtoMapper.Map(template, fieldComponentDto);

        return fieldComponentDto;
    }

    #endregion

    #region Private Methods

    #endregion
}