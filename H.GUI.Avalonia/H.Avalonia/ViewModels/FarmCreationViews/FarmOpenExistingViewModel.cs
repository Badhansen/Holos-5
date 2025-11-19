using DynamicData;
using H.Avalonia.Services;
using H.Avalonia.Views.ComponentViews;
using H.Avalonia.Views.FarmCreationViews;
using H.Core.Models;
using H.Core.Services.StorageService;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Linq;

namespace H.Avalonia.ViewModels.FarmCreationViews
{
    public class FarmOpenExistingViewmodel : ViewModelBase
    {
        #region Fields
        private readonly IRegionManager _regionManager;
        private Farm _selectedFarm;
        private string _searchText;
        private ObservableCollection<Farm> _farms;

        #endregion

        #region Constructors
        public FarmOpenExistingViewmodel()
        {
            
        }
        public FarmOpenExistingViewmodel(IRegionManager regionManager, IStorageService storageService) : base(regionManager, storageService)
        {
            _regionManager = regionManager ?? throw new System.ArgumentNullException(nameof(regionManager));
            NavigateToPreviousPage = new DelegateCommand(OnNavigateToPreviousPage);
            NavigateToNextPage = new DelegateCommand(OnOpenFarmExecute, NextCanExecute);
            Farms = new ObservableCollection<Farm>();
        }

        public FarmOpenExistingViewmodel(IRegionManager regionManager, IStorageService storageService, INotificationManagerService notificationManager) : base(regionManager, storageService, notificationManager)
        {
            _regionManager = regionManager ?? throw new System.ArgumentNullException(nameof(regionManager));
            NavigateToPreviousPage = new DelegateCommand(OnNavigateToPreviousPage);
            NavigateToNextPage = new DelegateCommand(OnOpenFarmExecute, NextCanExecute);
            Farms = new ObservableCollection<Farm>();
        }

        #endregion

        #region Properties

        public DelegateCommand NavigateToPreviousPage { get; }
        public DelegateCommand NavigateToNextPage { get; }
        public ObservableCollection<Farm> Farms
        {
            get => _farms;
            set
            {
                SetProperty(ref _farms, value);
            }
        }

        public Farm SelectedFarm
        {
            get => _selectedFarm;
            set
            {
                SetProperty(ref _selectedFarm, value);
                NavigateToNextPage.RaiseCanExecuteChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                if (string.IsNullOrEmpty(value))
                {
                    Farms.Clear();
                    var farms = base.StorageService.GetAllFarms();
                    Farms.Add(farms);
                }
                else
                {
                    Farms.Clear();
                    var farms = base.StorageService.GetAllFarms().Where(f => f.Name.ToLower().Contains(value.ToLower()) || f.DefaultSoilData.EcodistrictName.ToLower().Contains(value.ToLower()) || f.Province.ToString().ToLower().Contains(value.ToLower()));
                    Farms.Add(farms);
                }
            }
        }

        #endregion

        #region Public Methods

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Farms.Clear();
            var farms = base.StorageService.GetAllFarms();
            Farms.Add(farms);
            base.OnNavigatedTo(navigationContext);
        }

        #endregion

        #region Private Methods

        private void OnNavigateToPreviousPage()
        {
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(FarmOptionsView));
        }

        private void OnOpenFarmExecute()
        {
            base.StorageService.SetActiveFarm(this.SelectedFarm);
            // Line below ensures that the proper unit strings are used for the MeasurementSystemType of the existing farm being opened
            base.StorageService.Storage.ApplicationData.DisplayUnitStrings.SetStrings(this.SelectedFarm.MeasurementSystemType);

            this.ClearActiveView(); // likely solves the bug: System.InvalidOperationException: 'Sequence contains no elements'
            base.RegionManager.RequestNavigate(UiRegions.SidebarRegion, nameof(MyComponentsView));
        }

        private void ClearActiveView()
        {
            // Clear content region
            var contentView = this.RegionManager.Regions[UiRegions.ContentRegion].ActiveViews.SingleOrDefault();
            if (contentView != null)
            {
                this.RegionManager.Regions[UiRegions.ContentRegion].Deactivate(contentView);
                this.RegionManager.Regions[UiRegions.ContentRegion].Remove(contentView);
            }
        }

        #endregion

        #region Event Handler

        private bool NextCanExecute()
        {
            return this.SelectedFarm != null;
        }

        #endregion
    }
}
