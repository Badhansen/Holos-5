using AutoMapper;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using H.Avalonia.Infrastructure;
using H.Avalonia.Infrastructure.Dialogs;
using H.Avalonia.ViewModels;
using H.Avalonia.ViewModels.ComponentViews;
using H.Avalonia.ViewModels.ComponentViews.Beef;
using H.Avalonia.ViewModels.ComponentViews.Dairy;
using H.Avalonia.ViewModels.ComponentViews.Infrastructure;
using H.Avalonia.ViewModels.ComponentViews.LandManagement;
using H.Avalonia.ViewModels.ComponentViews.LandManagement.Field;
using H.Avalonia.ViewModels.ComponentViews.OtherAnimals;
using H.Avalonia.ViewModels.ComponentViews.Poultry;
using H.Avalonia.ViewModels.ComponentViews.Sheep;
using H.Avalonia.ViewModels.ComponentViews.Swine;
using H.Avalonia.ViewModels.FarmCreationViews;
using H.Avalonia.ViewModels.OptionsViews;
using H.Avalonia.ViewModels.OptionsViews.FileMenuViews;
using H.Avalonia.ViewModels.Results;
using H.Avalonia.ViewModels.SupportingViews.CountrySelection;
using H.Avalonia.ViewModels.SupportingViews.Disclaimer;
using H.Avalonia.ViewModels.SupportingViews.MeasurementProvince;
using H.Avalonia.ViewModels.SupportingViews.RegionSelection;
using H.Avalonia.ViewModels.SupportingViews.Start;
using H.Avalonia.Views;
using H.Avalonia.Views.ComponentViews;
using H.Avalonia.Views.FarmCreationViews;
using H.Avalonia.Services;
using H.Avalonia.Views.SupportingViews.CountrySelection;
using H.Avalonia.Views.SupportingViews.Disclaimer;
using H.Avalonia.Views.SupportingViews.MeasurementProvince;
using H.Avalonia.Views.SupportingViews.RegionSelection;
using H.Core;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Mappers;
using H.Core.Providers;
using H.Core.Providers.Feed;
using H.Core.Services;
using H.Core.Services.Countries;
using H.Core.Services.DietService;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.Provinces;
using H.Core.Services.StorageService;
using H.Infrastructure;
using H.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using ClimateResultsView = H.Avalonia.Views.ResultViews.ClimateResultsView;
using KmlHelpers = H.Avalonia.Infrastructure.KmlHelpers;
using SoilResultsView = H.Avalonia.Views.ResultViews.SoilResultsView;

namespace H.Avalonia
{
    public partial class App : PrismApplication
    {
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            base.Initialize();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Resolve through Prism so ViewModelLocator can run
                desktop.MainWindow = (Window)CreateShell();
                desktop.Exit += OnExit;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            var storage = Container.Resolve<IStorage>();
            if (storage != null)
            {
                storage.Save();
            }
        }

        /// <summary>Register Services and Views.</summary>
        /// <param name="containerRegistry"></param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Logging
            this.SetUpLogging(containerRegistry);

            // Views - Region Navigation
            containerRegistry.RegisterForNavigation<ToolbarView, ToolbarViewModel>();
            containerRegistry.RegisterForNavigation<SidebarView, SidebarViewModel>();
            containerRegistry.RegisterForNavigation<FooterView, FooterViewModel>();
            containerRegistry.RegisterForNavigation<ClimateDataView, ClimateDataViewModel>();
            containerRegistry.RegisterForNavigation<SoilDataView, SoilDataViewModel>();
            containerRegistry.RegisterForNavigation<AboutPageView, AboutPageViewModel>();
            containerRegistry.RegisterForNavigation<ClimateResultsView, ClimateResultsViewModel>();
            containerRegistry.RegisterForNavigation<SoilResultsView, SoilResultsViewModel>();
            containerRegistry.RegisterForNavigation<MyComponentsView, MyComponentsViewModel>();
            containerRegistry.RegisterForNavigation<ChooseComponentsView, ChooseComponentsViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.LandManagement.FieldComponentView, FieldComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionsView, OptionsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.SelectOptionView, SelectOptionViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionFarmView, FarmSettingsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionUserSettingsView, UserSettingsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionSoilView, SoilSettingsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionSoilN2OBreakdownView, SoilN2OBreakdownSettingsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.DefaultBeddingCompositionView, DefaultBeddingCompositionViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.DefaultManureCompositionView, DefaultManureCompositionViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionPrecipitationView, PrecipitationSettingsViewModel>();


