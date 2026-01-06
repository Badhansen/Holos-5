using Avalonia.Controls.Notifications;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using H.Avalonia.Infrastructure;
using H.Avalonia.Infrastructure.Dialogs;
using H.Avalonia.Models;
using H.Avalonia.Models.ClassMaps;
using H.Avalonia.Views;
using H.Avalonia.Views.ResultViews;
using H.Core.Enumerations;
using Mapsui;
using Mapsui.Extensions;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using SharpKml.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using H.Avalonia.Services;
using H.Avalonia.Views.ResultViews;
using H.Core.Services;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using SoilResultsView = H.Avalonia.Views.ResultViews.SoilResultsView;

namespace H.Avalonia.ViewModels
{
    public class SoilDataViewModel : ViewModelBase, IDataGridFeatures
    {
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal? _navigationJournal;
        private readonly MapHelpers _mapHelpers;
        private readonly IDialogService _dialogService;
        private readonly IDefaultGeocoderService _defaultGeocoderService;
        private double _longitude;
        private double _latitude;
        private string _address = string.Empty;
        private MPoint _navigationPoint;
        private ImportHelpers _importHelper;
        private SoilViewItemMap _soilViewItemMap;

        private ObservableCollection<Province> _provinces;

        public bool HasViewItems => StoragePlaceholder?.SoilViewItems != null && StoragePlaceholder.SoilViewItems.Any();

        public bool AnyViewItemsSelected => StoragePlaceholder?.SoilViewItems != null &&
                                            StoragePlaceholder.SoilViewItems.Any(item => item.IsSelected);
        public bool AllViewItemsSelected { get; set; }

        private readonly KmlHelpers _kmlHelpers;

        public readonly Dictionary<Province, List<Polygon>> WktPolygonMap = new();
        private bool _isDataProcessing;
        private bool _showPolygonsOnMap;
        private Province _selectedProvince;
        private ICountrySettings _countrySettings;

        /// <summary>
        /// The longitude value of a coordinate
        /// </summary>
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        /// <summary>
        /// The latitude value of a coordinate
        /// </summary>
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        /// <summary>
        /// An address field that signifies a location.
        /// </summary>
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        /// <summary>
        /// A point that the user wants to navigate to. This point is what the user selects when they specify a location in the <see cref="SoilDataView"/>
        /// </summary>
        public MPoint NavigationPoint
        {
            get => _navigationPoint;
            set => SetProperty(ref _navigationPoint, value);
        }

        /// <summary>
        /// A bool that indicates if data is still processing in the background. If data is still loading/processing, then return true. Returns false if all
        /// data is loaded and ready to use.
        /// </summary>
        public bool IsDataProcessing
        {
            get => _isDataProcessing;
            set => SetProperty(ref _isDataProcessing, value);
        }

        /// <summary>
        /// A collection of provinces for which SLC polygon data is available.
        /// </summary>
        public ObservableCollection<Province> Provinces 
        {
            get => _provinces;
            set => SetProperty(ref _provinces, value);
        }

        public SoilDataViewModel() { }

        /// <summary>
        /// A constructor that uses dependency injection to pass various objects into the class.
        /// </summary>
        /// <param name="regionManager">The region manager object controls the navigation of our view.</param>
        /// <param name="storage">The storage object contains various items that are passed between different viewmodels</param>
        /// <param name="importHelper">A set of methods that help with importing data from an external file.</param>
        /// <param name="kmlHelpers">A set of methods that help us process .kml files.</param>
        /// <param name="dialogService">A Prism dialogService object that helps us display dialogs to the user.</param>
        /// <param name="countrySettings"></param>
        public SoilDataViewModel(
            IRegionManager regionManager, 
            ImportHelpers importHelper, 
            KmlHelpers kmlHelpers, 
            IDialogService dialogService, 
            ICountrySettings countrySettings,
            IStorageService storageService,
            Storage storage,
            INotificationManagerService notificationManager,
            ILogger logger,
            IDefaultGeocoderService defaultGeocoderService) : base(regionManager, storageService, notificationManager, logger)
        {
            if (countrySettings != null)
            {
                _countrySettings = countrySettings; 
            }
            else
            {
                throw new ArgumentNullException(nameof(countrySettings));
            }

            if (defaultGeocoderService != null)
            {
                _defaultGeocoderService = defaultGeocoderService;
            }
            else
            {
                throw new ArgumentNullException(nameof(defaultGeocoderService));
            }

            this.StoragePlaceholder = storage;

            _regionManager = regionManager;
            _importHelper = importHelper;
            _dialogService = dialogService;
            _kmlHelpers = kmlHelpers;
            _mapHelpers = new MapHelpers();
            _soilViewItemMap = new SoilViewItemMap();
            InitializeCommands();
            CreateWktPolygons();
        }

