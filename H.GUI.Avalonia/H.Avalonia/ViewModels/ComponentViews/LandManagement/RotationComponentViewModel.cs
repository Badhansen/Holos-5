using System;
using System.Collections.ObjectModel;
using H.Core.Factories;
using H.Core.Factories.Crops;
using H.Core.Models;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ComponentViews.LandManagement
{
    public class RotationComponentViewModel : ViewModelBase
    {
        #region Fields

        private readonly IFieldComponentService _fieldComponentService;
        private readonly ILogger _logger;
        private readonly ICropFactory _cropFactory;
        private RotationComponent _selectedRotationComponent;
        private ObservableCollection<IFieldComponentDto> _fieldComponentDtos;

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

            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (fieldComponentService != null)
            {
                _fieldComponentService = fieldComponentService;
            }
            else
            {
                throw new ArgumentNullException(nameof(fieldComponentService));
            }

            this.Construct();
        }

        #endregion

        #region Properties

        public ObservableCollection<IFieldComponentDto> FieldComponentDtos
        {
            get => _fieldComponentDtos;
            set => SetProperty(ref _fieldComponentDtos, value);
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