            // New development work
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionEvapotranspirationView, EvapotranspirationSettingsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionTemperatureView, TemperatureSettingsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.OptionBarnTemperatureView, BarnTemperatureSettingsViewModel>();
            containerRegistry.RegisterForNavigation<DisclaimerView, DisclaimerViewModel>();
            containerRegistry.RegisterForNavigation<RegionSelectionView, RegionSelectionViewModel>();
            containerRegistry.RegisterForNavigation<MeasurementProvinceView, MeasurementProvinceViewModel>();
            containerRegistry.RegisterForNavigation<CountrySelectionView, CountrySelectionViewModel>();
            containerRegistry.RegisterForNavigation<FarmOptionsView,FarmOptionsViewModel>();
            containerRegistry.RegisterForNavigation<FarmCreationView, FarmCreationViewModel>();
            containerRegistry.RegisterForNavigation<FarmOpenExistingView, FarmOpenExistingViewmodel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.LandManagement.SheepComponentView, SheepComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.LandManagement.RotationComponentView, RotationComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Sheep.SheepFeedlotComponentView, SheepFeedlotComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.LandManagement.ShelterbeltComponentView, ShelterbeltComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Beef.CowCalfComponentView, CowCalfComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Beef.BackgroundingComponentView, BackgroundingComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Beef.FinishingComponentView, FinishingComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Dairy.DairyComponentView, DairyComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Sheep.RamsComponentView, RamsComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Sheep.LambsAndEwesComponentView, LambsAndEwesComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.OtherAnimals.GoatsComponentView, GoatsComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.OtherAnimals.DeerComponentView, DeerComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.OtherAnimals.HorsesComponentView, HorsesComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.OtherAnimals.MulesComponentView, MulesComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.OtherAnimals.BisonComponentView, BisonComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.OtherAnimals.LlamaComponentView, LlamaComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Infrastructure.AnaerobicDigestionComponentView, AnaerobicDigestionComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Swine.GrowerToFinishComponentView, GrowerToFinishComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Swine.FarrowToWeanComponentView, FarrowToWeanComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Swine.IsoWeanComponentView, IsoWeanComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Swine.FarrowToFinishComponentView, FarrowToFinishComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Poultry.ChickenPulletsComponentView, ChickenPulletsComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Poultry.ChickenMultiplierBreederComponentView, ChickenMultiplierBreederComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Poultry.ChickenMeatProductionComponentView, ChickenMeatProductionComponentViewModel>(); 
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Poultry.TurkeyMultiplierBreederComponentView, TurkeyMultiplierBreederComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Poultry.TurkeyMeatProductionComponentView, TurkeyMeatProductionComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Poultry.ChickenEggProductionComponentView, ChickenEggProductionComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.ComponentViews.Poultry.ChickenMultiplierHatcheryComponentView,  ChickenMultiplierHatcheryComponentViewModel>();
            containerRegistry.RegisterForNavigation<Views.SupportingViews.Start.StartView, StartViewModel>();
            containerRegistry.RegisterForNavigation<Views.FarmCreationViews.FileNewFarmView, FileNewFarmViewModel>();
            containerRegistry.RegisterForNavigation<Views.FarmCreationViews.FileOpenFarmView, FileOpenFarmViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.FileMenuViews.FarmManagementView, FarmManagementViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.FileMenuViews.FileSaveOptionsView, FileSaveOptionsViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.FileMenuViews.FileExportFarmView, FileExportFarmViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.FileMenuViews.FileImportFarmView, FileImportFarmViewModel>();
            containerRegistry.RegisterForNavigation<Views.FarmCreationViews.FarmImportFileView, FarmImportFileViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.FileMenuViews.FileExportClimateView, FileExportClimateViewModel>();
            containerRegistry.RegisterForNavigation<Views.OptionsViews.FileMenuViews.FileExportManureView, FileExportManureViewModel>();

