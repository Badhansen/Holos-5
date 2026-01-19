using System;
using System.Collections.ObjectModel;
using H.Core.Factories;
using H.Core.Factories.Crops;
using H.Core.Factories.Fields;
using H.Core.Factories.Rotations;
using H.Core.Models;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement
{
    public class RotationComponentViewModel : ViewModelBase
    {
        #region Fields

        private readonly IFieldComponentService _fieldComponentService;
        private readonly IRotationComponentService _rotationComponentService;
        private readonly ICropFactory _cropFactory;
        private RotationComponent _selectedRotationComponent;
        private IRotationComponentDto _rotationComponentDto;
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

        public IRotationComponentDto RotationComponentDto
        {
            get => _rotationComponentDto;
            set => SetProperty(ref _rotationComponentDto, value);
        }

        #endregion

        #region Public Methods

        public override void InitializeViewModel(ComponentBase component)
        {
            if (component is not RotationComponent rotationComponent)
            {
                return;
            }

            base.InitializeViewModel(component);
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
        }

        #endregion

        #region Private Methods

        private void Construct()
        {
            this.FieldComponentDtos = new ObservableCollection<IFieldComponentDto>();
        }

        #endregion
    }
}
