using H.Core.Enumerations;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;

namespace H.Avalonia.ViewModels.ComponentViews.OtherAnimals
{
    public class DeerComponentViewModel : OtherAnimalsViewModelBase
    {
        #region Constructors

        public DeerComponentViewModel(ILogger logger, IAnimalComponentService componentService, IStorageService storageService, IManagementPeriodService managementPeriodService) : base(logger, componentService, storageService, managementPeriodService)
        {
            ViewName = "Deer";
            OtherAnimalType = AnimalType.Deer;
        }

        public DeerComponentViewModel()
        {

        }

        #endregion
    }
}
