using System;
using H.Core.Factories.Crops;
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

        #endregion

        #region Constructors

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
        }

        #endregion
    }
}