            // Blank Page
            containerRegistry.RegisterForNavigation<Views.BlankView, BlankViewModel>();

            //containerRegistry.RegisterSingleton<ResultsViewModelBase>();

            #region Storage Registrations

            // V5 object
            containerRegistry.RegisterSingleton<Storage>();

            // V4 object
            containerRegistry.RegisterSingleton<IStorage, H.Core.Storage>();

            containerRegistry.RegisterSingleton<IStorageService, DefaultStorageService>();

            #endregion

            // Providers
            containerRegistry.RegisterSingleton<GeographicDataProvider>();
            containerRegistry.RegisterSingleton<ExportHelpers>();
            containerRegistry.RegisterSingleton<ImportHelpers>();
            containerRegistry.RegisterSingleton<KmlHelpers>();

            containerRegistry.RegisterSingleton<ICountrySettings, CountrySettings>();
            containerRegistry.Register<ICountries, CountriesService>();
            containerRegistry.RegisterSingleton<IProvinces, ProvincesService>();
            containerRegistry.RegisterSingleton<IDietProvider, DietProvider>();
            containerRegistry.RegisterSingleton<IFeedIngredientProvider, FeedIngredientProvider>();

            // Services
            containerRegistry.RegisterSingleton<IFarmHelper, FarmHelper>();
            containerRegistry.RegisterSingleton<IComponentInitializationService, ComponentInitializationService>();
            containerRegistry.RegisterSingleton<IFieldComponentService, FieldComponentService>();
            containerRegistry.RegisterSingleton<IFarmResultsService_NEW, FarmResultsService_NEW>();
            containerRegistry.RegisterSingleton<IDietService, DefaultDietService>();
            containerRegistry.RegisterSingleton<IErrorHandlerService, ErrorHandlerService>();
            containerRegistry.RegisterSingleton<IWindowNotificationManagerService, WindowNotificationManagerService>();

            // Unit conversion
            containerRegistry.RegisterSingleton<IUnitsOfMeasurementCalculator, UnitsOfMeasurementCalculator>();
            
            // Dialogs
            containerRegistry.RegisterDialog<DeleteRowDialog, DeleteRowDialogViewModel>();

            // Factories
            containerRegistry.RegisterSingleton<ICropFactory, CropFactory>();
            containerRegistry.RegisterSingleton<IFieldComponentDtoFactory, FieldComponentDtoFactory>();
            containerRegistry.RegisterSingleton<IDietFactory, DietFactory>();

            // Mappers
            this.SetupMappers(containerRegistry);

