using H.Avalonia.Services;
using H.Avalonia.ViewModels.OptionsViews.FileMenuViews;
using H.Avalonia.Views.FarmCreationViews;
using H.Core.Services.StorageService;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.Avalonia.ViewModels.FarmCreationViews
{
    public class FarmImportFileViewModel : FileImportFarmViewModel
    {
        #region Fields
        private readonly IRegionManager _regionManager;
        #endregion

        public FarmImportFileViewModel(IRegionManager regionManager, IStorageService storageService, INotificationManagerService notificationManager) : base(regionManager, storageService, notificationManager)
        {
            _regionManager = regionManager ?? throw new System.ArgumentNullException(nameof(regionManager));
            NavigateToPreviousPage = new DelegateCommand(OnNavigateToPreviousPage);
        }

        #region Properties

        public DelegateCommand NavigateToPreviousPage { get; }
        #endregion

        #region Private Methods
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Farms.Clear();
            IsFarmImported = false;
            ShowGrid = false;
            base.OnNavigatedTo(navigationContext);
        }

        private void OnNavigateToPreviousPage()
        {
            if(IsFarmImported)
            {
                _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(FarmOpenExistingView));
                return;
            }
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(FarmOptionsView));
        }
        #endregion
    }
}
