using H.Core.Factories.Animals.Dairy;
using H.Core.Factories.Animals;
using H.Core.Models;
using H.Core.Models.Animals.Dairy;
using H.Core.Services.Animals.Dairy;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using H.Core.Enumerations;

namespace H.Avalonia.ViewModels.ComponentViews.Dairy
{
    /// <summary>
    /// View model for the Dairy Component feature, which allows users to manage dairy cattle operations
    /// including herd composition, lactation stages, and production parameters.
    /// </summary>
    public class DairyComponentViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Service for managing dairy component operations and data transfer
        /// </summary>
        private readonly IDairyComponentService _dairyComponentService;

        /// <summary>
        /// The domain model object representing the dairy component being edited
        /// </summary>
        private DairyComponent _selectedDairyComponent;

        /// <summary>
        /// Data transfer object containing dairy component parameters (herd overview, lactation stages, etc.)
        /// This DTO is bound to the view and includes validation logic
        /// </summary>
        private IDairyComponentDto _selectedDairyComponentDto;

        /// <summary>
        /// Tracks which herd stage card is currently selected (Calf, Heifer, Lactating, or Dry)
        /// </summary>
        private string _selectedHerdStage;

        /// <summary>
        /// Indicates if the Calf card is selected
        /// </summary>
        private bool _isCalfSelected;

        /// <summary>
        /// Indicates if the Heifer card is selected
        /// </summary>
        private bool _isHeiferSelected;

        /// <summary>
        /// Indicates if the Lactating card is selected
        /// </summary>
        private bool _isLactatingSelected;

        /// <summary>
        /// Indicates if the Dry card is selected
        /// </summary>
        private bool _isDrySelected;

        /// <summary>
        /// Collection of available manure state types for dropdown selection
        /// </summary>
        private IEnumerable<ManureStateType> _manureStateTypes;

        /// <summary>
        /// Collection of available housing types for dropdown selection
        /// </summary>
        private IEnumerable<HousingType> _housingTypes;

        #endregion

        #region Constructors

        /// <summary>
        /// Default parameterless constructor for design-time support and testing.
        /// Initializes collections and commands but does not inject dependencies.
        /// </summary>
        public DairyComponentViewModel()
        {
            this.Construct();
        }

