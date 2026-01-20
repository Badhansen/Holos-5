using H.Core.Factories.Crops;
using H.Core.Factories.Fields;
using H.Core.Factories.Rotations;
using H.Core.Models;
using H.Core.Models.LandManagement.Fields;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using H.Avalonia.Views.ComponentViews;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement.Rotation
{
    public class RotationComponentViewModel : ViewModelBase
    {
        #region Fields

        private readonly IFieldComponentService _fieldComponentService;
        private readonly IRotationComponentService _rotationComponentService;
        private readonly ICropFactory _cropFactory;
        private RotationComponent _selectedRotationComponent;
        private IRotationComponentDto _selectedRotationComponentDto;
        private ObservableCollection<IFieldComponentDto> _fieldComponentDtos;
        private ObservableCollection<ICropDto> _cropDtos;

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

        public IRotationComponentDto SelectedRotationComponentDto
        {
            get => _selectedRotationComponentDto;
            set => SetProperty(ref _selectedRotationComponentDto, value);
        }

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
        }

        #endregion

        #region Event Handlers

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
}