            this.SetUpCaching(containerRegistry);
        }

        protected override AvaloniaObject CreateShell()
        {
            return base.Container.Resolve<MainWindow>();
        }

        /// <summary>Called after Initialize.</summary>
        protected override void OnInitialized()
        {
            this.SetLanguage();

            // Register views to the Region it will appear in. Don't register them in the ViewModel.
            var regionManager = base.Container.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(UiRegions.ToolbarRegion, typeof(ToolbarView));
                        
            //regionManager.RegisterViewWithRegion(UiRegions.SidebarRegion, typeof(SidebarView));
            regionManager.RegisterViewWithRegion(UiRegions.FooterRegion, typeof(FooterView));
            regionManager.RegisterViewWithRegion(UiRegions.ContentRegion, typeof(DisclaimerView));
            regionManager.RegisterViewWithRegion(UiRegions.ContentRegion, typeof(MeasurementProvinceView));
            regionManager.RegisterViewWithRegion(UiRegions.ContentRegion, typeof(FarmOptionsView));
            regionManager.RegisterViewWithRegion(UiRegions.ContentRegion, typeof(FarmCreationView));
            regionManager.RegisterViewWithRegion(UiRegions.ContentRegion, typeof(FarmOpenExistingView));

            var geographicProvider = Container.Resolve<GeographicDataProvider>();
            geographicProvider.Initialize();
            Container.Resolve<KmlHelpers>();

            var storage = Container.Resolve<IStorage>();
            storage.Load();
        }

        private void SetLanguage()
        {
            var settings = this.Container.Resolve<ICountrySettings>();
            var language = settings.Language;

            if (language == Languages.French)
            {
                H.Avalonia.Resources.Culture = InfrastructureConstants.FrenchCultureInfo;
                H.Core.Properties.Resources.Culture = InfrastructureConstants.FrenchCultureInfo;
            }
        }

        private void SetUpCaching(IContainerRegistry containerRegistry)
        {
            var options = new MemoryCacheOptions()
            {
                //SizeLimit = long.MaxValue,
            };

            containerRegistry.RegisterSingleton<IMemoryCache>(() => new MemoryCache(options));
            containerRegistry.RegisterSingleton<ICacheService, InMemoryCacheService>();
        }

        private void SetUpLogging(IContainerRegistry containerRegistry)
        {
            // Create a LoggerFactory and add NLog as the logging provider
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders(); // Clear any default providers
                builder.SetMinimumLevel(LogLevel.Trace); // Set your desired minimum log level
                builder.AddNLog(); // Add NLog as the logging provider
            });

            var logger = loggerFactory.CreateLogger<App>();

            // Register the ILogger instance as a singleton in the Prism container
            containerRegistry.RegisterInstance(typeof(ILogger), logger);
        }

        private void SetupMappers(IContainerRegistry containerRegistry)
        {
            var cropDtoToCropDtoConfiguration = new MapperConfiguration(expression =>
            {
                expression.AddProfile<CropDtoCropDtoMapper>();
            });

            var cropDtoToCropVieItemConfiguration = new MapperConfiguration(expression =>
            {
                expression.AddProfile<CropDtoToCropViewItemMapper>();
            });

            var cropViewItemToCropVieItemConfiguration = new MapperConfiguration(expression =>
            {
                expression.AddProfile<CropViewItemToCropDtoMapper>();
            });

            var fieldComponentToFieldDtoConfiguration = new MapperConfiguration(expression =>
            {
                expression.AddProfile<FieldComponentToDtoMapper>();
            });

            var fieldDtoToFieldDtoConfiguration = new MapperConfiguration(expression =>
            {
                expression.AddProfile<FieldDtoToFieldDtoMapper>();
            });

            // Register named mappers
            containerRegistry.RegisterInstance<IMapper>(cropDtoToCropDtoConfiguration.CreateMapper(), nameof(CropDtoCropDtoMapper));
            containerRegistry.RegisterInstance<IMapper>(cropDtoToCropVieItemConfiguration.CreateMapper(), nameof(CropDtoToCropViewItemMapper));
            containerRegistry.RegisterInstance<IMapper>(cropViewItemToCropVieItemConfiguration.CreateMapper(), nameof(CropViewItemToCropDtoMapper));
            containerRegistry.RegisterInstance<IMapper>(fieldComponentToFieldDtoConfiguration.CreateMapper(), nameof(FieldComponentToDtoMapper));
            containerRegistry.RegisterInstance<IMapper>(fieldDtoToFieldDtoConfiguration.CreateMapper(), nameof(FieldDtoToFieldDtoMapper));
        }
    }
}