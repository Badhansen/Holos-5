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
    public class RotationComponentViewModel : ViewModelBase
    {
        #region Fields

        private readonly IFieldComponentService _fieldComponentService;
        private readonly IRotationComponentService _rotationComponentService;
        private readonly ICropFactory _cropFactory;
        private readonly ICropColorService _cropColorService;
        private RotationComponent _selectedRotationComponent;
        private IRotationComponentDto _selectedRotationComponentDto;
        private ObservableCollection<IFieldComponentDto> _fieldComponentDtos;
        private ObservableCollection<ICropDto> _cropDtos;
        private ObservableCollection<FieldAssignmentRow> _fieldAssignmentRows;
        private bool _shiftRotationEnabled = true;

        #endregion

        #region Constructors

        public RotationComponentViewModel()
        {
            this.Construct();
        }

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
            if (cropFactory != null)
            {
                _cropFactory = cropFactory;
            }
            else
            {
                throw new ArgumentNullException(nameof(cropFactory));
            }

            if (fieldComponentService != null)
            {
                _fieldComponentService = fieldComponentService;
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldComponentService));
            }

            if (rotationComponentService != null)
            {
                _rotationComponentService = rotationComponentService;
            }
            else
            {
                throw new ArgumentNullException(nameof(rotationComponentService));
            }

            if (cropColorService != null)
            {
                _cropColorService = cropColorService;
            }
            else
            {
                throw new ArgumentNullException(nameof(cropColorService));
            }

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
            set
            {
                // Unsubscribe from old collection if it exists
                if (_cropDtos != null)
                {
                    _cropDtos.CollectionChanged -= OnCropDtosCollectionChanged;
                    // Unsubscribe from all existing crop property changes
                    foreach (var crop in _cropDtos)
                    {
                        if (crop is INotifyPropertyChanged notifyPropertyChanged)
                        {
                            notifyPropertyChanged.PropertyChanged -= OnCropDtoPropertyChanged;
                        }
                    }
                }

                SetProperty(ref _cropDtos, value);

                // Subscribe to new collection
                if (_cropDtos != null)
                {
                    _cropDtos.CollectionChanged += OnCropDtosCollectionChanged;
                    // Subscribe to all crop property changes
                    foreach (var crop in _cropDtos)
                    {
                        if (crop is INotifyPropertyChanged notifyPropertyChanged)
                        {
                            notifyPropertyChanged.PropertyChanged += OnCropDtoPropertyChanged;
                        }
                    }
                }

                // Regenerate field assignments when collection changes
                GenerateFieldAssignmentRows();
            }
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
        /// Whether rotation shifting is enabled (crops rotate across fields)
        /// </summary>
        public bool ShiftRotationEnabled
        {
            get => _shiftRotationEnabled;
            set
            {
                if (SetProperty(ref _shiftRotationEnabled, value))
                {
                    GenerateFieldAssignmentRows();
                }
            }
        }

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
        /// Command to add a new crop to the rotation
        /// </summary>
        public ICommand AddCropToRotationCommand { get; private set; }

        /// <summary>
        /// Command to set the selected crop when a timeline card is clicked
        /// </summary>
        public ICommand SetSelectedCropCommand { get; private set; }

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

        private void Construct()
        {
            this.FieldComponentDtos = new ObservableCollection<IFieldComponentDto>();
            this.CropDtos = new ObservableCollection<ICropDto>();
            this.FieldAssignmentRows = new ObservableCollection<FieldAssignmentRow>();

            // Initialize commands
            this.AddCropToRotationCommand = new DelegateCommand(OnAddCropToRotation);
            this.SetSelectedCropCommand = new DelegateCommand<object>(OnSetSelectedCropExecute);
            this.RemoveSpecificCropCommand = new DelegateCommand<object>(OnRemoveSpecificCropExecute);
        }

        /// <summary>
        /// Generates field assignment rows dynamically based on rotation parameters
        /// </summary>
        protected virtual void GenerateFieldAssignmentRows()
        {
            if (FieldAssignmentRows == null)
            {
                FieldAssignmentRows = new ObservableCollection<FieldAssignmentRow>();
            }
            else
            {
                FieldAssignmentRows.Clear();
            }

            if (CropDtos == null || !CropDtos.Any() || SelectedRotationComponentDto == null)
            {
                RaisePropertyChanged(nameof(HasNoCrops));
                return;
            }

            var crops = CropDtos.ToList();
            var rotationLength = crops.Count;
            var startYear = SelectedRotationComponentDto.StartYear;
            var endYear = SelectedRotationComponentDto.EndYear;
            var fieldArea = SelectedRotationComponentDto.FieldArea;
            var numberOfFields = SelectedRotationComponentDto.NumberOfFields;

            if (startYear <= 0 || endYear <= 0 || endYear <= startYear)
            {
                RaisePropertyChanged(nameof(HasNoCrops));
                return;
            }

            var totalYears = endYear - startYear + 1;

            for (int fieldIndex = 0; fieldIndex < numberOfFields; fieldIndex++)
            {
                var row = new FieldAssignmentRow
                {
                    FieldName = $"Field {fieldIndex + 1} ({fieldArea:F0} ha)",
                    YearAssignments = new ObservableCollection<YearCropAssignment>()
                };

                // Calculate the shift offset for this field
                int shiftOffset = ShiftRotationEnabled ? fieldIndex : 0;

                // Generate year assignments for this field
                for (int yearIndex = 0; yearIndex < totalYears; yearIndex++)
                {
                    int year = startYear + yearIndex;

                    // Apply rotation shift: wrap around if we go past the end
                    int cropIndex = (yearIndex + shiftOffset) % rotationLength;
                    var crop = crops[cropIndex];

                    var assignment = new YearCropAssignment
                    {
                        Year = year.ToString(),
                        CropType = crop.CropType, // Store crop type for selection matching
                        CropDisplay = _cropColorService?.GetCropDisplayName(crop.CropType) ?? crop.CropType.ToString(),
                        CropBackground = _cropColorService != null 
                            ? Brush.Parse(_cropColorService.GetCropColorHex(crop.CropType))
                            : Brush.Parse("#F5F5F5"),
                        IsSelected = false // Initialize to not selected
                    };

                    row.YearAssignments.Add(assignment);
                }

                FieldAssignmentRows.Add(row);
            }

            RaisePropertyChanged(nameof(HasNoCrops));
        }

        /// <summary>
        /// Sets the selected crop when a timeline card is clicked
        /// </summary>
        /// <param name="obj">The crop DTO to select</param>
        private void OnSetSelectedCropExecute(object obj)
        {
            if (!IsDisposed && obj is ICropDto cropDto)
            {
                // Update IsSelected property on all crops
                UpdateCropSelectionStates(cropDto);
                
                // Update IsSelected property on all cells in the preview grid
                UpdatePreviewCellSelection(cropDto);
            }
        }

        /// <summary>
        /// Removes a specific crop from the rotation
        /// </summary>
        /// <param name="obj">The crop DTO to remove</param>
        private void OnRemoveSpecificCropExecute(object obj)
        {
            if (!IsDisposed && obj is ICropDto cropDto)
            {
                try
                {
                    // Remove the specific crop from the collection
                    if (this.CropDtos != null && this.CropDtos.Contains(cropDto))
                    {
                        this.CropDtos.Remove(cropDto);

                        // Ensure consecutive ordering (by year) of all crops now that one has been removed
                        if (this.CropDtos != null && this.CropDtos.Any())
                        {
                            _fieldComponentService.ResetAllYears(this.CropDtos);
                        }

                        // If the removed crop was selected, select another crop
                        var wasSelected = cropDto.IsSelected;
                        if (wasSelected && this.CropDtos.Any())
                        {
                            var newSelectedCrop = this.CropDtos.Last();
                            UpdateCropSelectionStates(newSelectedCrop);
                        }
                        else if (!this.CropDtos.Any())
                        {
                            // Clear selection if no crops remain
                            UpdateCropSelectionStates(null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "Failed to remove specific crop from rotation");
                }
            }
        }

        /// <summary>
        /// Updates the IsSelected property on all crops based on the currently selected crop
        /// </summary>
        /// <param name="selectedCrop">The currently selected crop DTO (can be null to clear all selections)</param>
        private void UpdateCropSelectionStates(ICropDto selectedCrop)
        {
            if (this.CropDtos != null)
            {
                foreach (var crop in this.CropDtos)
                {
                    crop.IsSelected = selectedCrop != null && crop == selectedCrop;
                }
            }
        }

        /// <summary>
        /// Updates the IsSelected property on all preview grid cells based on the currently selected crop
        /// </summary>
        /// <param name="selectedCrop">The currently selected crop DTO (can be null to clear all selections)</param>
        private void UpdatePreviewCellSelection(ICropDto selectedCrop)
        {
            if (this.FieldAssignmentRows == null)
            {
                return;
            }

            var selectedCropType = selectedCrop?.CropType;

            // Update IsSelected on all cells across all fields
            foreach (var row in this.FieldAssignmentRows)
            {
                if (row.YearAssignments != null)
                {
                    foreach (var assignment in row.YearAssignments)
                    {
                        // Cell is selected if it has the same crop type as the selected card
                        assignment.IsSelected = selectedCropType.HasValue && 
                                                assignment.CropType == selectedCropType.Value;
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
            
            // Select the newly added crop
            UpdateCropSelectionStates(newCropDto);
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

            GenerateFieldAssignmentRows();
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
    /// Represents a row in the field assignment preview showing one field's crop assignments across years
    /// </summary>
    public class FieldAssignmentRow
    {
        public string FieldName { get; set; }
        public ObservableCollection<YearCropAssignment> YearAssignments { get; set; }
    }

    /// <summary>
    /// Represents a single year/crop assignment cell in the preview grid
    /// </summary>
    public class YearCropAssignment : Prism.Mvvm.BindableBase
    {
        private bool _isSelected;

        public string Year { get; set; }
        public string CropDisplay { get; set; }
        public IBrush CropBackground { get; set; }
        public CropType CropType { get; set; } // Store the crop type for selection matching
        
        /// <summary>
        /// Indicates whether this cell represents the currently selected crop type
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    #endregion
}
