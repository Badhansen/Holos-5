using AutoMapper;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Converters;
using H.Core.Factories;
using H.Core.Factories.Crops;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using Microsoft.Extensions.Logging;
using Prism.Ioc;

namespace H.Core.Services.LandManagement.Fields;

/// <summary>
/// A general service class to assist with various operations needing to be done on a <see cref="FieldSystemComponent"/> or <see cref="FieldSystemComponentDto"/>
/// </summary>
public class FieldComponentService : ComponentServiceBase, IFieldComponentService
{
    #region Fields
    
    private readonly IFieldFactory _fieldFactory;
    private readonly ICropFactory _cropFactory;

    private readonly ITransferService<CropViewItem, CropDto> _cropTransferService;
    private readonly ITransferService<FieldSystemComponent, FieldSystemComponentDto> _fieldTransferService;

    #endregion

    #region Constructors

    public FieldComponentService(IFieldFactory fieldFactory, ICropFactory cropFactory, ILogger logger, ITransferService<CropViewItem, CropDto> cropTransferService, ITransferService<FieldSystemComponent, FieldSystemComponentDto> fieldTransferService) : base(logger)
    {
        if (fieldTransferService != null)
        {
            _fieldTransferService = fieldTransferService;
        }
        else
        {
            throw new ArgumentNullException(nameof(fieldTransferService));
        }

        if (cropTransferService != null)
        {
            _cropTransferService = cropTransferService;
        }
        else
        {
            throw new ArgumentNullException(nameof(cropTransferService));
        }

        if (cropFactory != null)
        {
            _cropFactory = cropFactory;
        }
        else
        {
            throw new ArgumentNullException(nameof(cropFactory));
        }

        if (fieldFactory != null)
        {
            _fieldFactory = fieldFactory;
        }
        else
        {
            throw new ArgumentNullException(nameof(fieldFactory));
        }
    }

    #endregion

    #region Public Methods

    public void InitializeFieldSystemComponent(Farm farm, FieldSystemComponent fieldSystemComponent)
    {
        if (fieldSystemComponent.IsInitialized)
        {
            // The field has already been initialized - do not overwrite with default values
            return;
        }

        fieldSystemComponent.Name = base.GetUniqueComponentName(farm, fieldSystemComponent);

        fieldSystemComponent.IsInitialized = true;
    }

    public void InitializeCropDto(IFieldComponentDto fieldComponentDto, ICropDto cropDto)
    {
        cropDto.Year = this.GetNextCropYear(fieldComponentDto);

        fieldComponentDto.CropDtos.Add(cropDto);
    }

    public int GetNextCropYear(IFieldComponentDto fieldComponentDto)
    {
        var result = DateTime.Now.Year;

        if (fieldComponentDto.CropDtos.Any())
        {
            result = fieldComponentDto.CropDtos.Min(dto => dto.Year) - 1;
        }

        return result;
    }

    public void ResetAllYears(IEnumerable<ICropDto> cropDtos)
    {
        if (cropDtos.Any())
        {
            var maximumYear = cropDtos.Max(dto => dto.Year);

            var orderedDtos = cropDtos.OrderByDescending(dto => dto.Year);
            for (int i = 0; i < cropDtos.Count(); i++)
            {
                var dto = orderedDtos.ElementAt(i);
                dto.Year = maximumYear - i;
            }
        }
    }

    //public IFieldComponentDto Create(FieldSystemComponent template)
    //{
    //    IFieldComponentDto fieldDto;

    //    if (template.IsInitialized)
    //    {
    //        fieldDto = TransferToFieldComponentDto(template: template);
    //    }
    //    else
    //    {
    //        fieldDto = _fieldFactory.CreateDto();
    //        template.IsInitialized = true;
    //    }

    //    return fieldDto;
    //}

    public ICropDto TransferCropViewItemToCropDto(CropViewItem cropViewItem)
    {
        return _cropTransferService.TransferDomainObjectToDto(cropViewItem);
    }

    public CropViewItem TransferCropDtoToSystem(ICropDto cropDto, CropViewItem cropViewItem)
    {
        return _cropTransferService.TransferDtoToDomainObject((CropDto) cropDto, cropViewItem);
    }

    public FieldSystemComponent TransferFieldDtoToSystem(FieldSystemComponentDto fieldComponentDto,
        FieldSystemComponent fieldSystemComponent)
    {
        return _fieldTransferService.TransferDtoToDomainObject(fieldComponentDto, fieldSystemComponent);
    }

    public IFieldComponentDto TransferToFieldComponentDto(FieldSystemComponent template)
    {
        var fieldComponentDto = _fieldTransferService.TransferDomainObjectToDto(template);

        this.ConvertCropViewItemsToDtoCollection(template, fieldComponentDto);

        return fieldComponentDto;
    }

    public void ConvertCropViewItemsToDtoCollection(FieldSystemComponent fieldSystemComponent, IFieldComponentDto fieldComponentDto)
    {
        fieldComponentDto.CropDtos.Clear();

        foreach (var cropViewItem in fieldSystemComponent.CropViewItems)
        {
            var dto = _cropFactory.CreateCropDto(template: cropViewItem);

            fieldComponentDto.CropDtos.Add(dto);
        }
    }

    public void ConvertCropDtoCollectionToCropViewItemCollection(FieldSystemComponent fieldSystemComponent, IFieldComponentDto fieldComponentDto)
    {
        foreach (var cropDto in fieldComponentDto.CropDtos)
        {
            var viewItem = fieldSystemComponent.CropViewItems.SingleOrDefault(viewItem => viewItem.Guid.Equals(cropDto.Guid));
            if (viewItem != null)
            {
                this.TransferCropDtoToSystem(cropDto, viewItem);
            }
        }
    }

    public void AddCropDtoToSystem(FieldSystemComponent fieldSystemComponent, ICropDto cropDto)
    {
        var cropViewItem = _cropFactory.CreateCropViewItem(cropDto);

        fieldSystemComponent.CropViewItems.Add(cropViewItem);
    }

    public void RemoveCropFromSystem(FieldSystemComponent fieldSystemComponent, ICropDto cropDto)
    {
        if (cropDto != null)
        {
            // By default, all DTO objects will have their GUID property set to be equal to the GUID of the associated domain object
            var cropViewItem = fieldSystemComponent.CropViewItems.SingleOrDefault(x => x.Guid.Equals(cropDto.Guid));
            if (cropViewItem != null)
            {
                fieldSystemComponent.CropViewItems.Remove(cropViewItem);
            }
        }
    }

    public CropViewItem GetCropViewItemFromDto(ICropDto cropDto, FieldSystemComponent fieldSystemComponent)
    {
        return fieldSystemComponent.CropViewItems.SingleOrDefault(x => x.Guid.Equals(cropDto.Guid));
    }

    #endregion

    #region Private Methods

    #endregion
}