using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using DryIoc;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Factories.Animals;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Commands;
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
        Logger?.LogDebug("Parameterless constructor called");
        this.Construct();
    }

    protected AnimalComponentViewModelBase(
        IAnimalComponentService animalComponentService, 
        ILogger logger,
        IStorageService storageService, 
        IManagementPeriodService managementPeriodService) : base(storageService, logger)
    {
        Logger?.LogDebug("Constructor with dependencies. AnimalComponentService: {HasService}, ManagementPeriodService: {HasManagementService}", 
            animalComponentService != null, managementPeriodService != null);
        
        this.AnimalComponentService = animalComponentService;
        this.ManagementPeriodService = managementPeriodService;

        this.Construct();
    }

    private void Construct()
    {
        Logger?.LogDebug("Initializing collections and commands");
        
        ManagementPeriodDtos = new ObservableCollection<ManagementPeriodDto>();
        Groups = new ObservableCollection<AnimalGroup>();
        
        // Initialize commands
        AddManagementPeriodCommand = new DelegateCommand(HandleAddManagementPeriodEvent);
        AddGroupCommand = new DelegateCommand(HandleAddGroupEvent);
        
        Logger?.LogDebug("Initialization completed. ManagementPeriodDtos: {ManagementCount}, Groups: {GroupsCount}", 
            ManagementPeriodDtos?.Count ?? 0, Groups?.Count ?? 0);
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

    #region Commands

    /// <summary>
    /// Command to add a new management period with default values.
    /// </summary>
    public ICommand AddManagementPeriodCommand { get; private set; }

    /// <summary>
    /// Command to add a new animal group with default values.
    /// </summary>
    public ICommand AddGroupCommand { get; private set; }

    #endregion

    #region Public Methods

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        Logger?.LogInformation("Navigation started. Parameters: {ParameterCount}", 
            navigationContext?.Parameters?.Count ?? 0);
        
        base.OnNavigatedTo(navigationContext);

        if (navigationContext.Parameters.ContainsKey(GuiConstants.ComponentKey))
        {
            var parameter = navigationContext.Parameters[GuiConstants.ComponentKey];
            Logger?.LogDebug("Found ComponentKey parameter: {ParameterType}", 
                parameter?.GetType().Name ?? "null");
            
            if (parameter is AnimalComponentBase animalComponent)
            {
                Logger?.LogInformation("Initializing component: {ComponentName} ({ComponentType})", 
                    animalComponent.Name ?? "Unknown", animalComponent.GetType().Name);
                this.InitializeViewModel(animalComponent);
            }
            else
            {
                Logger?.LogWarning("ComponentKey parameter is not AnimalComponentBase: {ActualType}", 
                    parameter?.GetType().Name ?? "null");
            }
        }
        else
        {
            Logger?.LogWarning("No ComponentKey found in navigation parameters");
        }
    }

    /// <summary>
    /// When the user navigates to a <see cref="AnimalComponentBase"/>, we must initialize the component and any DTOs
    /// that will be used with the view
    /// </summary>
    /// <param name="component">The <see cref="AnimalComponentBase"/> to display to the user</param>
    public override void InitializeViewModel(ComponentBase component)
    {
        Logger?.LogInformation("ViewModel initialization started: {ComponentType}", 
            component?.GetType().Name ?? "null");

        if (component is not AnimalComponentBase animalComponentBase)
        {
            Logger?.LogWarning("Component is not AnimalComponentBase: {ComponentType}", 
                component?.GetType().Name ?? "null");
            return;
        }

        Logger?.LogDebug("Starting base initialization: {ComponentName}", 
            animalComponentBase.Name ?? "Unknown");
        base.InitializeViewModel(component);

        this.PropertyChanged += OnPropertyChanged;

        Logger?.LogDebug("Calling InitializeAnimalComponent");
        this.InitializeAnimalComponent(animalComponentBase);

        // Build a DTO to represent the model/domain object
        Logger?.LogDebug("Creating DTO from component. Service available: {HasService}", 
            AnimalComponentService != null);
        var animalComponentDto = this.AnimalComponentService?.TransferToAnimalComponentDto(animalComponentBase);

        if (animalComponentDto != null)
        {
            Logger?.LogInformation("DTO created successfully: {DtoType}", 
                animalComponentDto.GetType().Name);
            this.SelectedAnimalComponentDto = animalComponentDto;

            animalComponentDto.PropertyChanged += OnAnimalComponentDtoPropertyChanged;
            Logger?.LogDebug("Property change handler attached to DTO");
        }
        else
        {
            Logger?.LogError("Failed to create DTO. Service available: {ServiceAvailable}", 
                AnimalComponentService != null);
        }
    }

    public void AddExistingManagementPeriods()
    {
        Logger?.LogInformation("Adding existing management periods");
        
        try
        {
            Farm currentFarm = StorageService.GetActiveFarm();
            Logger?.LogDebug("Retrieved active farm: {FarmName}", 
                currentFarm?.Name ?? "Unknown");
            
            var existingManagementPeriods = currentFarm.GetAllManagementPeriods();
            Logger?.LogInformation("Found {PeriodCount} existing periods", 
                existingManagementPeriods?.Count ?? 0);

            int addedCount = 0;
            foreach (var managementPeriod in existingManagementPeriods)
            {
                var newManagementPeriodViewModel = new ManagementPeriodDto();
                newManagementPeriodViewModel.Name = managementPeriod.GroupName;
                newManagementPeriodViewModel.Start = managementPeriod.Start;
                newManagementPeriodViewModel.End = managementPeriod.End;
                newManagementPeriodViewModel.NumberOfDays = managementPeriod.NumberOfDays;
                
                ManagementPeriodDtos?.Add(newManagementPeriodViewModel);
                addedCount++;
                
                Logger?.LogDebug("Added period: {PeriodName} ({StartDate} to {EndDate})", 
                    managementPeriod.GroupName ?? "Unknown", managementPeriod.Start, managementPeriod.End);
            }
            
            Logger?.LogInformation("Successfully added {AddedCount} periods", addedCount);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error adding existing management periods");
        }
    }

    protected void InitializeAnimalComponent(AnimalComponentBase animalComponent)
    {
        Logger?.LogDebug("Starting component initialization: {ComponentName}", 
            animalComponent?.Name ?? "null");

        if (animalComponent == null)
        {
            Logger?.LogWarning("Component is null, exiting initialization");
            return;
        }

        // Hold a reference to the selected animal component
        _selectedAnimalComponent = animalComponent;
        Logger?.LogDebug("Component reference set");

        // Build a DTO to represent the model/domain object
        Logger?.LogDebug("Creating DTO using service: {ServiceType}", 
            AnimalComponentService?.GetType().Name ?? "null");
        var animalComponentDto = this.AnimalComponentService?.TransferToAnimalComponentDto(_selectedAnimalComponent);
        
        if (animalComponentDto != null)
        {
            Logger?.LogDebug("DTO created, attaching event handlers");
            // Listen for changes on the DTO
            animalComponentDto.PropertyChanged += OnAnimalComponentDtoPropertyChanged;

            // Assign the DTO to the property bound to the view
            this.SelectedAnimalComponentDto = animalComponentDto;
            Logger?.LogInformation("Component initialization completed");
        }
        else
        {
            Logger?.LogError("Failed to create DTO. Service available: {HasService}", 
                AnimalComponentService != null);
        }
    }

    private void OnAnimalComponentDtoPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Logger?.LogDebug("DTO property changed: {PropertyName}", e.PropertyName ?? "Unknown");
        
        if (sender is IAnimalComponentDto dto)
        {
            Logger?.LogDebug("Transferring changes to domain object");
            try
            {
                // A property on the DTO has been changed by the user, assign the new value to the system object after any unit conversion (if necessary)
                this.AnimalComponentService?.TransferAnimalComponentDtoToSystem((AnimalComponentDto) dto, _selectedAnimalComponent);
                Logger?.LogDebug("Successfully transferred changes to domain object");
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error transferring changes to domain object");
            }
        }
        else
        {
            Logger?.LogWarning("Property change sender is not IAnimalComponentDto: {SenderType}", 
                sender?.GetType().Name ?? "null");
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Logger?.LogDebug("Property changed: {PropertyName}", e.PropertyName ?? "Unknown");
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Adds an item to the <see cref="ManagementPeriodDtos"/> collection / a row to the respective bound DataGrid. Seeded with some default values.
    /// </summary>
    public void HandleAddManagementPeriodEvent()
    {
        Logger?.LogInformation("Adding new management period");
        
        try
        {
            if (ManagementPeriodDtos != null)
            {
                int numPeriods = ManagementPeriodDtos.Count;
                Logger?.LogDebug("Current period count: {CurrentCount}", numPeriods);
                
                var newManagementPeriodViewModel = new ManagementPeriodDto 
                { 
                    Name = $"Period #{numPeriods + 1}", 
                    Start = new DateTime(2024, 01, 01), 
                    End = new DateTime(2025, 01, 01), 
                    NumberOfDays = 364 
                };
                
                ManagementPeriodDtos.Add(newManagementPeriodViewModel);
                Logger?.LogInformation("Added period: {PeriodName}. Total: {TotalCount}", 
                    newManagementPeriodViewModel.Name, ManagementPeriodDtos.Count);
            }
            else
            {
                Logger?.LogError("Cannot add period - collection is null");
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error adding management period");
        }
    }

    /// <summary>
    /// Adds an item to the <see cref="AnimalComponentViewModelBase.Groups"/> collection / a row to the respective bound DataGrid. Seeded with <see cref="AnimalType"/>.
    /// </summary>
    public void HandleAddGroupEvent()
    {
        Logger?.LogInformation("Adding new group. Animal type: {AnimalType}", AnimalType);
        
        try
        {
            var currentGroupCount = Groups?.Count ?? 0;
            Logger?.LogDebug("Current group count: {CurrentCount}", currentGroupCount);
            
            var newGroup = new AnimalGroup { GroupType = AnimalType };
            Groups.Add(newGroup);
            
            Logger?.LogInformation("Added group with type: {GroupType}. Total: {TotalCount}", 
                AnimalType, Groups.Count);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error adding animal group");
        }
    }

    #endregion

    #region Private Methods

    #endregion

}