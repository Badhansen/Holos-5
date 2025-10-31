using AutoMapper;
using H.Core.Enumerations;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Initialization;
using Prism.Ioc;

namespace H.Core.Factories.Crops;

/// <summary>
/// A class used to create new <see cref="CropDto"/> and <see cref="CropViewItem"/> instances. The class will provide basic initialization of a new instance before returning the result to the caller.
/// </summary>
public class CropFactory : ICropFactory
{
    #region Fields

    private readonly IMapper _cropViewItemToDtoMapper;
    private readonly IMapper _cropDtoToDtoMapper;
    private readonly IMapper _cropDtoToViewItemMapper;

    private readonly ICropInitializationService _cropInitializationService;

    #endregion

    #region Constructors

    public CropFactory(ICropInitializationService cropInitializationService, IContainerProvider containerProvider)
    {
        if (cropInitializationService != null)
        {
            _cropInitializationService = cropInitializationService; 
        }
        else
        {
            throw new ArgumentNullException(nameof(cropInitializationService));
        }
        
        _cropViewItemToDtoMapper = containerProvider.Resolve<IMapper>(nameof(CropViewItemToCropDtoMapper));
        _cropDtoToDtoMapper = containerProvider.Resolve<IMapper>(nameof(CropDtoToCropDtoMapper));
        _cropDtoToViewItemMapper = containerProvider.Resolve<IMapper>(nameof(CropDtoToCropViewItemMapper));
    }

    #endregion

    #region Public Methods

    public CropDto CreateDto()
    {
        return new CropDto();
    }

    public CropDto CreateDto(Farm farm)
    {
        var cropViewItem = new CropViewItem();

        cropViewItem.CropType = CropType.Wheat;
        cropViewItem.Year = DateTime.Now.Year;
        cropViewItem.Name = $"{cropViewItem.CropType} {cropViewItem.Year}";

        _cropInitializationService.Initialize(cropViewItem, farm);

        var dto = this.CreateCropDto(cropViewItem);

        return dto;
    }

    public IDto CreateDtoFromDtoTemplate(IDto template)
    {
        var cropDto = new CropDto();

        _cropDtoToDtoMapper.Map(template, cropDto);

        return cropDto;
    }

    public CropDto CreateCropDto(CropViewItem template)
    {
        var cropDto = new CropDto();

        _cropViewItemToDtoMapper.Map(template, cropDto);

        return cropDto;
    }

    public CropViewItem CreateCropViewItem(ICropDto  cropDto)
    {
        var viewItem = new CropViewItem();

        _cropDtoToViewItemMapper.Map(cropDto, viewItem);

        return viewItem;
    }

    #endregion
}