using H.Core.Enumerations;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;

namespace H.Avalonia.ViewModels.ComponentViews.OtherAnimals
{
    public class HorsesComponentViewModel : OtherAnimalsViewModelBase
    {
        #region Constructors

        public HorsesComponentViewModel(ILogger logger, IAnimalComponentService componentService, IStorageService storageService, IManagementPeriodService managementPeriodService) : base(logger, componentService, storageService, managementPeriodService)
        {
            ViewName = "Horses";
            OtherAnimalType = AnimalType.Horses;
        }

        public HorsesComponentViewModel() 
        { 
        
        }

        #endregion
    }
}