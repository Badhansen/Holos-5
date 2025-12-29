using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DryIoc;
using H.Core.Enumerations;
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
    private AnimalComponentBase? _selectedAnimalComponent;

    /// <summary>
    /// The selected management period
    /// </summary>
    private ManagementPeriod? _selectedManagementPeriod;

    private ObservableCollection<ManagementPeriodDto>? _managementPeriodDtos;

    /// <summary>
    /// An animal component DTO that is bound to the view and is based on the values from the <see cref="_selectedAnimalComponent"/> model object.
    /// </summary>
    private IAnimalComponentDto? _selectedAnimalComponentDto;

    protected IAnimalComponentService? AnimalComponentService;
    protected IManagementPeriodService? ManagementPeriodService;
    protected AnimalType _animalType;
    protected ObservableCollection<AnimalGroup> _animalGroups;

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
        this.AnimalComponentService = animalComponentService;
        this.ManagementPeriodService = managementPeriodService;

        this.Construct();
    }

    private void Construct()
    {
        ManagementPeriodDtos = new ObservableCollection<ManagementPeriodDto>();
        Groups = new ObservableCollection<AnimalGroup>();
    }

    #endregion

    #region Properties

    protected IAnimalComponentDto? SelectedAnimalComponentDto
    {
        get => _selectedAnimalComponentDto;
        set => SetProperty(ref _selectedAnimalComponentDto, value);
    }

    /// <summary>
    /// An observable collection that holds <see cref="ManagementPeriodDto"/> objects, bound to a DataGrid in the view(s).
    /// </summary>
    public ObservableCollection<ManagementPeriodDto>? ManagementPeriodDtos
    {
        get => _managementPeriodDtos;
        set => SetProperty(ref _managementPeriodDtos, value);
    }

    /// <summary>
    ///  The <see cref="H.Core.Enumerations.AnimalType"/> a respective component represents, used in the <see cref="Groups"/> collection / Groups data grid in the view(s), value set in child classes.
    /// </summary>
    public AnimalType AnimalType
    {
        get => _animalType;
        set => SetProperty(ref _animalType, value);
    }

    /// <summary>
    /// An Observable Collection that holds <see cref="AnimalGroup"/> objects, bound to a DataGrid in the view(s).
    /// </summary>
    public ObservableCollection<AnimalGroup> Groups
    {
        get => _animalGroups;
        set => SetProperty(ref _animalGroups, value);
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
        var animalComponentDto = this.AnimalComponentService?.TransferToAnimalComponentDto(animalComponentBase);

        if (animalComponentDto != null)
        {
            this.SelectedAnimalComponentDto = animalComponentDto;

            animalComponentDto.PropertyChanged += OnAnimalComponentDtoPropertyChanged;
        }
    }

    public void AddExistingManagementPeriods()
    {
        Farm currentFarm = StorageService.GetActiveFarm();
        var existingManagementPeriods = currentFarm.GetAllManagementPeriods();
        foreach (var managementPeriod in existingManagementPeriods)
        {
            var newManagementPeriodViewModel = new ManagementPeriodDto();
            newManagementPeriodViewModel.Name = managementPeriod.GroupName;
            newManagementPeriodViewModel.Start = managementPeriod.Start;
            newManagementPeriodViewModel.End = managementPeriod.End;
            newManagementPeriodViewModel.NumberOfDays = managementPeriod.NumberOfDays;
            ManagementPeriodDtos.Add(newManagementPeriodViewModel);
        }
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
        var animalComponentDto = this.AnimalComponentService?.TransferToAnimalComponentDto(_selectedAnimalComponent);
        if (animalComponentDto != null)
        {
            // Listen for changes on the DTO
            animalComponentDto.PropertyChanged += OnAnimalComponentDtoPropertyChanged;

            // Assign the DTO to the property bound to the view
            this.SelectedAnimalComponentDto = animalComponentDto;
        }
    }

    private void OnAnimalComponentDtoPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is IAnimalComponentDto dto)
        {
            // A property on the DTO has been changed by the user, assign the new value to the system object after any unit conversion (if necessary)
            this.AnimalComponentService?.TransferAnimalComponentDtoToSystem((AnimalComponentDto) dto, _selectedAnimalComponent);
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Bound to a button in the view, adds an item to the <see cref="ManagementPeriodDtos"/> collection / a row to the respective bound DataGrid. Seeded with some default values.
    /// </summary>
    public void HandleAddManagementPeriodEvent()
    {
        if (ManagementPeriodDtos != null)
        {
            int numPeriods = ManagementPeriodDtos.Count;
            var newManagementPeriodViewModel = new ManagementPeriodDto { Name = $"Period #{numPeriods}", Start = new DateTime(2024, 01, 01), End = new DateTime(2025, 01, 01), NumberOfDays = 364 };
            ManagementPeriodDtos.Add(newManagementPeriodViewModel);
        }
    }

    /// <summary>
    /// Bound to a button in the view, adds an item to the <see cref="AnimalComponentViewModelBase.Groups"/> collection / a row to the respective bound DataGrid. Seeded with <see cref="AnimalType"/>.
    /// </summary>
    public void HandleAddGroupEvent()
    {
        Groups.Add(new AnimalGroup { GroupType = AnimalType });
    }

    #endregion

    #region Private Methods

    #endregion

}