using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DryIoc;
using H.Avalonia.Infrastructure;
using H.Avalonia.Infrastructure.DependencyInjection;
using H.Avalonia.Services;
using H.Avalonia.ViewModels;
using H.Avalonia.Views;
using H.Avalonia.Views.FarmCreationViews;
using H.Avalonia.Views.SupportingViews;
using H.Avalonia.Views.SupportingViews.Disclaimer;
using H.Avalonia.Views.SupportingViews.MeasurementProvince;
using H.Core;
using H.Core.Enumerations;
using H.Core.Providers;
using H.Core.Services;
using H.Core.Services.Countries;
using H.Core.Services.StorageService;
using H.Infrastructure;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using System;
using KmlHelpers = H.Avalonia.Infrastructure.KmlHelpers;

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
            // Set up logging first as it's needed for error handling
            SetUpLogging(containerRegistry);

            try
            {
                var logger = Container.Resolve<ILogger>();
                
                // Create and use the registration service to handle all type registrations
                var registrationService = new ContainerRegistrationService(Container, logger);
                registrationService.RegisterAllTypes(containerRegistry);

                logger.LogInformation("All container types registered successfully");
            }
            catch (Exception ex)
            {
                var logger = Container.Resolve<ILogger>();
                logger.LogError(ex, "Failed to register container types: {ErrorMessage}", ex.Message);
                throw;
            }
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
    }
}