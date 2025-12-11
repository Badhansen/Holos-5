using AutoMapper;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using H.Avalonia.Infrastructure;
using H.Avalonia.Infrastructure.Dialogs;
using H.Avalonia.Infrastructure.MapperServices;
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
using H.Avalonia.ViewModels.SupportingViews;
using H.Avalonia.ViewModels.SupportingViews.CountrySelection;
using H.Avalonia.ViewModels.SupportingViews.Disclaimer;
using H.Avalonia.ViewModels.SupportingViews.MeasurementProvince;
using H.Avalonia.ViewModels.SupportingViews.RegionSelection;
using H.Avalonia.ViewModels.SupportingViews.Start;
using H.Avalonia.Views;
using H.Avalonia.Views.ComponentViews;
using H.Avalonia.Views.ComponentViews.LandManagement.Field;
using H.Avalonia.Views.FarmCreationViews;
using H.Avalonia.Views.ResultViews;
using H.Avalonia.Views.SupportingViews;
using H.Avalonia.Services;
using H.Avalonia.Views.SupportingViews.CountrySelection;
using H.Avalonia.Views.SupportingViews.Disclaimer;
using H.Avalonia.Views.SupportingViews.MeasurementProvince;
using H.Avalonia.Views.SupportingViews.RegionSelection;
using H.Core;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Factories.FarmFactory;
using H.Core.Mappers;
using H.Core.Models.Animals;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers;
using H.Core.Providers.Energy;
using H.Core.Providers.Feed;
using H.Core.Services;
using H.Core.Services.Animals;
using H.Core.Services.Countries;
using H.Core.Services.DietService;
using H.Core.Services.Initialization;
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
using H.Core.Factories.Crops;
using H.Core.Factories.Climate;
using H.Core.Services.Climate;
using H.Core.Models.Climate;
using H.Core.Providers.Climate;
using System.Threading.Tasks;
using Avalonia.Controls;
using ClimateResultsView = H.Avalonia.Views.ResultViews.ClimateResultsView;
using KmlHelpers = H.Avalonia.Infrastructure.KmlHelpers;
using SoilResultsView = H.Avalonia.Views.ResultViews.SoilResultsView;
using DryIoc;
using H.Avalonia.Views.ComponentViews.Beef;
using H.Avalonia.Views.ComponentViews.Dairy;
using H.Avalonia.Views.ComponentViews.Infrastructure;
using H.Avalonia.Views.ComponentViews.LandManagement;
using H.Avalonia.Views.ComponentViews.OtherAnimals;
using H.Avalonia.Views.ComponentViews.Poultry;
using H.Avalonia.Views.ComponentViews.Sheep;
using H.Avalonia.Views.ComponentViews.Swine;
using H.Avalonia.Views.OptionsViews;
using H.Avalonia.Views.OptionsViews.FileMenuViews;
using H.Avalonia.Views.SupportingViews.Start;

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
            #region Storage Registrations

            // V5 object
            containerRegistry.RegisterSingleton<Storage>();

            // V4 object
            containerRegistry.RegisterSingleton<IStorage, H.Core.Storage>();

            containerRegistry.RegisterSingleton<IStorageService, DefaultStorageService>();


            var storage = Container.Resolve<IStorage>();
            storage.Load();
            var storageService = Container.Resolve<IStorageService>();
            var activeFarm = storageService.GetActiveFarm();



            #endregion

            // Logging
            SetUpLogging(containerRegistry);

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
            containerRegistry.RegisterForNavigation<FieldComponentView, FieldComponentViewModel>();
            containerRegistry.RegisterForNavigation<OptionsView, OptionsViewModel>();
            containerRegistry.RegisterForNavigation<SelectOptionView, SelectOptionViewModel>();
            containerRegistry.RegisterForNavigation<OptionFarmView, FarmSettingsViewModel>();
            containerRegistry.RegisterForNavigation<OptionUserSettingsView, UserSettingsViewModel>();
            containerRegistry.RegisterForNavigation<OptionSoilView, SoilSettingsViewModel>();
            containerRegistry.RegisterForNavigation<OptionSoilN2OBreakdownView, SoilN2OBreakdownSettingsViewModel>();
            containerRegistry.RegisterForNavigation<DefaultBeddingCompositionView, DefaultBeddingCompositionViewModel>();
            containerRegistry.RegisterForNavigation<DefaultManureCompositionView, DefaultManureCompositionViewModel>();
            containerRegistry.RegisterForNavigation<OptionPrecipitationView, PrecipitationSettingsViewModel>();

            // New development work
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterForNavigation<OptionEvapotranspirationView, EvapotranspirationSettingsViewModel>();
            containerRegistry.RegisterForNavigation<OptionTemperatureView, TemperatureSettingsViewModel>();
            containerRegistry.RegisterForNavigation<OptionBarnTemperatureView, BarnTemperatureSettingsViewModel>();
            containerRegistry.RegisterForNavigation<DisclaimerView, DisclaimerViewModel>();
            containerRegistry.RegisterForNavigation<RegionSelectionView, RegionSelectionViewModel>();
            containerRegistry.RegisterForNavigation<MeasurementProvinceView, MeasurementProvinceViewModel>();
            containerRegistry.RegisterForNavigation<CountrySelectionView, CountrySelectionViewModel>();
            containerRegistry.RegisterForNavigation<FarmOptionsView,FarmOptionsViewModel>();
            containerRegistry.RegisterForNavigation<FarmCreationView, FarmCreationViewModel>();
            containerRegistry.RegisterForNavigation<FarmOpenExistingView, FarmOpenExistingViewmodel>();
            containerRegistry.RegisterForNavigation<SheepComponentView, SheepComponentViewModel>();
            containerRegistry.RegisterForNavigation<RotationComponentView, RotationComponentViewModel>();
            containerRegistry.RegisterForNavigation<SheepFeedlotComponentView, SheepFeedlotComponentViewModel>();
            containerRegistry.RegisterForNavigation<ShelterbeltComponentView, ShelterbeltComponentViewModel>();
            containerRegistry.RegisterForNavigation<CowCalfComponentView, CowCalfComponentViewModel>();
            containerRegistry.RegisterForNavigation<BackgroundingComponentView, BackgroundingComponentViewModel>();
            containerRegistry.RegisterForNavigation<FinishingComponentView, FinishingComponentViewModel>();
            containerRegistry.RegisterForNavigation<DairyComponentView, DairyComponentViewModel>();
            containerRegistry.RegisterForNavigation<RamsComponentView, RamsComponentViewModel>();
            containerRegistry.RegisterForNavigation<LambsAndEwesComponentView, LambsAndEwesComponentViewModel>();
            containerRegistry.RegisterForNavigation<GoatsComponentView, GoatsComponentViewModel>();
            containerRegistry.RegisterForNavigation<DeerComponentView, DeerComponentViewModel>();
            containerRegistry.RegisterForNavigation<HorsesComponentView, HorsesComponentViewModel>();
            containerRegistry.RegisterForNavigation<MulesComponentView, MulesComponentViewModel>();
            containerRegistry.RegisterForNavigation<BisonComponentView, BisonComponentViewModel>();
            containerRegistry.RegisterForNavigation<LlamaComponentView, LlamaComponentViewModel>();
            containerRegistry.RegisterForNavigation<AnaerobicDigestionComponentView, AnaerobicDigestionComponentViewModel>();
            containerRegistry.RegisterForNavigation<GrowerToFinishComponentView, GrowerToFinishComponentViewModel>();
            containerRegistry.RegisterForNavigation<FarrowToWeanComponentView, FarrowToWeanComponentViewModel>();
            containerRegistry.RegisterForNavigation<IsoWeanComponentView, IsoWeanComponentViewModel>();
            containerRegistry.RegisterForNavigation<FarrowToFinishComponentView, FarrowToFinishComponentViewModel>();
            containerRegistry.RegisterForNavigation<ChickenPulletsComponentView, ChickenPulletsComponentViewModel>();
            containerRegistry.RegisterForNavigation<ChickenMultiplierBreederComponentView, ChickenMultiplierBreederComponentViewModel>();
            containerRegistry.RegisterForNavigation<ChickenMeatProductionComponentView, ChickenMeatProductionComponentViewModel>();
            containerRegistry.RegisterForNavigation<TurkeyMultiplierBreederComponentView, TurkeyMultiplierBreederComponentViewModel>();
            containerRegistry.RegisterForNavigation<TurkeyMeatProductionComponentView, TurkeyMeatProductionComponentViewModel>();
            containerRegistry.RegisterForNavigation<ChickenEggProductionComponentView, ChickenEggProductionComponentViewModel>();
            containerRegistry.RegisterForNavigation<ChickenMultiplierHatcheryComponentView, ChickenMultiplierHatcheryComponentViewModel>();
            containerRegistry.RegisterForNavigation<StartView, StartViewModel>();
            containerRegistry.RegisterForNavigation<FileNewFarmView, FileNewFarmViewModel>();
            containerRegistry.RegisterForNavigation<FileOpenFarmView, FileOpenFarmViewModel>();
            containerRegistry.RegisterForNavigation<FarmManagementView, FarmManagementViewModel>();
            containerRegistry.RegisterForNavigation<FileSaveOptionsView, FileSaveOptionsViewModel>();
            containerRegistry.RegisterForNavigation<FileExportFarmView, FileExportFarmViewModel>();
            containerRegistry.RegisterForNavigation<FileImportFarmView, FileImportFarmViewModel>();
            containerRegistry.RegisterForNavigation<FarmImportFileView, FarmImportFileViewModel>();
            containerRegistry.RegisterForNavigation<FileExportClimateView, FileExportClimateViewModel>();
            containerRegistry.RegisterForNavigation<FileExportManureView, FileExportManureViewModel>();

            // Diet
            containerRegistry.RegisterForNavigation<DietFormulatorView, DietFormulatorViewModel>();
            containerRegistry.RegisterForNavigation<FeedIngredientsView, FeedIngredientsViewModel>();

            // Blank Page
            containerRegistry.RegisterForNavigation<BlankView, BlankViewModel>();

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
            containerRegistry.RegisterSingleton<IClimateProvider, ClimateProvider>();
            containerRegistry.RegisterSingleton<ISlcClimateProvider, SlcClimateDataProvider>();

            // Services
            containerRegistry.RegisterSingleton<IFarmHelper, FarmHelper>();
            containerRegistry.RegisterSingleton<IComponentInitializationService, ComponentInitializationService>();
            containerRegistry.RegisterSingleton<IFieldComponentService, FieldComponentService>();
            containerRegistry.RegisterSingleton<IRotationComponentService, RotationComponentService>();
            containerRegistry.RegisterSingleton<IClimateService, ClimateService>();
            containerRegistry.RegisterSingleton<IFarmResultsService_NEW, FarmResultsService_NEW>();
            containerRegistry.RegisterSingleton<IDietService, DefaultDietService>();
            containerRegistry.RegisterSingleton<ICropInitializationService, CropInitializationService>();
            containerRegistry.RegisterSingleton<IAnimalComponentService, AnimalComponentService>();
            containerRegistry.RegisterSingleton<IManagementPeriodService, ManagementPeriodService>();
            containerRegistry.RegisterSingleton<IErrorHandlerService, ErrorHandlerService>();
            containerRegistry.RegisterSingleton<INotificationManagerService, NotificationManagerService>();
            containerRegistry.RegisterSingleton<IDefaultGeocoderService, NominatimGeocoderService>();

            // Unit conversion
            containerRegistry.RegisterSingleton<IUnitsOfMeasurementCalculator, UnitsOfMeasurementCalculator>();
            
            // Dialogs
            containerRegistry.RegisterDialog<DeleteRowDialog, DeleteRowDialogViewModel>();

            // Factories
            containerRegistry.RegisterSingleton<IDietFactory, DietFactory>();
            containerRegistry.RegisterSingleton<IFarmFactory, FarmFactory>();
            containerRegistry.RegisterSingleton<IManagementPeriodFactory, ManagementPeriodFactory>();
            containerRegistry.RegisterSingleton<IDailyClimateDataFactory, DailyClimateDataFactory>();

            containerRegistry.Register(typeof(IFactory<CropDto>), typeof(CropFactory));
            containerRegistry.Register(typeof(IFactory<FieldSystemComponentDto>), typeof(FieldFactory));
            containerRegistry.Register(typeof(IFactory<AnimalComponentDto>), typeof(AnimalComponentFactory));
            containerRegistry.Register(typeof(IFactory<DailyClimateDto>), typeof(DailyClimateDataFactory));

            containerRegistry.Register(typeof(ICropFactory), typeof(CropFactory));
            containerRegistry.Register(typeof(IFieldFactory), typeof(FieldFactory));
            containerRegistry.RegisterSingleton<IAnimalComponentFactory, AnimalComponentFactory>();

            // Tables
            containerRegistry.RegisterSingleton<ITable50FuelEnergyEstimatesProvider, Table50FuelEnergyEstimatesProvider>();

            // Mappers
            var mapperRegistrationService = new MapperRegistrationService();
            mapperRegistrationService.RegisterMappers(containerRegistry);

            SetUpCaching(containerRegistry);

            SetupTransferServices(containerRegistry);
        }

        protected override AvaloniaObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>Called after Initialize.</summary>
        protected override void OnInitialized()
        {
            SetLanguage();

            // Register views to the Region it will appear in. Don't register them in the ViewModel.
            var regionManager = Container.Resolve<IRegionManager>();

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
        }

        private void SetLanguage()
        {
            var settings = Container.Resolve<ICountrySettings>();
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
            
            // Configure DryIoc logging for resolution errors
            ConfigureDryIocLogging(containerRegistry, logger);
        }

        private void ConfigureDryIocLogging(IContainerRegistry containerRegistry, ILogger logger)
        {
            try
            {
                // Access the underlying DryIoc container
                if (containerRegistry is DryIocContainerExtension dryIocExtension)
                {
                    var container = dryIocExtension.Instance;
                    
                    // Configure DryIoc with enhanced error reporting
                    var newContainer = container.With(rules => rules
                        .WithCaptureContainerDisposeStackTrace()
                        .WithTrackingDisposableTransients()
                        .WithDefaultReuse(Reuse.Transient));
                    
                    logger.LogInformation("DryIoc container logging configured successfully");
                }
                else
                {
                    logger.LogWarning("Unable to configure DryIoc logging - container is not a DryIocContainerExtension");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to configure DryIoc logging: {ErrorMessage}", ex.Message);
            }
        }

        private void SetupTransferServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(ITransferService<,>), typeof(TransferService<,>));

            // Register TransferService for DailyClimateData and DailyClimateDto using the named mapper
            containerRegistry.Register<ITransferService<DailyClimateData, DailyClimateDto>>(() =>
            {
                var unitsCalculator = Container.Resolve<IUnitsOfMeasurementCalculator>();
                var dailyClimateDataFactory = Container.Resolve<IFactory<DailyClimateDto>>();
                var dtoToModelMapper = Container.Resolve<IMapper>(nameof(DailyClimateDtoToDailyClimateDataMapper));
                var modelToDtoMapper = Container.Resolve<IMapper>(nameof(DailyClimateDataToDailyClimateDtoMapper));

                return new TransferService<DailyClimateData, DailyClimateDto>(
                    unitsOfMeasurementCalculator: unitsCalculator,
                    dtoFactory: dailyClimateDataFactory,
                    dtoToModelMapper: dtoToModelMapper,
                    modelToDtoMapper: modelToDtoMapper
                );
            });

            // Register TransferService for CropViewItem and CropDto using the named mapper
            containerRegistry.Register<ITransferService<CropViewItem, CropDto>>(() =>
            {
                var unitsCalculator = Container.Resolve<IUnitsOfMeasurementCalculator>();
                var cropDtoFactory = Container.Resolve<IFactory<CropDto>>();
                var dtoToModelMapper = Container.Resolve<IMapper>(nameof(CropDtoToCropViewItemMapper));
                var modelToDtoMapper = Container.Resolve<IMapper>(nameof(CropViewItemToCropDtoMapper));

                // If TransferService supports injecting IMapper, pass it here.
                return new TransferService<CropViewItem, CropDto>(
                    unitsOfMeasurementCalculator: unitsCalculator,
                    dtoFactory: cropDtoFactory,
                    dtoToModelMapper: dtoToModelMapper,
                    modelToDtoMapper: modelToDtoMapper
                );
            });

            // Register TransferService for CropViewItem and CropDto using the named mapper
            containerRegistry.Register<ITransferService<FieldSystemComponent, FieldSystemComponentDto>>(() =>
            {
                var unitsCalculator = Container.Resolve<IUnitsOfMeasurementCalculator>();
                var cropDtoFactory = Container.Resolve<IFactory<FieldSystemComponentDto>>();
                var dtoToModelMapper = Container.Resolve<IMapper>(nameof(FieldDtoToFieldComponentMapper));
                var modelToDtoMapper = Container.Resolve<IMapper>(nameof(FieldComponentToDtoMapper));

                // If TransferService supports injecting IMapper, pass it here.
                return new TransferService<FieldSystemComponent, FieldSystemComponentDto>(
                    unitsOfMeasurementCalculator: unitsCalculator,
                    dtoFactory: cropDtoFactory,
                    dtoToModelMapper: dtoToModelMapper,
                    modelToDtoMapper: modelToDtoMapper
                );
            });


            // Register TransferService for CropViewItem and CropDto using the named mapper
            containerRegistry.Register<ITransferService<AnimalComponentBase, AnimalComponentDto>>(() =>
            {
                var unitsCalculator = Container.Resolve<IUnitsOfMeasurementCalculator>();
                var animalDtoFactory = Container.Resolve<IFactory<AnimalComponentDto>>();
                var dtoToModelMapper = Container.Resolve<IMapper>(nameof(AnimalComponentDtoToAnimalComponentMapper));
                var modelToDtoMapper = Container.Resolve<IMapper>(nameof(AnimalComponentBaseToAnimalComponentDtoMapper));

                // If TransferService supports injecting IMapper, pass it here.
                return new TransferService<AnimalComponentBase, AnimalComponentDto>(
                    unitsOfMeasurementCalculator: unitsCalculator,
                    dtoFactory: animalDtoFactory,
                    dtoToModelMapper: dtoToModelMapper,
                    modelToDtoMapper: modelToDtoMapper
                );
            });
        }
    }
}