using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using H.Avalonia.Views.ComponentViews;
using H.Avalonia.Views.ComponentViews.LandManagement;
using H.Avalonia.Views.ComponentViews.LandManagement.Field;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Factories.Crops;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Plants;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement.Field;

/// <summary>
/// The view model that is used with a <see cref="FieldComponentView"/>.
/// </summary>
public class FieldComponentViewModel : ViewModelBase
{
    #region Fields

    /// <summary>
    /// The selected field
    /// </summary>
    private FieldSystemComponent _selectedFieldSystemComponent;

    /// <summary>
    /// The field DTO that is bound to the view and is based on the values from the <see cref="_selectedFieldSystemComponent"/> model object
    /// </summary>
    private IFieldComponentDto _selectedFieldSystemComponentDto;

    /// <summary>
    /// The selected crop
    /// </summary>
    private CropViewItem _selectedCropViewItem;

    /// <summary>
    /// The crop DTO that is bound to the view and is based on the values from the <see cref="_selectedCropViewItem"/>
    /// </summary>
    private ICropDto _selectedCropDto;

    /// <summary>
    /// A service class to perform domain/business logic on field and crop DTOs/objects
    /// </summary>
    private readonly IFieldComponentService _fieldComponentService;

    /// <summary>
    /// A logger instance
    /// </summary>
    private readonly ILogger _logger;

    private readonly ICropFactory _cropFactory;

    #endregion

    #region Constructors

    private void Construct()
    {
    }

    public FieldComponentViewModel()
    {
        this.Construct();
    }

