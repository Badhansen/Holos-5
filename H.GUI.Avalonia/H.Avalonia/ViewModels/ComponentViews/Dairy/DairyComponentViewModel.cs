using H.Core.Factories.Animals.Dairy;
using H.Core.Models;
using H.Core.Models.Animals.Dairy;
using H.Core.Services.Animals.Dairy;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Regions;
using System;
using System.ComponentModel;

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
        /// Initializes the dairy component and sets up data binding
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
            var dairyComponentDto = _dairyComponentService.TransferToDairyComponentDto(dairyComponent);

            // Listen for changes on the DTO so we can validate user input before assigning values to the model
            dairyComponentDto.PropertyChanged += this.DairyComponentDtoOnPropertyChanged;

            // Assign the DTO to the property that is bound to the view
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
            // Initialize commands and collections here as needed
            // Currently no commands or collections needed for basic dairy component
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles property changes on the DairyComponentDto
        /// </summary>
        private void OnDairyComponentDtoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Handle property change notifications from the DTO
            // This is where we can add logic to respond to specific property changes
        }

        /// <summary>
        /// Some property on the <see cref="SelectedDairyComponentDto"/> has changed. Check if we need to validate any user
        /// input before assigning the value on to the associated <see cref="DairyComponent"/> domain object.
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