        /// <summary>
        /// Primary constructor with dependency injection for runtime use.
        /// Validates all injected dependencies and initializes the view model.
        /// </summary>
        /// <param name="regionManager">Prism region manager for navigation between views</param>
        /// <param name="eventAggregator">Event aggregator for pub/sub messaging between components</param>
        /// <param name="storageService">Service for accessing application storage and active farm data</param>
        /// <param name="dairyComponentService">Service for dairy component operations and data transfer</param>
        /// <param name="logger">Logger instance for diagnostic and error logging</param>
        /// <exception cref="ArgumentNullException">Thrown if any required dependency is null</exception>
        public DairyComponentViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IStorageService storageService,
            IDairyComponentService dairyComponentService,
            ILogger logger) : base(regionManager, eventAggregator, storageService, logger)
        {
            // Validate and store dairy component service dependency
            _dairyComponentService = dairyComponentService ?? throw new ArgumentNullException(nameof(dairyComponentService));

            // Initialize collections and commands
            this.Construct();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The DTO representing the dairy component's herd overview and production parameters.
        /// This property is bound to the view and includes validation logic for user input.
        /// 
        /// ARCHITECTURE NOTE:
        /// This DTO contains AnimalGroupDtos which the view should bind to directly via:
        /// {Binding SelectedDairyComponentDto.AnimalGroupDtos}
        /// 
        /// This ensures proper validation and data flow through the DTO layer.
        /// The collection is guaranteed to be non-null (initialized in AnimalComponentDto constructor).
        /// </summary>
        public IDairyComponentDto SelectedDairyComponentDto
        {
            get => _selectedDairyComponentDto;
            set
            {
                // Unsubscribe from old DTO if it exists
                if (_selectedDairyComponentDto != null)
                {
                    _selectedDairyComponentDto.PropertyChanged -= OnDairyComponentDtoPropertyChanged;
                }

                SetProperty(ref _selectedDairyComponentDto, value);

                // Subscribe to new DTO
                if (_selectedDairyComponentDto != null)
                {
                    _selectedDairyComponentDto.PropertyChanged += OnDairyComponentDtoPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently selected herd stage
        /// </summary>
        public string SelectedHerdStage
        {
            get => _selectedHerdStage;
            set => SetProperty(ref _selectedHerdStage, value);
        }

        /// <summary>
        /// Gets or sets whether the Calf card is selected
        /// </summary>
        public bool IsCalfSelected
        {
            get => _isCalfSelected;
            set
            {
                if (SetProperty(ref _isCalfSelected, value))
                {
                    RaisePropertyChanged(nameof(IsAnyStageSelected));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Heifer card is selected
        /// </summary>
        public bool IsHeiferSelected
        {
            get => _isHeiferSelected;
            set
            {
                if (SetProperty(ref _isHeiferSelected, value))
                {
                    RaisePropertyChanged(nameof(IsAnyStageSelected));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Lactating card is selected
        /// </summary>
        public bool IsLactatingSelected
        {
            get => _isLactatingSelected;
            set
            {
                if (SetProperty(ref _isLactatingSelected, value))
                {
                    RaisePropertyChanged(nameof(IsAnyStageSelected));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the Dry card is selected
        /// </summary>
        public bool IsDrySelected
        {
            get => _isDrySelected;
            set
            {
                if (SetProperty(ref _isDrySelected, value))
                {
                    RaisePropertyChanged(nameof(IsAnyStageSelected));
                }
            }
        }

        /// <summary>
        /// Gets or sets the collection of available manure state types
        /// </summary>
        public IEnumerable<ManureStateType> ManureStateTypes
        {
            get => _manureStateTypes;
            set => SetProperty(ref _manureStateTypes, value);
        }

        /// <summary>
        /// Gets or sets the collection of available housing types
        /// </summary>
        public IEnumerable<HousingType> HousingTypes
        {
            get => _housingTypes;
            set => SetProperty(ref _housingTypes, value);
        }

        /// <summary>
        /// Gets whether any lifecycle stage card is currently selected.
        /// Used to control visibility of Step 3 (Lifecycle Configuration).
        /// </summary>
        public bool IsAnyStageSelected => IsCalfSelected || IsHeiferSelected || IsLactatingSelected || IsDrySelected;

        #endregion

        #region Public Methods

        /// <summary>
        /// Entry point when navigating to this view. Gets a reference to the <see cref="DairyComponent"/>
        /// the user selected from the component selection view.
        /// </summary>
        /// <param name="navigationContext">An object holding a reference to the selected <see cref="DairyComponent"/></param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey(GuiConstants.ComponentKey))
            {
                var parameter = navigationContext.Parameters[GuiConstants.ComponentKey];
                if (parameter is DairyComponent dairyComponent)
                {
                    this.InitializeViewModel(dairyComponent);
                }
            }
        }

        /// <summary>
        /// Called when navigating away from this view. Performs final transfer of DTO data to domain model
        /// and validates that no errors exist before allowing navigation.
        /// </summary>
        /// <param name="navigationContext">Navigation context containing navigation parameters</param>
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);

            // Perform final transfer from DTO to domain model before leaving
            if (_selectedDairyComponent != null && _selectedDairyComponentDto != null)
            {
                // Cast to concrete type to access validation properties
                var dairyComponentDto = _selectedDairyComponentDto as DairyComponentDto;
                
                // Only transfer if there are no validation errors
                if (dairyComponentDto != null && !dairyComponentDto.HasErrors)
                {
                    try
                    {
                        _dairyComponentService.TransferDairyDtoToSystem(
                            dairyComponentDto, 
                            _selectedDairyComponent);
                        
                        Logger?.LogInformation("Successfully saved dairy component changes");
                    }
                    catch (Exception exception)
                    {
                        Logger?.LogError(exception, "Error saving dairy component changes during navigation");
                    }
                }
                else
                {
                    Logger?.LogWarning("Dairy component has validation errors, changes not saved");
                }
            }

            // Clean up event handlers
            if (_selectedDairyComponentDto != null)
            {
                _selectedDairyComponentDto.PropertyChanged -= OnDairyComponentDtoPropertyChanged;
                
                if (_selectedDairyComponentDto is DairyComponentDto dto)
                {
                    dto.PropertyChanged -= DairyComponentDtoOnPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Initializes the view model with a dairy component
        /// </summary>
        /// <param name="component">The dairy component to initialize with</param>
        public override void InitializeViewModel(ComponentBase component)
        {
            if (component is not DairyComponent dairyComponent)
            {
                return;
            }

            base.InitializeViewModel(component);

            this.InitializeDairyComponent(dairyComponent);
        }

        /// <summary>
        /// Initializes the dairy component and sets up data binding.
        /// 
        /// ARCHITECTURE NOTE:
        /// This method creates a DairyComponentDto from the domain model.
        /// The DTO will contain AnimalGroupDtos (not domain AnimalGroup objects).
        /// The service layer handles the conversion between domain objects and DTOs.
        /// </summary>
        /// <param name="dairyComponent">The dairy component to initialize</param>
        public void InitializeDairyComponent(DairyComponent dairyComponent)
        {
            if (dairyComponent == null)
            {
                return;
            }

            // Hold a reference to the selected dairy component domain object
            _selectedDairyComponent = dairyComponent;

            // Build a DTO to represent the model/domain object using the dairy-specific service
            // This will also convert AnimalGroup domain objects to AnimalGroupDtos
            var dairyComponentDto = _dairyComponentService.TransferToDairyComponentDto(dairyComponent);

            // Listen for changes on the DTO so we can validate user input before assigning values to the model
            dairyComponentDto.PropertyChanged += this.DairyComponentDtoOnPropertyChanged;

            // Assign the DTO to the property that is bound to the view
            // This will also trigger RaisePropertyChanged for AnimalGroupDtos
            this.SelectedDairyComponentDto = dairyComponentDto;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Common initialization method called by both constructors.
        /// Sets up empty collections and initializes all commands with their execution and validation logic.
        /// This ensures consistent initialization regardless of which constructor is used.
        /// </summary>
        private void Construct()
        {
            // Initialize selection properties
            _selectedHerdStage = string.Empty;
            _isCalfSelected = false;
            _isHeiferSelected = false;
            _isLactatingSelected = false;
            _isDrySelected = false;

            // Initialize manure state types (excluding obsolete options)
            ManureStateTypes = Enum.GetValues<ManureStateType>()
                .Where(x => !x.GetType().GetMember(x.ToString())[0]
                    .GetCustomAttributes(typeof(ObsoleteAttribute), false).Any())
                .ToList();

            // Initialize housing types (excluding obsolete options)
            HousingTypes = Enum.GetValues<HousingType>()
                .Where(x => !x.GetType().GetMember(x.ToString())[0]
                    .GetCustomAttributes(typeof(ObsoleteAttribute), false).Any())
                .ToList();
        }

        /// <summary>
        /// Selects a specific herd stage card and deselects others
        /// </summary>
        /// <param name="stage">The stage to select ("Calf", "Heifer", "Lactating", or "Dry")</param>
        public void SelectHerdStage(string stage)
        {
            // Deselect all cards first
            IsCalfSelected = false;
            IsHeiferSelected = false;
            IsLactatingSelected = false;
            IsDrySelected = false;

            // Select the clicked card
            SelectedHerdStage = stage;

            switch (stage)
            {
                case "Calf":
                    IsCalfSelected = true;
                    break;
                case "Heifer":
                    IsHeiferSelected = true;
                    break;
                case "Lactating":
                    IsLactatingSelected = true;
                    break;
                case "Dry":
                    IsDrySelected = true;
                    break;
            }

            Logger?.LogDebug($"Selected herd stage: {stage}");
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles property changes on the DairyComponentDto.
        /// This is where we can add logic to respond to specific property changes.
        /// </summary>
        private void OnDairyComponentDtoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Handle property change notifications from the DTO
            // Views binding directly to SelectedDairyComponentDto.AnimalGroupDtos will
            // automatically receive collection change notifications via INotifyPropertyChanged
        }

        /// <summary>
        /// Some property on the <see cref="SelectedDairyComponentDto"/> has changed. Check if we need to validate any user
        /// input before assigning the value on to the associated <see cref="DairyComponent"/> domain object.
        /// 
        /// ARCHITECTURE NOTE:
        /// Changes to AnimalGroupDtos will also flow through this handler.
        /// The service layer handles transferring AnimalGroupDto changes back to AnimalGroup domain objects.
        /// </summary>
        private void DairyComponentDtoOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            if (sender is DairyComponentDto dairyComponentDto)
            {
                /*
                 * Before assigning values from the bound DTOs, check for any validation errors. If there are any validation errors
                 * we should not proceed with the transfer of user input from the DTO to the model until the validation errors are fixed
                 */

                if (!dairyComponentDto.HasErrors)
                {
                    try
                    {
                        // A property on the DTO has been changed by the user, assign the new value to the system object after any unit conversion (if necessary)
                        // This includes changes to AnimalGroupDtos which will be transferred to AnimalGroup domain objects
                        _dairyComponentService.TransferDairyDtoToSystem(dairyComponentDto, _selectedDairyComponent);
                    }
                    catch (Exception exception)
                    {
                        Logger?.LogError(exception, "Error transferring dairy component DTO to domain object");
                    }
                }
            }
        }

        #endregion
    }
}