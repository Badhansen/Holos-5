using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Regions;
using System.Collections.ObjectModel;
using H.Core.Factories.Animals;

namespace H.Avalonia.ViewModels.ComponentViews.OtherAnimals
{
    public abstract class OtherAnimalsViewModelBase : AnimalComponentViewModelBase
    {
        #region Fields

        #endregion

        #region Constructors

        public OtherAnimalsViewModelBase()
        {
            this.Construct();
        }

        public OtherAnimalsViewModelBase(
            ILogger logger, 
            IAnimalComponentService animalComponentService, 
            IStorageService storageService, 
            IManagementPeriodService managementPeriodService) : base(animalComponentService, logger, storageService, managementPeriodService)
        {
            this.Construct();
        }

        private void Construct()
        {

        }

        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
        }

        #endregion

        #region Private Methods



        #endregion
    }
}