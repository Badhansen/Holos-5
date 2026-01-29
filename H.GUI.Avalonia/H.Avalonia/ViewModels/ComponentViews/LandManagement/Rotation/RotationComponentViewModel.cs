using H.Core.Factories.Crops;
using H.Core.Factories.Fields;
using H.Core.Factories.Rotations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using H.Core.Services.CropColorService;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using H.Avalonia.Views.ComponentViews;
using Prism.Commands;
using Avalonia.Media;
using H.Core.Enumerations;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement.Rotation
{
    /// <summary>
    /// View model for the Rotation Component feature, which allows users to create and manage crop rotations
    /// across multiple fields over multiple years. This view model handles three main responsibilities:
    /// 
    /// 1. Step 1 (Basic Settings): Farm rotation parameters (start/end year, number of fields, field area)
    /// 2. Step 2 (Timeline): Building the crop rotation sequence by adding crops
    /// 3. Step 3 (Preview): Visualizing how crops are distributed across fields over time
    /// 
    /// The rotation can operate in two modes:
    /// - Shift Enabled: Crops rotate across fields, starting at different points (e.g., Field 1 starts with Wheat, Field 2 starts with Barley)
    /// - Shift Disabled: All fields follow the same crop sequence in the same years
    /// 
    /// Key features:
    /// - Dynamic preview grid showing crop assignments per field per year
    /// - Color-coded cells based on crop type (cereals=orange, oilseeds=green, pulses=blue, forages=purple, fallow=gray)
    /// - Interactive selection: clicking a crop card in Step 2 highlights all matching crop types in Step 3 preview
    /// </summary>
    public class RotationComponentViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// Service for managing field component operations (initialization, validation, transfer between DTO and model)
        /// </summary>
        private readonly IFieldComponentService _fieldComponentService;
        
        /// <summary>
        /// Service for managing rotation component operations and data transfer
        /// </summary>
        private readonly IRotationComponentService _rotationComponentService;
        
        /// <summary>
        /// Factory for creating new crop DTOs with proper initialization
        /// </summary>
        private readonly ICropFactory _cropFactory;
        
        /// <summary>
        /// Service providing color codes and display names for different crop types
        /// </summary>
        private readonly ICropColorService _cropColorService;
        
        /// <summary>
        /// The domain model object representing the rotation component being edited
        /// </summary>
        private RotationComponent _selectedRotationComponent;
        
        /// <summary>
        /// Data transfer object containing rotation parameters (start year, end year, number of fields, field area)
        /// This DTO is bound to the view and includes validation logic
        /// </summary>
        private IRotationComponentDto _selectedRotationComponentDto;
        
        /// <summary>
        /// Collection of field component DTOs (one per crop in rotation, used for field system integration)
        /// </summary>
        private ObservableCollection<IFieldComponentDto> _fieldComponentDtos;
        
        /// <summary>
        /// Collection of crop DTOs representing the rotation sequence (Step 2 timeline)
        /// Each crop represents one year in the rotation cycle
        /// </summary>
        private ObservableCollection<ICropDto> _cropDtos;
        
        /// <summary>
        /// Collection of field assignment rows for the preview grid (Step 3)
        /// Each row represents one field, containing year/crop assignments
        /// </summary>
        private ObservableCollection<FieldAssignmentRow> _fieldAssignmentRows;
        
        /// <summary>
        /// The direction in which crops shift across fields in the rotation.
        /// - None: All fields grow the same crops in the same years (no staggering)
        /// - RightShift: Each field starts one position later in the sequence (traditional rotation)
        /// - LeftShift: Each field starts one position earlier in the sequence (reverse rotation)
        /// </summary>
        private RotationShiftDirection _shiftDirection = RotationShiftDirection.RightShift;

        /// <summary>
        /// Flag to track if the crop selection came from clicking a preview grid cell
        /// This is used to determine if auto-scrolling should occur
        /// </summary>
        private bool _selectionFromGridCell = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Default parameterless constructor for design-time support and testing.
        /// Initializes collections and commands but does not inject dependencies.
        /// </summary>
        public RotationComponentViewModel()
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
        /// <param name="fieldComponentService">Service for field component operations and data transfer</param>
        /// <param name="rotationComponentService">Service for rotation component operations and data transfer</param>
        /// <param name="logger">Logger instance for diagnostic and error logging</param>
        /// <param name="cropFactory">Factory for creating new crop DTOs with proper initialization</param>
        /// <param name="cropColorService">Service providing color codes and display names for crop types</param>
        /// <exception cref="ArgumentNullException">Thrown if any required dependency is null</exception>
        public RotationComponentViewModel(
            IRegionManager regionManager, 
            IEventAggregator eventAggregator, 
            IStorageService storageService, 
            IFieldComponentService fieldComponentService, 
            IRotationComponentService rotationComponentService,
            ILogger logger, 
            ICropFactory cropFactory,
            ICropColorService cropColorService) : base(regionManager, eventAggregator, storageService, logger)
        {
            // Validate and store crop factory dependency
            if (cropFactory != null)
            {
                _cropFactory = cropFactory;
            }
            else
            {
                throw new ArgumentNullException(nameof(cropFactory));
            }

            // Validate and store field component service dependency
            if (fieldComponentService != null)
            {
                _fieldComponentService = fieldComponentService;
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldComponentService));
            }

            // Validate and store rotation component service dependency
            if (rotationComponentService != null)
            {
                _rotationComponentService = rotationComponentService;
            }
            else
            {
                throw new ArgumentNullException(nameof(rotationComponentService));
            }

            // Validate and store crop color service dependency
            if (cropColorService != null)
            {
                _cropColorService = cropColorService;
            }
            else
            {
                throw new ArgumentNullException(nameof(cropColorService));
            }

            // Initialize collections and commands
            this.Construct();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Each of the field components that are part of the rotation, there will be one field component dto per view item in the rotation
        /// </summary>
        public ObservableCollection<IFieldComponentDto> FieldComponentDtos
        {
            get => _fieldComponentDtos;
            set => SetProperty(ref _fieldComponentDtos, value);
        }

        /// <summary>
        /// The user adds crops to the rotation, this collection holds the crop DTOs for each crop in the rotation. For each crop dto in this collection
        /// there is a corresponding field component dto in the <see cref="FieldComponentDtos"/> collection.
        /// </summary>
        public ObservableCollection<ICropDto> CropDtos
        {
            get => _cropDtos;
            set => SetProperty(ref _cropDtos, value);
        }

        /// <summary>
        /// The currently selected crop DTO that is being edited in Step 3
        /// </summary>
        private ICropDto _selectedCropDto;
        public ICropDto SelectedCropDto
        {
            get => _selectedCropDto;
            set => SetProperty(ref _selectedCropDto, value);
        }

        /// <summary>
        /// Indicates whether the current crop selection came from clicking a grid cell
        /// Used by the view to determine if auto-scrolling should occur
        /// </summary>
        private bool _shouldTriggerAutoScroll = false;
        public bool ShouldTriggerAutoScroll
        {
            get => _shouldTriggerAutoScroll;
            set => SetProperty(ref _shouldTriggerAutoScroll, value);
        }

        public IRotationComponentDto SelectedRotationComponentDto
        {
            get => _selectedRotationComponentDto;
            set
            {
                // Unsubscribe from old DTO if it exists
                if (_selectedRotationComponentDto != null)
                {
                    _selectedRotationComponentDto.PropertyChanged -= OnRotationDtoPropertyChanged;
                }

                SetProperty(ref _selectedRotationComponentDto, value);

                // Subscribe to new DTO
                if (_selectedRotationComponentDto != null)
                {
                    _selectedRotationComponentDto.PropertyChanged += OnRotationDtoPropertyChanged;
                }

                // Regenerate field assignments when DTO changes
                GenerateFieldAssignmentRows();
            }
        }

        /// <summary>
        /// Gets or sets the direction in which crops shift across fields.
        /// 
        /// Changing this property triggers regeneration of the preview grid to show
        /// the new rotation pattern with the selected shift direction.
        /// 
        /// Options:
        /// - None: No shifting, all fields synchronized
        /// - RightShift: Traditional rotation pattern (most common)
        /// - LeftShift: Reverse rotation pattern (alternative strategy)
        /// </summary>
        public RotationShiftDirection ShiftDirection
        {
            get => _shiftDirection;
            set
            {
                if (SetProperty(ref _shiftDirection, value))
                {
                    GenerateFieldAssignmentRows();
                    RaisePropertyChanged(nameof(IsShiftEnabled));
                }
            }
        }

        /// <summary>
        /// Computed property indicating whether any shifting is enabled (either left or right).
        /// Used for conditional visibility of UI hints about rotation shifting.
        /// </summary>
        public bool IsShiftEnabled => ShiftDirection != RotationShiftDirection.None;

        /// <summary>
        /// Dynamic collection of field assignment rows for the preview grid
        /// </summary>
        public ObservableCollection<FieldAssignmentRow> FieldAssignmentRows
        {
            get => _fieldAssignmentRows;
            set => SetProperty(ref _fieldAssignmentRows, value);
        }

        /// <summary>
        /// Whether there are no crops in the rotation
        /// </summary>
        public bool HasNoCrops => CropDtos == null || !CropDtos.Any();

        /// <summary>
        /// Whether the preview should be shown - both crops and fields must be configured
        /// </summary>
        public bool ShouldShowPreview => SelectedRotationComponentDto != null && 
                                          SelectedRotationComponentDto.NumberOfFields > 0 && 
                                          !HasNoCrops;

        /// <summary>
        /// Command to add a new crop to the rotation
        /// </summary>
        public ICommand AddCropToRotationCommand { get; private set; }

        /// <summary>
        /// Command to set the selected crop when a timeline card is clicked
        /// </summary>
        public ICommand SetSelectedCropCommand { get; private set; }

        /// <summary>
        /// Command to set the selected crop when a preview grid cell is clicked
        /// </summary>
        public ICommand SetSelectedCropFromCellCommand { get; private set; }

        /// <summary>
        /// Command to remove a specific crop from the rotation
        /// </summary>
        public ICommand RemoveSpecificCropCommand { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// A first point of entry to this class (after the constructor is called). Get a reference to the <see cref="RotationComponent"/> the
        /// user selected from the <see cref="MyComponentsView"/>.
        /// </summary>
        /// <param name="navigationContext">An object holding a reference to the selected <see cref="RotationComponent"/></param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey(GuiConstants.ComponentKey))
            {
                var parameter = navigationContext.Parameters[GuiConstants.ComponentKey];
                if (parameter is RotationComponent rotationComponent)
                {
                    this.InitializeViewModel(rotationComponent);
                }
            }
        }

        public override void InitializeViewModel(ComponentBase component)
        {
            if (component is not RotationComponent rotationComponent)
            {
                return;
            }

            base.InitializeViewModel(component);

            this.InitializeRotationComponent(rotationComponent);
        }

        public void InitializeRotationComponent(RotationComponent rotationComponent)
        {
            if (rotationComponent == null)
            {
                return;
            }

            // Hold a reference to the selected field system object
            _selectedRotationComponent = rotationComponent;

            // Build a DTO to represent the model/domain object
            var rotationComponentDto = _rotationComponentService.TransferToRotationComponentDto(rotationComponent);

            // Listen for changes on the DTO so we can validate user input before assigning values to the model
            rotationComponentDto.PropertyChanged += this.RotationComponentDtoOnPropertyChanged;

            // Assign the DTO to the property that is bound to the view
            this.SelectedRotationComponentDto = rotationComponentDto;
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
            // Initialize empty collections for field components, crops, and preview grid rows
            this.FieldComponentDtos = new ObservableCollection<IFieldComponentDto>();
            this.CropDtos = new ObservableCollection<ICropDto>();
            this.FieldAssignmentRows = new ObservableCollection<FieldAssignmentRow>();

            // Subscribe to collection changed events to regenerate preview when crops are added/removed
            this.CropDtos.CollectionChanged += OnCropDtosCollectionChanged;

            // Initialize command for adding a new crop to the rotation sequence
            this.AddCropToRotationCommand = new DelegateCommand(OnAddCropToRotation);
            
            // Initialize command for selecting a crop card (used in Step 2 timeline)
            this.SetSelectedCropCommand = new DelegateCommand<object>(OnSetSelectedCropExecute);
            
            // Initialize command for selecting a crop from preview grid cell (used in Step 3)
            this.SetSelectedCropFromCellCommand = new DelegateCommand<object>(OnSetSelectedCropFromCellExecute);
            
            // Initialize command for removing a crop from the rotation
            this.RemoveSpecificCropCommand = new DelegateCommand<object>(OnRemoveSpecificCropExecute);
        }

        /// <summary>
        /// Generates the field assignment rows for the preview grid (Step 3) based on current rotation parameters.
        /// 
        /// This method performs the core rotation calculation logic:
        /// 1. Takes the crop sequence defined in Step 2 (CropDtos collection)
        /// 2. Applies rotation parameters (start year, end year, number of fields, field area)
        /// 3. Creates a grid showing which crop grows in which field in which year
        /// 4. Optionally applies rotation shifting across fields
        /// 
        /// Algorithm:
        /// - For each field, iterate through all years in the rotation period
        /// - For each year, calculate which crop from the sequence should be grown
        /// - If shift is enabled: each field starts at a different point in the sequence (field offset)
        /// - Uses modulo arithmetic to wrap around when reaching the end of the crop sequence
        /// 
        /// Example with 3 crops [Wheat, Barley, Oats] and 2 fields:
        /// - Field 1: Wheat(2020), Barley(2021), Oats(2022), Wheat(2023)...
        /// - Field 2 (shifted): Barley(2020), Oats(2021), Wheat(2022), Barley(2023)...
        /// </summary>
        protected virtual void GenerateFieldAssignmentRows()
        {
            // Initialize or clear the field assignment rows collection
            if (FieldAssignmentRows == null)
            {
                FieldAssignmentRows = new ObservableCollection<FieldAssignmentRow>();
            }
            else
            {
                FieldAssignmentRows.Clear();
            }

            // Early exit if required data is missing
            if (CropDtos == null || !CropDtos.Any() || SelectedRotationComponentDto == null)
            {
                RaisePropertyChanged(nameof(HasNoCrops));
                return;
            }

            // Extract rotation parameters
            var crops = CropDtos.ToList();
            var rotationLength = crops.Count; // Number of crops in the rotation sequence
            var startYear = SelectedRotationComponentDto.StartYear;
            var endYear = SelectedRotationComponentDto.EndYear;
            var fieldArea = SelectedRotationComponentDto.FieldArea;
            var numberOfFields = SelectedRotationComponentDto.NumberOfFields;

            // Validate year range
            if (startYear <= 0 || endYear <= 0 || endYear <= startYear)
            {
                RaisePropertyChanged(nameof(HasNoCrops));
                return;
            }

            var totalYears = endYear - startYear + 1;

            // Generate a row for each field
            for (int fieldIndex = 0; fieldIndex < numberOfFields; fieldIndex++)
            {
                var row = new FieldAssignmentRow
                {
                    FieldName = $"Field {fieldIndex + 1} ({fieldArea:F0} ha)",
                    YearAssignments = new ObservableCollection<YearCropAssignment>()
                };

                // Calculate the shift offset for this field based on shift direction
                // The offset determines where in the crop sequence this field starts
                //
                // None: All fields start at position 0 (no offset)
                // RightShift: Field N starts at position N (0, 1, 2, 3...)
                //   Example: Field 0=Wheat, Field 1=Barley, Field 2=Oats (each field one step ahead)
                // LeftShift: Field N starts at position -N, which wraps around via modulo
                //   Example: Field 0=Wheat, Field 1=Oats, Field 2=Barley (each field one step behind)
                int shiftOffset = ShiftDirection switch
                {
                    RotationShiftDirection.RightShift => fieldIndex,        // Traditional: 0, 1, 2, 3...
                    RotationShiftDirection.LeftShift => -fieldIndex,         // Reverse: 0, -1, -2, -3...
                    RotationShiftDirection.None => 0,                        // No shift: all fields at 0
                    _ => 0                                                   // Default to no shift
                };

                // Generate year assignments for this field
                for (int yearIndex = 0; yearIndex < totalYears; yearIndex++)
                {
                    int year = startYear + yearIndex;

                    // Calculation of crop from sequence for the current year
                    int rawIndex = (yearIndex + shiftOffset) % rotationLength;
                    int cropIndex = (rawIndex + rotationLength) % rotationLength;  // Ensure positive result
                    var sourceCrop = crops[cropIndex];

                    // Create a unique crop DTO instance for this specific cell
                    // This ensures each cell has its own independent data that can be edited separately
                    ICropDto cellCropDto = null;
                    
                    if (_cropFactory != null && base.ActiveFarm != null)
                    {
                        cellCropDto = _cropFactory.CreateDto(base.ActiveFarm);
                        
                        // Copy properties from the source crop
                        cellCropDto.CropType = sourceCrop.CropType;
                        cellCropDto.Year = year;
                        cellCropDto.WetYield = sourceCrop.WetYield;
                        cellCropDto.AmountOfIrrigation = sourceCrop.AmountOfIrrigation;
                        cellCropDto.ValidCropTypes = sourceCrop.ValidCropTypes;
                    }
                    else
                    {
                        // Fallback to using the source crop if factory is not available
                        cellCropDto = sourceCrop;
                    }

                    // Create the cell data for this year/field combination
                    var assignment = new YearCropAssignment
                    {
                        Year = year.ToString(),
                        CropType = sourceCrop.CropType, // Store crop type for selection matching in Step 3
                        CropDto = cellCropDto, // Store reference to the unique crop DTO for this cell
                        CropDisplay = _cropColorService?.GetCropDisplayName(sourceCrop.CropType) ?? sourceCrop.CropType.ToString(),
                        CropBackground = _cropColorService != null 
                            ? Brush.Parse(_cropColorService.GetCropColorHex(sourceCrop.CropType))
                            : Brush.Parse("#F5F5F5"),
                        IsSelected = false // Initialize to not selected (updated when user clicks timeline card)
                    };

                    row.YearAssignments.Add(assignment);
                }

                FieldAssignmentRows.Add(row);
            }

            // Notify UI that the HasNoCrops property may have changed
            RaisePropertyChanged(nameof(HasNoCrops));
        }

        /// <summary>
        /// Handles the user clicking a crop card in the timeline (Step 2).
        /// 
        /// This method performs two key updates:
        /// 1. Updates the IsSelected property on all crop DTOs in the timeline
        /// 2. Updates the IsSelected property on all cells in the preview grid (Step 3)
        /// 
        /// The preview grid update highlights all cells that have the same crop type as the selected card,
        /// allowing users to visualize where that specific crop appears across all fields over time.
        /// 
        /// Example: Clicking "Wheat" card highlights all Wheat cells in the preview grid (blue border, 3px),
        /// showing users exactly which fields will grow wheat in which years.
        /// </summary>
        /// <param name="obj">The crop DTO representing the clicked timeline card</param>
        private void OnSetSelectedCropExecute(object obj)
        {
            // Validate that the view model is not disposed and the parameter is a valid crop DTO
            if (!IsDisposed && obj is ICropDto cropDto)
            {
                // Update the selection state on all crops in the timeline (Step 2)
                // Only the clicked crop will have IsSelected = true, others will be false
                UpdateCropSelectionStates(cropDto);
                
                // Update the selection state on all cells in the preview grid (Step 3)
                // All cells with matching crop type will have IsSelected = true
                UpdatePreviewCellSelection(cropDto);

                // Set the flag to false - timeline selection should NOT trigger auto-scroll
                this.ShouldTriggerAutoScroll = false;
                
                // Set the selected crop for editing in Step 3
                this.SelectedCropDto = cropDto;
            }
        }

        /// <summary>
        /// Handles the selection of a crop from a preview grid cell (Step 3).
        /// 
        /// This method updates only the clicked cell's selection state without affecting other cells.
        /// When a cell is clicked, it becomes the selected crop for editing in Step 4.
        /// </summary>
        /// <param name="obj">The YearCropAssignment representing the clicked cell in the preview grid</param>
        private void OnSetSelectedCropFromCellExecute(object obj)
        {
            // Validate that the view model is not disposed and the parameter is a YearCropAssignment
            if (!IsDisposed && obj is YearCropAssignment assignment)
            {
                // Clear all previous selections in the preview grid
                ClearAllCellSelections();
                
                // Set the flag to true - grid cell selection SHOULD trigger auto-scroll
                this.ShouldTriggerAutoScroll = true;
                
                // Set the selected crop for editing in Step 4
                this.SelectedCropDto = assignment.CropDto;
                
                // Select only the specific clicked cell
                assignment.IsSelected = true;
            }
        }

        /// <summary>
        /// Handles the deletion of a crop from the rotation sequence (Step 2 timeline).
        /// 
        /// This method performs several important operations:
        /// 1. Removes the crop from the CropDtos collection
        /// 2. Resets the Year property on the remaining crops to maintain consecutive years
        /// 3. Manages selection state (selects a different crop if the removed one was selected)
        /// 4. Triggers regeneration of the preview grid through collection change event
        /// 
        /// Year Reset Example:
        /// Before: [Wheat(2020), Barley(2021), Oats(2022), Corn(2023)]
        /// Remove Barley
        /// After: [Wheat(2020), Oats(2021), Corn(2022)] <- Years adjusted to remain consecutive
        /// </summary>
        /// <param name="obj">The crop DTO to remove from the rotation</param>
        private void OnRemoveSpecificCropExecute(object obj)
        {
            // Validate that the view model is not disposed and the parameter is a valid crop DTO
            if (!IsDisposed && obj is ICropDto cropDto)
            {
                try
                {
                    // Verify the crop exists in our collection before attempting removal
                    if (this.CropDtos != null && this.CropDtos.Contains(cropDto))
                    {
                        // Remove from the observable collection (triggers CollectionChanged event)
                        this.CropDtos.Remove(cropDto);

                        // Reset years to maintain consecutive sequence
                        // This ensures the timeline always shows years in order without gaps
                        // Example: If we had 2020, 2021, 2022 and removed 2021,
                        // the service will renumber to 2020, 2021 (was 2022)
                        if (this.CropDtos != null && this.CropDtos.Any())
                        {
                            _fieldComponentService.ResetAllYears(this.CropDtos);
                        }

                        // Handle selection management
                        // If we just deleted the selected crop, we need to select a different one
                        var wasSelected = cropDto.IsSelected;
                        if (wasSelected && this.CropDtos.Any())
                        {
                            // Select the last crop in the remaining sequence
                            var newSelectedCrop = this.CropDtos.Last();
                            UpdateCropSelectionStates(newSelectedCrop);
                        }
                        else if (!this.CropDtos.Any())
                        {
                            // No crops remain, clear all selections
                            UpdateCropSelectionStates(null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but don't crash the application
                    Logger?.LogError(ex, "Failed to remove specific crop from rotation");
                }
            }
        }

        /// <summary>
        /// Updates the IsSelected property on all crop DTOs in the timeline (Step 2).
        /// 
        /// This creates a "radio button" effect where only one crop can be selected at a time.
        /// The selected crop card will show visual selection styling (highlighted border, scale effect).
        /// 
        /// This method is called when:
        /// - User clicks a crop card in the timeline
        /// - A crop is deleted and we need to select a different one
        /// - All crops are deleted and we need to clear selection
        /// </summary>
        /// <param name="selectedCrop">The crop DTO to mark as selected, or null to clear all selections</param>
        private void UpdateCropSelectionStates(ICropDto selectedCrop)
        {
            if (this.CropDtos != null)
            {
                // Iterate through all crops in the timeline
                foreach (var crop in this.CropDtos)
                {
                    // Set IsSelected = true for the selected crop, false for all others
                    crop.IsSelected = selectedCrop != null && crop == selectedCrop;
                }
            }
        }

        /// <summary>
        /// Updates the IsSelected property on all cells in the preview grid (Step 3) based on crop type matching.
        /// 
        /// This method creates a visual connection between the timeline (Step 2) and preview grid (Step 3):
        /// - When user clicks a crop card in the timeline (e.g., "Wheat")
        /// - All cells in the preview grid that contain Wheat are highlighted (blue border, 3px thickness)
        /// - This shows users where that specific crop appears across all fields and years
        /// 
        /// Key difference from year-based selection:
        /// - We match on CropType, not Year
        /// - This highlights ALL instances of a crop, regardless of which year they appear in
        /// - Useful for seeing crop distribution patterns in a shifted rotation
        /// 
        /// Example: If Wheat is selected:
        /// - Field 1, 2020: Wheat → Highlighted
        /// - Field 1, 2023: Wheat → Highlighted (due to rotation cycle)
        /// - Field 2, 2021: Wheat → Highlighted (due to rotation shift)
        /// - Field 2, 2020: Barley → Not highlighted (different crop type)
        /// </summary>
        /// <param name="selectedCrop">The crop DTO from the timeline card that was clicked, or null to clear all selections</param>
        private void UpdatePreviewCellSelection(ICropDto selectedCrop)
        {
            // Early exit if preview grid hasn't been generated yet
            if (this.FieldAssignmentRows == null)
            {
                return;
            }

            // Extract the crop type from the selected crop (will be null if no crop is selected)
            var selectedCropType = selectedCrop?.CropType;

            // Iterate through all fields in the preview grid
            foreach (var row in this.FieldAssignmentRows)
            {
                if (row.YearAssignments != null)
                {
                    // Iterate through all year/crop assignments in this field
                    foreach (var assignment in row.YearAssignments)
                    {
                        // Highlight this cell if it has the same crop type as the selected timeline card
                        // The converter will apply blue border and 3px thickness when IsSelected = true
                        assignment.IsSelected = selectedCropType.HasValue && 
                                                assignment.CropType == selectedCropType.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the selection state of all cells in the preview grid.
        /// </summary>
        private void ClearAllCellSelections()
        {
            if (this.FieldAssignmentRows == null)
            {
                return;
            }

            foreach (var row in this.FieldAssignmentRows)
            {
                if (row.YearAssignments != null)
                {
                    foreach (var assignment in row.YearAssignments)
                    {
                        assignment.IsSelected = false;
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new crop DTO to the rotation
        /// </summary>
        private void OnAddCropToRotation()
        {
            if (_cropFactory == null)
            {
                return;
            }

            // Get the active farm
            var farm = this.ActiveFarm;
            if (farm == null)
            {
                return;
            }

            // Create a new crop DTO using the factory with farm initialization
            var newCropDto = _cropFactory.CreateDto(farm);

            // Set the year based on existing crops or rotation start year
            if (this.CropDtos != null && this.CropDtos.Any())
            {
                // Set year to be one year after the last crop
                var lastCrop = this.CropDtos.LastOrDefault();
                newCropDto.Year = lastCrop != null ? lastCrop.Year + 1 : DateTime.Now.Year;
            }
            else if (this.SelectedRotationComponentDto != null && this.SelectedRotationComponentDto.StartYear > 0)
            {
                // Use rotation start year if available
                newCropDto.Year = this.SelectedRotationComponentDto.StartYear;
            }
            else
            {
                // Default to current year
                newCropDto.Year = DateTime.Now.Year;
            }

            // Add to collection
            this.CropDtos?.Add(newCropDto);
            
            // DO NOT auto-select the newly added crop
            // Let the user explicitly click a cell in the preview grid to edit details
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles changes to the CropDtos collection (add/remove)
        /// </summary>
        private void OnCropDtosCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Subscribe to property changes on newly added items
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notifyPropertyChanged)
                    {
                        notifyPropertyChanged.PropertyChanged += OnCropDtoPropertyChanged;
                    }
                }
            }

            // Unsubscribe from property changes on removed items
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notifyPropertyChanged)
                    {
                        notifyPropertyChanged.PropertyChanged -= OnCropDtoPropertyChanged;
                    }
                }
            }

            // Regenerate the preview grid when crops are added or removed
            GenerateFieldAssignmentRows();
            
            // Notify UI that HasNoCrops and ShouldShowPreview may have changed
            RaisePropertyChanged(nameof(HasNoCrops));
            RaisePropertyChanged(nameof(ShouldShowPreview));
        }

        /// <summary>
        /// Handles property changes on individual crop DTOs
        /// </summary>
        private void OnCropDtoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Regenerate field assignments when CropType changes
            if (e.PropertyName == nameof(ICropDto.CropType))
            {
                GenerateFieldAssignmentRows();
            }
        }

        /// <summary>
        /// Handles property changes on the RotationComponentDto
        /// </summary>
        private void OnRotationDtoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Regenerate field assignments if relevant properties change
            if (e.PropertyName == nameof(IRotationComponentDto.StartYear) ||
                e.PropertyName == nameof(IRotationComponentDto.EndYear) ||
                e.PropertyName == nameof(IRotationComponentDto.FieldArea) ||
                e.PropertyName == nameof(IRotationComponentDto.NumberOfFields))
            {
                GenerateFieldAssignmentRows();
                
                // Notify UI that ShouldShowPreview may have changed when NumberOfFields changes
                if (e.PropertyName == nameof(IRotationComponentDto.NumberOfFields))
                {
                    RaisePropertyChanged(nameof(ShouldShowPreview));
                }
            }
        }

        /// <summary>
        /// Some property on the <see cref="SelectedRotationComponentDto"/> has changed. Check if we need to validate any user
        /// input before assigning the value on to the associated <see cref="RotationComponent"/> domain object.
        /// </summary>
        private void RotationComponentDtoOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            if (sender is RotationComponentDto rotationComponentDto)
            {
                /*
                 * Before assigning values from the bound DTOs, check for any validation errors. If there are any validation errors
                 * we should not proceed with the transfer of user input from the DTO to the model until the validation errors are fixed
                 */

                if (!rotationComponentDto.HasErrors)
                {
                    try
                    {
                        // A property on the DTO has been changed by the user, assign the new value to the system object after any unit conversion (if necessary)
                        _rotationComponentService.TransferRotationDtoToSystem(rotationComponentDto, _selectedRotationComponent);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                        throw;
                    }
                }
            }
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Represents a single row in the field assignment preview grid (Step 3).
    /// 
    /// Each row corresponds to one physical field on the farm and contains:
    /// - Field identifier (e.g., "Field 1 (100 ha)")
    /// - Collection of year/crop assignments showing what crop grows in each year
    /// 
    /// The grid is structured as:
    /// - Rows = Fields (vertical axis)
    /// - Columns = Years (horizontal axis)
    /// - Cell values = Crop types with color coding
    /// 
    /// Example with 3 fields over 4 years:
    /// | Field Name        | 2020  | 2021   | 2022 | 2023  |
    /// |-------------------|-------|--------|------|-------|
    /// | Field 1 (100 ha)  | Wheat | Barley | Oats | Corn  |
    /// | Field 2 (100 ha)  | Barley| Oats   | Corn | Wheat |
    /// | Field 3 (100 ha)  | Oats  | Corn   | Wheat| Barley|
    /// </summary>
    public class FieldAssignmentRow
    {
        /// <summary>
        /// Display name for this field including the field number and area in hectares.
        /// Format: "Field {number} ({area} ha)"
        /// Example: "Field 1 (100 ha)"
        /// </summary>
        public string FieldName { get; set; }
        
        /// <summary>
        /// Collection of year/crop assignments for this field, ordered chronologically.
        /// Each assignment represents one year in the rotation period and specifies which crop grows that year.
        /// The length of this collection equals (EndYear - StartYear + 1) from the rotation parameters.
        /// </summary>
        public ObservableCollection<YearCropAssignment> YearAssignments { get; set; }
    }

    /// <summary>
    /// Represents a single cell in the preview grid showing which crop grows in a specific field in a specific year.
    /// 
    /// This class implements INotifyPropertyChanged (via BindableBase) to support dynamic selection highlighting.
    /// When a user clicks a crop card in the timeline (Step 2), all cells with matching crop types in the
    /// preview grid (Step 3) are highlighted with a blue border.
    /// 
    /// Cell appearance is determined by:
    /// - Background color: Based on crop category (cereals=orange, oilseeds=green, pulses=blue, forages=purple, fallow=gray)
    /// - Border: Normal (1px gray) when not selected, highlighted (3px blue) when selected
    /// - Text: Crop name with emoji icon, truncated if too long with tooltip showing full name
    /// 
    /// Example cell data:
    /// - Year: "2020"
    /// - CropType: CropType.Wheat
    /// - CropDisplay: "Wheat 🌾"
    /// - CropBackground: "#FFF3E0" (light orange)
    /// - IsSelected: true (if user clicked the Wheat card in timeline)
    /// </summary>
    public class YearCropAssignment : Prism.Mvvm.BindableBase
    {
        /// <summary>
        /// Backing field for the IsSelected property
        /// </summary>
        private bool _isSelected;

        /// <summary>
        /// The calendar year for this assignment.
        /// Format: Four-digit year as string (e.g., "2020", "2025")
        /// Displayed in the top section of each cell in smaller, gray text.
        /// </summary>
        public string Year { get; set; }
        
        /// <summary>
        /// Human-readable crop name with emoji icon for display in the UI.
        /// Format: "{CropName} {Emoji}"
        /// Examples: "Wheat 🌾", "Canola 🌻", "Peas 🫘", "Alfalfa 🍀"
        /// 
        /// Provided by the ICropColorService.GetCropDisplayName() method.
        /// Falls back to CropType.ToString() if service is unavailable.
        /// 
        /// Displayed in the bottom section of each cell with text wrapping/trimming.
        /// </summary>
        public string CropDisplay { get; set; }
        
        /// <summary>
        /// Background color brush for the cell, based on crop category.
        /// 
        /// Color scheme:
        /// - Cereals (Wheat, Barley, Oats, etc.): Orange (#FFF3E0)
        /// - Oilseeds (Canola, Flax, Sunflower, etc.): Green (#E8F5E9)
        /// - Pulses (Peas, Lentils, Chickpeas, etc.): Blue (#E3F2FD)
        /// - Forages (Alfalfa, Hay, Grasses, etc.): Purple (#F3E5F5)
        /// - Fallow: Gray (#FAFAFA)
        /// - Unknown/Default: Light Gray (#F5F5F5)
        /// 
        /// Provided by the ICropColorService.GetCropColorHex() method.
        /// This creates visual consistency across the timeline (Step 2) and preview (Step 3).
        /// </summary>
        public IBrush CropBackground { get; set; }
        
        /// <summary>
        /// The actual crop type enumeration value for this assignment.
        /// 
        /// This property is used for selection matching:
        /// - When user clicks a crop card in the timeline (Step 2)
        /// - All cells in the preview grid (Step 3) with matching CropType are highlighted
        /// - This allows users to visualize where a specific crop appears across all fields and years
        /// 
        /// Example: If user clicks "Wheat" card, all cells where CropType == CropType.Wheat
        /// will have IsSelected set to true, triggering the blue border highlight.
        /// </summary>
        public CropType CropType { get; set; }
        
        /// <summary>
        /// Reference to the actual crop DTO from the rotation sequence.
        /// This allows the cell to provide the crop data for editing when clicked.
        /// </summary>
        public ICropDto CropDto { get; set; }
        
        /// <summary>
        /// Indicates whether this cell represents the currently selected crop type from the timeline.
        /// 
        /// Selection behavior:
        /// - When true: Cell displays with blue border (3px thickness) via BoolToSelectionBorderBrushConverter
        /// - When false: Cell displays with normal gray border (1px thickness)
        /// 
        /// This property is updated by UpdatePreviewCellSelection() when:
        /// - User clicks a crop card in the timeline (Step 2)
        /// - All cells with matching CropType have IsSelected set to true
        /// - All cells with non-matching CropType have IsSelected set to false
        /// 
        /// Property change notifications ensure the UI updates immediately when selection changes.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    #endregion
}
