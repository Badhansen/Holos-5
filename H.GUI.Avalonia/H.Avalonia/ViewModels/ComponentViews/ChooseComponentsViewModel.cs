using H.Avalonia.ViewModels;
using H.Avalonia.Views;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using H.Avalonia.Events;
using H.Core.Models;
using H.Core.Models.Animals.Sheep;
using H.Core.Models.LandManagement.Fields;  
using H.Core.Models.LandManagement.Rotation;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Shelterbelt;
using H.Core.Models.Animals.Dairy;
using H.Core.Models.Animals.OtherAnimals;
using H.Core.Models.Infrastructure;
using H.Core.Models.Animals.Swine;
using H.Core.Models.Animals.Poultry.Chicken;
using H.Core.Models.Animals.Poultry.Turkey;
using H.Core.Services.StorageService;
using H.Infrastructure;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace H.Avalonia.ViewModels.ComponentViews
{
    public class ChooseComponentsViewModel : ViewModelBase
    {
        #region Fields

        private string _selectedComponentTitle;
        private string _selectedComponentDescription;

        private ComponentBase _selectedComponent;

        private ObservableCollection<ComponentBase> _availableComponents;

        #endregion

        #region Constructors

        public ChooseComponentsViewModel()
        {
            this.AvailableComponents = new ObservableCollection<ComponentBase>();
            InitializeAvailableComponents();
            this.SelectedComponent = this.AvailableComponents.First();
        }

        public ChooseComponentsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IStorageService storageService, ILogger logger) : base(regionManager, eventAggregator, storageService, logger)
        {
            this.PropertyChanged += OnPropertyChanged;
            this.AvailableComponents = new ObservableCollection<ComponentBase>();
            InitializeAvailableComponents();
            this.SelectedComponent = this.AvailableComponents.First();
        }

        #endregion

        #region Properties

        public string SelectedComponentTitle
        {
            get => _selectedComponentTitle;
            set => SetProperty(ref _selectedComponentTitle, value);
        }

        public string SelectedComponentDescription
        {
            get => _selectedComponentDescription;
            set => SetProperty(ref _selectedComponentDescription, value);
        }

        public ObservableCollection<ComponentBase> AvailableComponents
        {
            get => _availableComponents;
            set => SetProperty(ref _availableComponents, value);
        }

        public ComponentBase SelectedComponent
        {
            get => _selectedComponent;
            set => SetProperty(ref _selectedComponent, value);
        }

        #endregion

        #region Public Methods

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);                 

            this.InitializeViewModel();
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            base.OnNavigatedFrom(navigationContext);
        }

        public void InitializeViewModel()
        {
            this.SelectedComponent = this.AvailableComponents.First();
        }

        #endregion

        #region Private Methods

        private void UpdateComponentDescription()
        {
        }

        private void InitializeAvailableComponents()
        {
            /*
             * Land Management
             */

            _availableComponents.Add(new RotationComponent());
            _availableComponents.Add(new ShelterbeltComponent());
            _availableComponents.Add(new FieldSystemComponent());
            
            /*
             * Beef production
             */

            _availableComponents.Add(new CowCalfComponent());
            _availableComponents.Add(new BackgroundingComponent());
            _availableComponents.Add(new FinishingComponent());

            /*
             * Dairy cattle
             */

            _availableComponents.Add(new DairyComponent());

            /*
             * Swine
             */

            _availableComponents.Add(new GrowerToFinishComponent());
            _availableComponents.Add(new FarrowToWeanComponent());
            _availableComponents.Add(new IsoWeanComponent());
            _availableComponents.Add(new FarrowToFinishComponent());

            /*
             * Sheep
             */

            // _availableComponents.Add(new SheepComponent());
            _availableComponents.Add(new SheepFeedlotComponent());
            _availableComponents.Add(new RamsComponent());
            _availableComponents.Add(new EwesAndLambsComponent());

            /*
             * Other animals / livestock
             */

            _availableComponents.Add(new GoatsComponent());
            _availableComponents.Add(new DeerComponent());
            _availableComponents.Add(new HorsesComponent());
            _availableComponents.Add(new MulesComponent());
            _availableComponents.Add(new BisonComponent());
            _availableComponents.Add(new LlamaComponent());
            
            /*
             * Poultry
             */

            _availableComponents.Add(new ChickenPulletsComponent());
            _availableComponents.Add(new ChickenMultiplierBreederComponent());
            _availableComponents.Add(new ChickenMeatProductionComponent());
            _availableComponents.Add(new TurkeyMultiplierBreederComponent());
            _availableComponents.Add(new TurkeyMeatProductionComponent());
            _availableComponents.Add(new ChickenEggProductionComponent());
            _availableComponents.Add(new ChickenMultiplierHatcheryComponent());

            /* 
             * Land Management
             */

            _availableComponents.Add(new AnaerobicDigestionComponent());
        }

        #endregion

        #region Event Handlers

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(this.SelectedComponent))
            {
                this.SelectedComponentTitle = this.SelectedComponent.ComponentType.GetDescription();
                this.SelectedComponentDescription = this.SelectedComponent.ComponentDescriptionString;
            }
        }

        public void OnAddComponentExecute()
        {
            base.EventAggregator.GetEvent<ComponentAddedEvent>().Publish(this.SelectedComponent);
        }

        public void OnFinishedAddingComponentsExecute()
        {
            var view = this.RegionManager.Regions[UiRegions.ContentRegion].ActiveViews.Single();
            this.RegionManager.Regions[UiRegions.ContentRegion].Deactivate(view);
            this.RegionManager.Regions[UiRegions.ContentRegion].Remove(view);

            base.EventAggregator.GetEvent<EditingComponentsCompletedEvent>().Publish();
        }

        #endregion
    }
}
