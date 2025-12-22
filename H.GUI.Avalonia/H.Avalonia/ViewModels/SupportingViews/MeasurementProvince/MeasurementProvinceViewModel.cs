using H.Core.Enumerations;
using H.Infrastructure;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using H.Core.Services.Provinces;
using System;


namespace H.Avalonia.ViewModels.SupportingViews.MeasurementProvince
{
    public class MeasurementProvinceViewModel : ViewModelBase
    {
        #region Fields

        private MeasurementSystemType _selectedMeasurementSystem;
        private object _selectedProvince;
        private readonly IRegionManager _regionManager;
        private readonly IProvinces _provincesService;

        #endregion

        #region Constructors

        public MeasurementProvinceViewModel(IRegionManager regionManager, IProvinces provincesService) : base(regionManager)
        {




            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            _provincesService = provincesService ?? throw new ArgumentNullException(nameof(provincesService));



            MeasurementSystemCollection = new ObservableCollection<MeasurementSystemType>(EnumHelper.GetValues<MeasurementSystemType>());
            ProvinceCollection = new ObservableCollection<object>(_provincesService.GetProvinces());

            // Set default selected province to the first one or a default value
            _selectedProvince = ProvinceCollection.FirstOrDefault() ?? new object();
            SelectedProvince = _selectedProvince;

            NavigateCommand = new DelegateCommand(OnNavigate);
        }

        #endregion

        #region Properties

        public ObservableCollection<MeasurementSystemType> MeasurementSystemCollection { get; set; }
        public ObservableCollection<object> ProvinceCollection { get; set; }

        public MeasurementSystemType SelectedMeasurementSystem
        {
            get { return _selectedMeasurementSystem; }
            set { SetProperty(ref _selectedMeasurementSystem, value); }
        }

        public object SelectedProvince
        {
            get { return _selectedProvince; }
            set { SetProperty(ref _selectedProvince, value); }
        }

        public ICommand NavigateCommand { get; }

        #endregion

        #region Methods

        private void OnNavigate()
        {               
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(Views.BlankView));                     
        }

        #endregion
    }
}