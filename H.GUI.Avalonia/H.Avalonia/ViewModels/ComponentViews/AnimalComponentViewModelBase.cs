using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using H.Core.Factories;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ComponentViews;

public abstract class AnimalComponentViewModelBase : ViewModelBase
{
    #region Fields

    private AnimalComponentBase _selectedAnimalComponent;

    protected IAnimalComponentDto _selectedAnimalComponentDto;

    protected IAnimalComponentService AnimalComponentService;
    private ObservableCollection<ManagementPeriodDto> _managementPeriodDtos;

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

    public override void InitializeViewModel(ComponentBase component)
    {
        if (component is AnimalComponentBase animalComponentBase)
        {
            base.InitializeViewModel(component);

            this.PropertyChanged += OnPropertyChanged;

            _selectedAnimalComponent = animalComponentBase;

            this.AnimalComponentService.InitializeComponent(base.StorageService.GetActiveFarm(), animalComponentBase);

            // Build a DTO to represent the model/domain object
            var dto = this.AnimalComponentService.TransferToAnimalComponentDto(animalComponentBase);

            this.SelectedAnimalComponentDto = dto;

            dto.PropertyChanged += OnAnimalComponentDtoPropertyChanged;
        }
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

    #region Private Methods

    private void Construct()
    {
        this.ManagementPeriodDtos = new ObservableCollection<ManagementPeriodDto>();
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
}