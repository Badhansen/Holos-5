using H.Core.Enumerations;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;

namespace H.Avalonia.ViewModels.ComponentViews.OtherAnimals
{
    public class GoatsComponentViewModel : OtherAnimalsViewModelBase
    {
        #region Constructors

        public GoatsComponentViewModel(ILogger logger, IAnimalComponentService componentService, IStorageService storageService, IManagementPeriodService managementPeriodService) : base(logger, componentService, storageService, managementPeriodService)
        {
            ViewName = "Goats";
            OtherAnimalType = AnimalType.Goats;
        }

        public GoatsComponentViewModel() 
        { 

        }

        #endregion
    }
}