        public override void InitializeViewModel()
        {
            base.InitializeViewModel();

            this.Provinces = new ObservableCollection<Province>(_countrySettings.GetProvinces());
            this.SelectedProvince = Province.SelectProvince;
            base.RaisePropertyChanged(nameof(this.SelectedProvince));
        }

        /// <summary>
        /// Initializes the various commands used by the related view.
        /// </summary>
        private void InitializeCommands()
        {
            SwitchToResultsViewFromSingleCoordinateCommand = new DelegateCommand(SwitchToSoilResultsViewFromSingleCoordinate);
            SwitchToResultsViewFromMultiCoordinateCommand =
                new DelegateCommand(SwitchToSoilResultsViewFromMultiCoordinate).ObservesCanExecute(() => HasViewItems);
            ImportFromCsvCommand = new DelegateCommand<object>(OnImportCsv);
            ToggleSelectAllRowsCommand = new DelegateCommand(OnToggleSelectAllRows).ObservesCanExecute(() => HasViewItems);
            AddRowCommand = new DelegateCommand(OnAddRow);
            DeleteRowCommand = new DelegateCommand<object>(OnDeleteRow);
            DeleteSelectedRowsCommand = new DelegateCommand(OnDeleteSelectedRows).ObservesCanExecute(() => AnyViewItemsSelected);
            GetCoordinatesFromAddressCommand = new DelegateCommand(OnGetCoordinates);
            GetAddressFromCoordinateCommand = new DelegateCommand(OnGetAddress);
            UpdateNavigationPointCommand = new DelegateCommand<MPoint>(OnUpdateNavigationPoint);
            UpdateInformationFromNavigationPointCommand = new DelegateCommand(OnUpdateInformationFromNavPoint);
        }

        /// <summary>
        /// Triggered when a user navigates to this page.
        /// </summary>
        /// <param name="navigationContext">The navigation context of the user. Contains the navigation tree and journal</param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // When we navigate to this view, we instantiate the journal property. This allows us to do navigation through journaling.
            _navigationJournal = navigationContext.NavigationService.Journal;
            if (StoragePlaceholder?.SoilViewItems != null)
            {
                StoragePlaceholder.SoilViewItems.CollectionChanged += OnSoilViewItemsCollectionChanged;
            }

            this.InitializeViewModel();
        }

        /// <summary>
        /// The province currently selected by the user.
        /// </summary>
        public Province SelectedProvince
        {
            get => _selectedProvince;
            set => SetProperty(ref _selectedProvince, value);
        }

        /// <summary>
        /// Command switches the current <see cref="SoilDataView"/> from the single coordinate tab to the results section.
        /// </summary>
        public DelegateCommand SwitchToResultsViewFromSingleCoordinateCommand { get; set; }

        /// <summary>
        /// Command switches the current <see cref="SoilDataView"/> from the multiple coordinate tab to the results section.
        /// </summary>
        public DelegateCommand SwitchToResultsViewFromMultiCoordinateCommand { get; set; }

        /// <summary>
        /// Selects or deselects all rows added to the grid by the user.
        /// </summary>
        public DelegateCommand ToggleSelectAllRowsCommand { get; set; }

        /// <summary>
        /// Adds a row to the grid displayed to the user.
        /// </summary>
        public DelegateCommand AddRowCommand { get; set; }

        /// <summary>
        /// Triggered by the user when they click the delete icon next to a row. Deletes that specific row only.
        /// </summary>
        public DelegateCommand<object> DeleteRowCommand { get; set; }

        /// <summary>
        /// Imports inputs from a csv file for which soil data is required. This csv file must have the following headers:
        /// Longitude, Latitude
        /// </summary>
        public DelegateCommand<object> ImportFromCsvCommand { get; set; }

