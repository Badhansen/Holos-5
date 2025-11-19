using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using H.Core.Models;
using H.Core.Services.StorageService;
using Prism.Events;
using System.ComponentModel;
using System.Collections;
using Avalonia;
using Avalonia.Threading;
using System;
using System.Configuration;
using System.Threading.Tasks;
using H.Avalonia.Services;
using H.Core.Helpers;
using Microsoft.Extensions.Logging;

namespace H.Avalonia.ViewModels
{
    public abstract class ViewModelBase : ErrorValidationBase, INavigationAware, INotifyDataErrorInfo
    {
        #region Fields

        protected bool IsInitialized;

        private Storage _storagePlaceholder;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IStorageService _storageService;
        private string _viewName;
        private bool _allowNavigation;
        protected ILogger Logger;

        #endregion

        #region Constructors

        protected ViewModelBase()
        {
        }

        protected ViewModelBase(IStorageService storageService)
        {
            if (storageService != null)
            {
                this.StorageService = storageService;
                this.StorageService.Storage.ApplicationData.GlobalSettings.PropertyChanged += GlobalSettingsPropertyChanged;
            }
            else
            {
                throw new ArgumentNullException(nameof(storageService));
            }
        }

        protected ViewModelBase(IStorageService storageService, INotificationManagerService notificationManager) : this(storageService)
        {
            if (notificationManager != null)
            {
                this.NotificationManager = notificationManager;
            }
            else
            {
                throw new ArgumentNullException(nameof(notificationManager));
            }
        }

        protected ViewModelBase(IStorageService storageService, ILogger logger) : this(storageService)
        {
            if (logger != null)
            {
                Logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }
        }

        protected ViewModelBase(IEventAggregator eventAggregator)
        {
            if (eventAggregator != null)
            {
                this.EventAggregator = eventAggregator;
            }
            else
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }
        }