    public FieldComponentViewModel(
        IRegionManager regionManager, 
        IEventAggregator eventAggregator, 
        IStorageService storageService,
        IFieldComponentService fieldComponentService,
        ILogger logger,
        ICropFactory cropFactory) : base(regionManager, eventAggregator, storageService, logger)
    {
        if (cropFactory != null)
        {
            _cropFactory = cropFactory; 
        }
        else
        {
            throw new ArgumentNullException(nameof(cropFactory));
        }

        if (logger != null)
        {
            _logger = logger;
        }
        else
        {
            throw new ArgumentNullException(nameof(logger));
        }

        if (fieldComponentService != null)
        {
            _fieldComponentService = fieldComponentService;
        }
        else
        {
            throw new ArgumentNullException(nameof(fieldComponentService));
        }

        this.Construct();

        this.AddCropCommand = new DelegateCommand<object>(OnAddCropExecute, AddCropCanExecute);
        this.RemoveCropCommand = new DelegateCommand<object>(OnRemoveCropExecute, RemoveCropCanExecute);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Responsible for handling the addition of new crops
    /// </summary>
    public DelegateCommand<object> AddCropCommand { get; set; }

    /// <summary>
    /// Responsible for handling the removal of crops
    /// </summary>
    public DelegateCommand<object> RemoveCropCommand { get; set; }

    /// <summary>
    /// The selected <see cref="SelectedFieldSystemComponentDto"/>
    /// </summary>
    public IFieldComponentDto SelectedFieldSystemComponentDto
    {
        get => _selectedFieldSystemComponentDto;
        set => SetProperty(ref _selectedFieldSystemComponentDto, value);
    }

    /// <summary>
    /// The selected <see cref="CropDto"/>
    /// </summary>
    public ICropDto SelectedCropDto
    {
        get => _selectedCropDto;
        set => SetProperty(ref _selectedCropDto, value);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// When the user navigates to a <see cref="FieldSystemComponent"/>, we must initialize the component and any DTOs
    /// that will be used with the view
    /// </summary>
    /// <param name="component">The <see cref="FieldSystemComponent"/> to display to the user</param>
    public override void InitializeViewModel(ComponentBase component)
    {
        if (component is FieldSystemComponent fieldSystemComponent)
        {
            base.InitializeViewModel(fieldSystemComponent);

            this.PropertyChanged += OnPropertyChanged;

            // Hold a reference to the selected field system object
            _selectedFieldSystemComponent = fieldSystemComponent;

            // Build a DTO to represent the model/domain object
            var fieldComponentDto = _fieldComponentService.TransferToFieldComponentDto(_selectedFieldSystemComponent);

            // Listen for changes on the DTO
            fieldComponentDto.PropertyChanged += FieldSystemComponentDtoOnPropertyChanged;

            // Assign the DTO to the property that is bound to the view
            this.SelectedFieldSystemComponentDto = fieldComponentDto;

            // If there are any crops associated with the field, select the first one by default
            if (this.SelectedFieldSystemComponentDto.CropDtos.Any())
            {
                // Check if we can restore last selected item
                if (this.SelectedFieldSystemComponentDto.CropDtos.Contains(this.SelectedCropDto))
                {
                    this.SelectedCropDto = this.SelectedCropDto;
                }
                else
                {
                    this.SelectedCropDto = this.SelectedFieldSystemComponentDto.CropDtos.First();
                }
            }
            else
            {
                // There are no crops associated with this field, add a new one
                this.AddCropDto();
            }

            // Hold a reference to the selected crop view item
            _selectedCropViewItem = _fieldComponentService.GetCropViewItemFromDto(this.SelectedCropDto, _selectedFieldSystemComponent);
        }
    }

    /// <summary>
    /// A first point of entry to this class (after the constructor is called). Get a reference to the <see cref="FieldSystemComponent"/> the
    /// user selected from the <see cref="MyComponentsView"/>.
    /// </summary>
    /// <param name="navigationContext">An object holding a reference to the selected <see cref="FieldSystemComponent"/></param>
    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.ContainsKey(GuiConstants.ComponentKey))
        {
            var parameter = navigationContext.Parameters[GuiConstants.ComponentKey];
            if (parameter is FieldSystemComponent fieldSystemComponent)
            {
                this.InitializeViewModel(fieldSystemComponent);
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// A user can add a crop under any condition
    /// </summary>
    private bool AddCropCanExecute(object arg)
    {
        return true;
    }

    /// <summary>
    /// Adds a new <see cref="CropDto"/> to the <see cref="SelectedFieldSystemComponentDto"/> property
    /// </summary>
    private void OnAddCropExecute(object obj)
    {
        this.AddCropDto();
    }

    /// <summary>
    /// Some property on the <see cref="SelectedFieldSystemComponentDto"/> has changed. Check if we need to validate any user
    /// input before assigning the value on to the associated <see cref="FieldSystemComponent"/>
    /// </summary>
    private void FieldSystemComponentDtoOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is FieldSystemComponentDto fieldSystemComponentDto)
        {
            /*
             * Before assigning values from the bound DTOs, check for any validation errors. If there are any validation errors
             * we should not proceed with the transfer of user input from the DTO to the model until the validation errors are fixed
             */

            if (!fieldSystemComponentDto.HasErrors)
            {
                // A property on the DTO has been changed by the user, assign the new value to the system object after any unit conversion (if necessary)
                _fieldComponentService.TransferFieldDtoToSystem(fieldSystemComponentDto, _selectedFieldSystemComponent);
            }
        }
    }

    /// <summary>
    /// Some property on the <see cref="SelectedCropDto"/> has changed. Check if we need to validate any user
    /// input before assigning the value on to the associated <see cref="CropViewItem"/>
    /// </summary>
    private void CropDtoOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is CropDto cropDto)
        {
            /*
             * Before assigning values from the bound DTOs, check for any validation errors. If there are any validation errors
             * we should not proceed with the transfer of user input from the DTO to the model until the validation errors are fixed
             */

            if (!cropDto.HasErrors)
            {
                var viewItem = _fieldComponentService.GetCropViewItemFromDto(cropDto, _selectedFieldSystemComponent);

                // Persist the changes to the system
                _fieldComponentService.TransferCropDtoToSystem(cropDto, viewItem);
            }
        }
    }

    /// <summary>
    /// Used to indicate to the GUI if the command button should be enabled or disabled
    /// </summary>
    private bool RemoveCropCanExecute(object arg)
    {
        return this.SelectedFieldSystemComponentDto != null && this.SelectedFieldSystemComponentDto.CropDtos.Any();
    }

    private void OnRemoveCropExecute(object obj)
    {
        if (this.SelectedCropDto != null)
        {
            // Keep a reference to the dto to remove before removing it from the collection
            var dtoToRemove = this.SelectedCropDto;

            this.SelectedFieldSystemComponentDto.CropDtos.Remove(dtoToRemove);

            // Ensure consecutive ordering (by year) of all crops now that one has been removed
            _fieldComponentService.ResetAllYears(this.SelectedFieldSystemComponentDto.CropDtos);

            this.RemoveCropCommand.RaiseCanExecuteChanged();

            _fieldComponentService.RemoveCropFromSystem(_selectedFieldSystemComponent, dtoToRemove);
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(SelectedCropDto)))
        {
            RemoveCropCommand.RaiseCanExecuteChanged();

            if (this.SelectedCropDto != null)
            {
                this.SelectedCropDto.PropertyChanged -= CropDtoOnPropertyChanged;
                this.SelectedCropDto.PropertyChanged += CropDtoOnPropertyChanged;

                _selectedCropViewItem = _fieldComponentService.GetCropViewItemFromDto(this.SelectedCropDto, _selectedFieldSystemComponent);
            }
        }
    }

    /// <summary>
    /// Adds a new <see cref="CropDto"/> to the <see cref="SelectedFieldSystemComponentDto"/> property
    /// Used in both the <see cref="OnAddCropExecute(object)"/> and in the <see cref="InitializeViewModel(ComponentBase)"/> methods
    /// </summary>
    private void AddCropDto()
    {
        CropDto dto = _cropFactory.CreateDto(base.ActiveFarm);

        _fieldComponentService.InitializeCropDto(this.SelectedFieldSystemComponentDto, dto);

        // Use this as the new selected instance
        this.SelectedCropDto = dto;

        // If disabled before, enable this command now so that the user can remove a DTO
        this.RemoveCropCommand.RaiseCanExecuteChanged();

        _fieldComponentService.AddCropDtoToSystem(_selectedFieldSystemComponent, dto);

        _selectedCropViewItem = _fieldComponentService.GetCropViewItemFromDto(dto, _selectedFieldSystemComponent);
    }

    #endregion
}