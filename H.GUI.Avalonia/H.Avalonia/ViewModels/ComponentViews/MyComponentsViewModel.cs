using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using H.Avalonia.Events;
using H.Avalonia.Models;
using H.Avalonia.Views.ComponentViews;
using H.Core.Models;
using H.Core.Services;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels.ComponentViews;

public class MyComponentsViewModel : ViewModelBase
{
    #region Fields

    private ComponentBase _selectedComponent;
    private ObservableCollection<ComponentBase> _myComponents;
    private H.Core.Models.Farm _selectedFarm;

    private IComponentInitializationService _componentInitializationService;

    #endregion

    #region Constructors

    public MyComponentsViewModel()
    {
        this.MyComponents = new ObservableCollection<ComponentBase>();
    }

    public MyComponentsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IStorageService storageService, IComponentInitializationService componentInitializationService, ILogger logger) : base(regionManager, eventAggregator, storageService, logger)
    {
        if (componentInitializationService != null)
        {
            _componentInitializationService = componentInitializationService; 
        }
        else
        {
            throw new ArgumentNullException(nameof(componentInitializationService));
        }
        
        base.PropertyChanged += OnPropertyChanged;

        this.MyComponents = new ObservableCollection<ComponentBase>();
        
        // Initialize the RemoveComponent command
        RemoveComponent = new DelegateCommand(OnRemoveComponentExecute, OnRemoveComponentCanExecute);
        
        base.EventAggregator.GetEvent<ComponentAddedEvent>().Subscribe(OnComponentAddedEvent);
        base.EventAggregator.GetEvent<EditingComponentsCompletedEvent>().Subscribe(OnEditingComponentsCompletedEvent);
    }

    #endregion

    #region Properties

    public ComponentBase SelectedComponent
    {
        get => _selectedComponent;
        set 
        {
            SetProperty(ref _selectedComponent, value);
            RemoveComponent.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<ComponentBase> MyComponents
    {
        get => _myComponents;
        set => SetProperty(ref _myComponents, value);
    }
    public H.Core.Models.Farm SelectedFarm
    {
        get => _selectedFarm;
        set => SetProperty(ref _selectedFarm, value);
    }

    public DelegateCommand RemoveComponent { get; }

    #endregion

    #region Public Methods

    public override void OnNavigatedTo(NavigationContext navigationContext)
    {
        base.OnNavigatedTo(navigationContext);
        this.InitializeViewModel();
    }

    public void InitializeViewModel()
    {
        if (!base.IsInitialized)
        {
            MyComponents.Clear();
            foreach (var component in base.ActiveFarm.Components)
            {
                this.MyComponents.Add(component);
            }

            base.IsInitialized = true;

            this.SelectedComponent = this.MyComponents.FirstOrDefault();
        }
    }

    #endregion

    #region Event Handlers

    public void OnEditComponentsExecute()
    {
        var activeViews = this.RegionManager.Regions[UiRegions.ContentRegion].ActiveViews;
        if (activeViews != null && activeViews.All(x => x.GetType() != typeof(ChooseComponentsView)))
        {
            this.RegionManager.RequestNavigate(UiRegions.ContentRegion, nameof(ChooseComponentsView));
        }
    }

    public void OnRemoveComponentExecute()
    {
        if (this.SelectedComponent != null)
        {
            // Store the component to remove since the SelectedComponent will be null after removal from the local collection
            var componentToRemove = this.SelectedComponent;

            // Remove from the local collection
            this.MyComponents.Remove(componentToRemove);
            
            // Remove from the farm's Components collection
            base.ActiveFarm.Components.Remove(componentToRemove);

            this.SelectedComponent = this.MyComponents.LastOrDefault();

            if (this.MyComponents.Any() == false)
            {
                this.ClearActiveView();
            }
        }
    }

    private bool OnRemoveComponentCanExecute()
    {
        return this.SelectedComponent != null;
    }

    private void OnComponentAddedEvent(ComponentBase componentBase)
    {
        var instanceType = componentBase.GetType();
        var instance = Activator.CreateInstance(instanceType) as ComponentBase;

        _componentInitializationService.Initialize(instance);

        this.MyComponents.Add(instance);
        this.SelectedComponent = instance;

        base.ActiveFarm.Components.Add(instance);
        base.ActiveFarm.SelectedComponent = instance;
    }

    public void OnOptionsExecute()
    {  
        base.RegionManager.RequestNavigate(UiRegions.SidebarRegion, nameof(Views.OptionsViews.OptionsView));
        base.RegionManager.RequestNavigate(UiRegions.ContentRegion, nameof(Views.OptionsViews.SelectOptionView));
    }

    private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName.Equals(nameof(this.SelectedComponent)))
        {
            System.Diagnostics.Debug.WriteLine($"{SelectedComponent} has been selected");
            var isInEditMode = this.RegionManager.Regions[UiRegions.ContentRegion].ActiveViews.Any(x => x.GetType() == typeof(ChooseComponentsView));
            if (!isInEditMode)
            {
                
                this.NavigateToSelectedComponent();
            }
        }
    }

    private void OnEditingComponentsCompletedEvent()
    {
        this.NavigateToSelectedComponent();
    }

    private void ClearActiveView()
    {
        // Clear current view
        var activeView = this.RegionManager.Regions[UiRegions.ContentRegion].ActiveViews.SingleOrDefault();
        if (activeView != null)
        {
            this.RegionManager.Regions[UiRegions.ContentRegion].Deactivate(activeView);
            this.RegionManager.Regions[UiRegions.ContentRegion].Remove(activeView);
        }
    }

    private void NavigateToSelectedComponent()
    {
        // When the user is finished editing components, navigate to the selected component
        if (this.SelectedComponent != null)
        {
            var viewName = ComponentTypeToViewTypeMapper.GetViewName(this.SelectedComponent);

            var navigationParameters = new NavigationParameters { { GuiConstants.ComponentKey, this.SelectedComponent } };
            this.RegionManager.RequestNavigate(UiRegions.ContentRegion, viewName, navigationParameters);
        }
    }

    #endregion

}