        /// <summary>
        /// Deletes a selection of rows that are marked as selected by the user.
        /// </summary>
        public DelegateCommand DeleteSelectedRowsCommand { get; set; }

        /// <summary>
        /// Get the coordinates of a location based on the address specified by the user.
        /// </summary>
        public DelegateCommand GetCoordinatesFromAddressCommand { get; set; }

        /// <summary>
        /// Get the address of a location based on the coordinates specified by the user.
        /// </summary>
        public DelegateCommand GetAddressFromCoordinateCommand { get; set; }

        /// <summary>
        /// Updates the point that the user wants to navigate to when the user clicks a specific area on the world map.
        /// </summary>
        public DelegateCommand<MPoint> UpdateNavigationPointCommand { get; private set; }

        /// <summary>
        /// Updates the <see cref="Latitude"/>, <see cref="Longitude"/> and <see cref="Address"/> fields when the user selects
        /// a new navigation point.
        /// </summary>
        public DelegateCommand UpdateInformationFromNavigationPointCommand { get; private set; }

        /// <summary>
        /// Triggered when the <see cref="Storage.SoilViewItems"/> changes. This method raises CanExecuteChanged events for the various
        /// buttons on the page and also attaches/detaches PropertyChanged events to individual properties inside the collection so that
        /// we can be notified when an internal property changes in the collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSoilViewItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleSelectAllRowsCommand.RaiseCanExecuteChanged();
            DeleteSelectedRowsCommand.RaiseCanExecuteChanged();
            SwitchToResultsViewFromMultiCoordinateCommand.RaiseCanExecuteChanged();
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    if (item != null)
                        item.PropertyChanged += CollectionItemOnPropertyChanged;
                }
                AllViewItemsSelected = false;
            }

            if (e.OldItems == null) return;
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    if (item != null)
                        item.PropertyChanged -= CollectionItemOnPropertyChanged;
                }
            }
        }

        /// <summary>
        /// A property changed event that is attached to each property of the <see cref="Storage.ClimateViewItems"/> collection.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event that was triggered.</param>
        private void CollectionItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SoilViewItem.IsSelected))
            {
                DeleteSelectedRowsCommand.RaiseCanExecuteChanged();

                if (sender is not SoilViewItem viewItem) return;
                if (!viewItem.IsSelected)
                {
                    AllViewItemsSelected = false;
                }
            }
        }

        /// <summary>
        /// Helps select all rows that are currently added to the grid.
        /// </summary>
        private void OnToggleSelectAllRows()
        {
            if (StoragePlaceholder?.SoilViewItems == null) return;
            if (AllViewItemsSelected)
            {
                foreach (var item in StoragePlaceholder.SoilViewItems)
                {
                    item.IsSelected = false;
                }
                AllViewItemsSelected = false;
            }
            else
            {
                foreach (var item in StoragePlaceholder.SoilViewItems)
                {
                    item.IsSelected = true;
                }

                AllViewItemsSelected = true;
            }
        }

        /// <summary>
        /// Adds a row to the grid.
        /// </summary>
        private void OnAddRow()
        {
            StoragePlaceholder?.SoilViewItems?.Add(new SoilViewItem());
        }

        /// <summary>
        /// Deletes a row from a grid.
        /// </summary>
        /// <param name="obj"></param>
        private void OnDeleteRow(object obj)
        {
            if (obj is not SoilViewItem viewItem) return;
            var message = Core.Properties.Resources.RowDeleteMessage;
            _dialogService.ShowMessageDialog(nameof(DeleteRowDialog), message, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    StoragePlaceholder?.SoilViewItems?.Remove(viewItem);
                }
            });
        }

        /// <summary>
        /// Called when the user imports a csv file. The imported csv file must have the following column headers:
        /// Latitude, Longitude (respectively).
        /// </summary>
        /// <param name="obj">The <see cref="IStorageItem"/> object passed to the method containing the file path where the csv is located.</param>
        private void OnImportCsv(object obj)
        {
            var item = obj as IReadOnlyCollection<IStorageItem>;
            var file = item?.FirstOrDefault();

            if (file == null) return;

            _importHelper.ImportPath = file.Path.AbsolutePath;
            try
            {
                StoragePlaceholder?.SoilViewItems.AddRange(_importHelper.ImportFromCsv(_soilViewItemMap));

            }
            catch (HeaderValidationException e)
            {
                NotificationManager.ShowToast(H.Core.Properties.Resources.InvalidHeaderTitle, e.Message, NotificationType.Error);
            }
            catch (TypeConverterException e)
            {
                NotificationManager.ShowToast(H.Core.Properties.Resources.InvalidCSVContentTitle, e.Message, NotificationType.Error);
            }
            catch (IOException e)
            {
                NotificationManager.ShowToast(H.Core.Properties.Resources.FileInUse, e.Message, NotificationType.Error);
            }
        }

        /// <summary>
        /// Deletes the rows marked as selected by the user
        /// </summary>
        private void OnDeleteSelectedRows()
        {
            if (!StoragePlaceholder.SoilViewItems.Any()) return;
            var message = Core.Properties.Resources.RowDeleteMessage;
            _dialogService.ShowMessageDialog(nameof(DeleteRowDialog), message, r =>
            {
                if (r.Result != ButtonResult.OK) return;
                var currentItems = StoragePlaceholder.SoilViewItems.ToList();
                foreach (var item in currentItems.Where(item => item.IsSelected))
                {
                    StoragePlaceholder?.SoilViewItems?.Remove(item);
                }

                if (!HasViewItems)
                {
                    AllViewItemsSelected = false;
                }
            });
        }

        /// <summary>
        /// Sets the coordinates of the active farm based on the current latitude and longitude values.
        /// </summary>
        private void SetCoordinates()
        {
            var activeFarm = this.StorageService.GetActiveFarm();
            if (activeFarm != null)
            {
                activeFarm.Latitude = this.Latitude;
                activeFarm.Longitude = this.Longitude;
            }
        }

        /// <summary>
        /// Switch to <see cref="SoilResultsView"/> from the current single page coordinate tab.
        /// </summary>
        private void SwitchToSoilResultsViewFromSingleCoordinate()
        {
            this.SetCoordinates();

            StoragePlaceholder.SingleSoilViewItem.Latitude = Latitude;
            StoragePlaceholder.SingleSoilViewItem.Longitude = Longitude;
            StoragePlaceholder.ShowSingleCoordinateResults = true;
            StoragePlaceholder.ShowMultipleCoordinateResults = false;
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(SoilResultsView));
        }

        /// <summary>
        /// Switch to <see cref="SoilResultsView"/> from the current multiple page coordinate tab.
        /// </summary>
        private void SwitchToSoilResultsViewFromMultiCoordinate()
        {
            this.SetCoordinates();

            StoragePlaceholder.ShowMultipleCoordinateResults = true;
            StoragePlaceholder.ShowSingleCoordinateResults = false;
            _regionManager.RequestNavigate(UiRegions.ContentRegion, nameof(SoilResultsView));
        }

        /// <summary>
        /// Gets the new navigation point based on the update latitude and longitude values.
        /// </summary>
        /// <returns></returns>
        private MPoint GetNavigationPoint()
        {
            var coordinate = _mapHelpers.ConvertLatLongtToSphericalMercator(Latitude, Longitude);
            return (coordinate.x, coordinate.y).ToMPoint();
        }

        /// <summary>
        /// Gets the new coordinate values based on the address provided by the user.
        /// </summary>
        private async void OnGetCoordinates()
        {
            if (string.IsNullOrEmpty(Address))
            {
                Logger.LogDebug($@"Cannot find location as an empty address was entered.");
                NotificationManager.ShowToast(H.Core.Properties.Resources.AddressFieldEmpty, Core.Properties.Resources.MessageEmptyAddress, NotificationType.Information);
                return;
            }
            try
            {
                // Call the geocoding service to get coordinates from the address, return early if problem encountered.
                Logger.LogInformation($"Attempting coordinate acquisition from address in {nameof(SoilDataViewModel)}.{nameof(OnGetAddress)}");
                var point = await _defaultGeocoderService.GetCoordinates(Address);
                if (point.latitude == 0 || point.longitude == 0)
                {
                    Logger.LogDebug($@"Cannot find the coordinate from the address entered.");
                    NotificationManager.ShowToast(H.Core.Properties.Resources.CoordinateError, H.Core.Properties.Resources.CantFindCoordinate, NotificationType.Error);
                    return;
                }
                // Call the geocoding service to get province from the address, return early if problem encountered.
                var province = await _defaultGeocoderService.GetProvince(Address);
                if (province == null)
                {
                    Logger.LogDebug($@"Cannot find the province from the address entered.");
                    NotificationManager.ShowToast(H.Core.Properties.Resources.InvalidAddress, H.Core.Properties.Resources.CantFindAddress, NotificationType.Error);
                    return;
                }
                // Load province polygon and set coordinates.
                SelectedProvince = (Province)province;
                Logger.LogInformation($"Coordinate acquired from address in {nameof(SoilDataViewModel)}.{nameof(OnGetAddress)}");
                Latitude = point.latitude;
                Longitude = point.longitude;
                NavigationPoint = GetNavigationPoint();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.LogError($@"{e.Message}. Exception thrown in {nameof(OnGetCoordinates)} by class {nameof(SoilDataViewModel)}");
                NotificationManager.ShowToast(H.Core.Properties.Resources.InvalidAddress, Core.Properties.Resources.MessageIncorrectAddress, NotificationType.Error);
            }
        }

        /// <summary>
        /// Gets the new address values based on the coordinates provided by the user.
        /// </summary>
        private async void OnGetAddress()
        {
            var address = await _mapHelpers.GetAddressFromLocationAsync(Latitude, Longitude);
            if (string.IsNullOrEmpty(address))
            {
                Logger.LogDebug($@"Cannot find the coordinate. Please enter correct latitude and longitude values");
                NotificationManager.ShowToast(H.Core.Properties.Resources.IncorrectCoordinate, Core.Properties.Resources.MessageInValidCoordinateEntered, NotificationType.Information);
                return;
            }

            Address = address;
            NavigationPoint = GetNavigationPoint();
        }

        /// <summary>
        /// Updates the navigation point based on user choice.
        /// </summary>
        /// <param name="point">The new point that is to be set as the navigation point.</param>
        private void OnUpdateNavigationPoint(MPoint? point)
        {
            if (point is null) return;

            NavigationPoint = point;
        }

        /// <summary>
        /// Updates various information displayed to the user when the navigation point updates.
        /// </summary>
        private async void OnUpdateInformationFromNavPoint()
        {
            var point = _mapHelpers.ConvertSphericalMercatorToCoordinate(NavigationPoint);
            Latitude = point.latitude;
            Longitude = point.longitude;

            var address = await _mapHelpers.GetAddressFromLocationAsync(Latitude, Longitude);
            if (string.IsNullOrEmpty(address))
            {
                Logger.LogDebug($@"Incorrect coordinate location cannot find matching address.");
                NotificationManager.ShowToast(H.Core.Properties.Resources.CantFindAddress, Core.Properties.Resources.MessageIncorrectLocationSelected, NotificationType.Information);
                return;
            }
            Address = address;

        }

        /// <summary>
        /// Calls the <see cref="CreateWktPolygonsAsync"/> method to create a WKT (Well-known-text) representation of all the SLC polygons.
        /// </summary>
        private async void CreateWktPolygons()
        {
            IsDataProcessing = true;
            if (_kmlHelpers.LoadPolygonsAsync != null) await _kmlHelpers.LoadPolygonsAsync;
            await CreateWktPolygonsAsync();
            IsDataProcessing = false;
        }

        /// <summary>
        /// Creates a WKT (Well-known-text) representation of all the SLC polygons in the province KML files.
        /// </summary>
        private async Task CreateWktPolygonsAsync()
        {
            await Task.Run(() =>
            {
                foreach (var (province, polygons) in _kmlHelpers.PolygonMap)
                {
                    var result = new List<Polygon>();
                    foreach (var polygonItem in polygons)
                    {
                        var wktPolygon = polygonItem.sharpKmlPolygon.ToWkt();
                        var polygon = (Polygon)new WKTReader().Read(wktPolygon);
                        result.Add(polygon);
                    }
                    WktPolygonMap.TryAdd(province, result);
                }
            });
        }
    }
}
