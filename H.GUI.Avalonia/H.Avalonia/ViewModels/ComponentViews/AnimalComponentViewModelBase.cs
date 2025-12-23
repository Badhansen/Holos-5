using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DryIoc;
using H.Core.Factories;
using H.Core.Factories.Animals;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ComponentViews;

public abstract class AnimalComponentViewModelBase : ViewModelBase
{
    #region Fields

    /// <summary>
    /// The selected animal component
    /// </summary>
    private AnimalComponentBase _selectedAnimalComponent;

    /// <summary>
    /// The selected management period
    /// </summary>
    private ManagementPeriod _selectedManagementPeriod;

    protected IAnimalComponentService AnimalComponentService;

    private ObservableCollection<ManagementPeriodDto> _managementPeriodDtos;

    /// <summary>
    /// An animal component DTO that is bound to the view and is based on the values from the <see cref="_selectedAnimalComponent"/> model object.
    /// </summary>
    private IAnimalComponentDto _selectedAnimalComponentDto;

    #endregion

    #region Constructors

    protected AnimalComponentViewModelBase()
    {
        this.Construct();
    }

    protected AnimalComponentViewModelBase(
        IAnimalComponentService animalComponentService, 
        ILogger logger,
        IStorageService storageService, 
        IManagementPeriodService managementPeriodService) : base(storageService, logger)
    {
        if (animalComponentService != null)
        {
            AnimalComponentService = animalComponentService; 
        }
        else
        {
            throw new ArgumentNullException(nameof(animalComponentService));
        }

        this.Construct();
    }

    #endregion

    #region Properties

    protected IAnimalComponentDto SelectedAnimalComponentDto
    {
        get => _selectedAnimalComponentDto;
        set => SetProperty(ref _selectedAnimalComponentDto, value);
    }

    /// <summary>
    /// An Observable Collection that holds <see cref="ManagementPeriodDto"/> objects, bound to a DataGrid in the view(s).
    /// </summary>
    public ObservableCollection<ManagementPeriodDto> ManagementPeriodDtos
    {
        get => _managementPeriodDtos;
        set => SetProperty(ref _managementPeriodDtos, value);
    }

    #endregion

    #region Public Methods

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);

        if (navigationContext.Parameters.ContainsKey(GuiConstants.ComponentKey))
        {
            var parameter = navigationContext.Parameters[GuiConstants.ComponentKey];
            if (parameter is AnimalComponentBase animalComponent)
            {
                this.InitializeViewModel(animalComponent);
            }
        }
    }

    /// <summary>
    /// When the user navigates to a <see cref="AnimalComponentBase"/>, we must initialize the component and any DTOs
    /// that will be used with the view
    /// </summary>
    /// <param name="component">The <see cref="AnimalComponentBase"/> to display to the user</param>
    public override void InitializeViewModel(ComponentBase component)
    {
        if (component is not AnimalComponentBase animalComponentBase)
        {
            return;
        }

        base.InitializeViewModel(component);

        this.PropertyChanged += OnPropertyChanged;

        this.InitializeAnimalComponent(animalComponentBase);

        // Build a DTO to represent the model/domain object
        var dto = this.AnimalComponentService.TransferToAnimalComponentDto(animalComponentBase);

        this.SelectedAnimalComponentDto = dto;

        dto.PropertyChanged += OnAnimalComponentDtoPropertyChanged;
    }

    protected void InitializeAnimalComponent(AnimalComponentBase animalComponent)
    {
        if (animalComponent == null)
        {
            return;
        }

        // Hold a reference to the selected animal component
        _selectedAnimalComponent = animalComponent;

        // Build a DTO to represent the model/domain object
        var animalComponentDto = this.AnimalComponentService.TransferToAnimalComponentDto(_selectedAnimalComponent);

        // Listen for changes on the DTO
        animalComponentDto.PropertyChanged += OnAnimalComponentDtoPropertyChanged;

        // Assign the DTO to the property bound to the view
        this.SelectedAnimalComponentDto = animalComponentDto;
    }

    private void OnAnimalComponentDtoPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is IAnimalComponentDto dto)
        {
            // A property on the DTO has been changed by the user, assign the new value to the system object after any unit conversion (if necessary)
            this.AnimalComponentService.TransferAnimalComponentDtoToSystem((AnimalComponentDto) dto, _selectedAnimalComponent);
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    #endregion



    #region Event Handlers

    /// <summary>
    ///  bound to a button in the view, adds an item to the <see cref="ManagementPeriodDtos"/> collection / a row to the respective bound DataGrid. Seeded with some default values.
    /// </summary>
    public void HandleAddManagementPeriodEvent()
    {
        int numPeriods = ManagementPeriodDtos.Count;
        var newManagementPeriodViewModel = new ManagementPeriodDto { Name = $"Period #{numPeriods}", Start = new DateTime(2024, 01, 01), End = new DateTime(2025, 01, 01), NumberOfDays = 364 };
        ManagementPeriodDtos.Add(newManagementPeriodViewModel);
    }

    #endregion

    #region Private Methods

    private void Construct()
    {
        this.ManagementPeriodDtos = new ObservableCollection<ManagementPeriodDto>();
    }

    #endregion
}