        protected ViewModelBase(IStorageService storageService, IEventAggregator eventAggregator)
        {
            if(eventAggregator != null)
            {
                this.EventAggregator = eventAggregator;
            }
            else
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }
            if (storageService != null)
            {
                this.StorageService = storageService;
                this.StorageService.Storage.ApplicationData.GlobalSettings.PropertyChanged += GlobalSettingsPropertyChanged;
            }
            else
            {
                throw new ArgumentNullException(nameof(storageService));
            }
        }

        protected ViewModelBase(IRegionManager regionManager, IEventAggregator eventAggregator) : this(eventAggregator)
        {
            if (regionManager != null)
            {
                RegionManager = regionManager;
            }
            else
            {
                throw new ArgumentNullException(nameof(regionManager));
            }
        }

        protected ViewModelBase(IRegionManager regionManager, IStorageService storageService)
        {
            if (storageService != null)
            {
                this.StorageService = storageService;
            }
            else
            {
                throw new ArgumentNullException(nameof(storageService));
            }

            if (regionManager != null)
            {
                RegionManager = regionManager;
            }
            else
            {
                throw new ArgumentNullException(nameof(regionManager));
            }
        }

        protected ViewModelBase(IRegionManager regionManager, IStorageService storageService, ILogger logger) : this(regionManager, storageService)
        {
            if (logger != null)
            {
                this.Logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }
        }

        protected ViewModelBase(IRegionManager regionManager, IStorageService storageService, INotificationManagerService notificationManager) : this(regionManager, storageService)
        {
            if (notificationManager != null)
            {
                this.NotificationManager = notificationManager;
            }
            else
            {
                throw new ArgumentNullException(nameof(notificationManager));
            }
        }

        protected ViewModelBase(IRegionManager regionManager, IStorageService storageService, INotificationManagerService notificationManager , ILogger logger) : this(regionManager, storageService, notificationManager)
        {
            if (logger != null)
            {
                this.Logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }
        }

        protected ViewModelBase(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IStorageService storageService, ILogger logger) : this(regionManager, storageService)
        {
            if (logger != null)
            {
                this.Logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (storageService != null)
            {
                this.StorageService = storageService;
                this.StorageService.Storage.ApplicationData.GlobalSettings.PropertyChanged += GlobalSettingsPropertyChanged;
            }
            else
            {
                throw new ArgumentNullException(nameof(storageService));
            }

            if (eventAggregator != null)
            {
                this.EventAggregator = eventAggregator;
            }
            else
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }
        }

        protected ViewModelBase(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IStorageService storageService, INotificationManagerService notificationManager) : this(regionManager, storageService)
        {
            if(eventAggregator != null)
            {
                this.EventAggregator = eventAggregator;
            }
            else
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            if (notificationManager != null)
            {
                this.NotificationManager = notificationManager;
            }
            else
            {
                throw new ArgumentNullException(nameof(notificationManager));
            }
        }

        protected ViewModelBase(IRegionManager regionManager)
        {
            if (regionManager != null)
            {
                this.RegionManager = regionManager;
            }
            else
            {
                throw new ArgumentNullException(nameof(regionManager));
            }
        }

        protected ViewModelBase(IRegionManager regionManager, INotificationManagerService notificationManager) : this(regionManager)
        {
            if (notificationManager != null)
            {
                this.NotificationManager = notificationManager;
            }
            else
            {
                throw new ArgumentNullException(nameof(notificationManager));
            }
        }

        #endregion

        #region Properties

        public Storage StoragePlaceholder
        {
            get => _storagePlaceholder;
            set => SetProperty(ref _storagePlaceholder, value);
        }

        /// <summary>
        /// The notification manager that handles displaying notifications on the page.
        /// </summary>
        public INotificationManagerService NotificationManager { get; set; }

        protected IRegionManager RegionManager
        {
            get => _regionManager;
            set => SetProperty(ref _regionManager, value);
        }

        protected IEventAggregator EventAggregator 
        {
            get => _eventAggregator;
            set { SetProperty(ref _eventAggregator, value); } 
        }

        public IStorageService StorageService
        {
            get => _storageService;
            set => SetProperty(ref _storageService, value);
        }

        public Farm ActiveFarm
        {
            get => this.StorageService.GetActiveFarm();
            //set => SetProperty(ref _activeFarm, value);
        }

        /// <summary>
        /// String used to refer to a particular other animals component, value set by child classes. Bound to the view(s), used as a title.
        /// Can be changed by the user, if they happen to leave it empty, an error will be thrown.
        /// </summary>
        public string ViewName
        {
            get => _viewName;
            set 
            {
                if (SetProperty(ref _viewName, value))
                {
                    ValidateViewName();
                }
            }
        }

        public bool AllowNavigation
        {
            get => _allowNavigation;
            set => SetProperty(ref _allowNavigation, value);
        }

        #endregion

        #region Public Methods

        public virtual void InitializeViewModel()
        {
        }

        public virtual void InitializeViewModel(ComponentBase component)
        {
            Logger.LogDebug("initializing " + component);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        /// <summary>Navigation validation checker.</summary>
        /// <remarks>Override for Prism 7.2's IsNavigationTarget.</remarks>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns><see langword="true"/> if this instance accepts the navigation request; otherwise, <see langword="false"/>.</returns>
        public virtual bool OnNavigatingTo(NavigationContext navigationContext)
        {
            return true;
        }

        public void InvokeOnUiThread(Action action)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.UIThread.Invoke(action);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensures that a user cannot leave the <see cref="ViewName"/> empty when editing it in the UI. Uses INotifyDataErrorInfo implementation in <see cref="ViewModelBase"/>.
        /// </summary>
        private void ValidateViewName()
        {
            RemoveError(nameof(ViewName));

            if (string.IsNullOrEmpty(ViewName))
            {
                AddError(nameof(ViewName), H.Core.Properties.Resources.ErrorNameCannotBeEmpty);
                return;
            }
        }

        #endregion

        #region Event Listeners

        private void GlobalSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GlobalSettings.ActiveFarm))
            {
                this.IsInitialized = false;
            }
        }

        #endregion

        #region Protected Methods

        #endregion
    }
}
