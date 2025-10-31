using H.Core.Enumerations;
using H.Core.Services.Animals;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;

namespace H.Avalonia.ViewModels.ComponentViews.OtherAnimals
{
    public class LlamaComponentViewModel : OtherAnimalsViewModelBase
    {
        #region

        public LlamaComponentViewModel(ILogger logger, IAnimalComponentService componentService, IStorageService storageService, IManagementPeriodService managementPeriodService) : base(logger, componentService, storageService, managementPeriodService)
        {
            ViewName = "Llamas";
            OtherAnimalType = AnimalType.Llamas;
        }

        public LlamaComponentViewModel()
        {

        }

        #endregion
    }
}
