using System;
using System.ComponentModel;
using System.Reflection.Metadata;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using H.Core.Events;
using H.Avalonia.ViewModels.OptionsViews.DataTransferObjects;
using H.Core.Enumerations;
using H.Core.Models;
using H.Core.Providers.Animals;
using H.Avalonia.Services;
using H.Core.Services.StorageService;
using H.Infrastructure;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.ViewModels.OptionsViews
{
    public class SoilN2OBreakdownSettingsViewModel : ViewModelBase//, IConfirmNavigationRequest
    {
        #region Fields

        private SoilN2OBreakdownSettingsDTO _data;
        private readonly IErrorHandlerService _errorHandlerService;
        private double _total;
        private bool _entriesAreValid;

        #endregion

        #region Constructors

        public SoilN2OBreakdownSettingsViewModel(IStorageService storageService, IEventAggregator eventAggregator, IErrorHandlerService errorHandlerService) : base(storageService)
        {
            EventAggregator = eventAggregator;
            _errorHandlerService = errorHandlerService as ErrorHandlerService;
        }
        #endregion

        #region Properties
        public SoilN2OBreakdownSettingsDTO Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        public double Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        public new WindowNotificationManager NotificationManager
        {
            get => base.NotificationManager;
            set
            {
                if (base.NotificationManager == value) return;
                base.NotificationManager = value;
                _errorHandlerService.NotificationManager = value;
            }
        }
        #endregion

        #region Public Methods

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!IsInitialized)
            {
                Data = new SoilN2OBreakdownSettingsDTO(StorageService);
                IsInitialized = true;
            }
            CalculateTotal();
            Data.PropertyChanged += ValidateTotalEquals100;
            
            EventAggregator.GetEvent<ValidationErrorOccurredEvent>().Subscribe(LockOnInvalidEntries);
            EventAggregator.GetEvent<ValidationPassOccurredEvent>().Subscribe(UnlockOnValidEntries);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (_entriesAreValid)
            {
                Data.PropertyChanged -= ValidateTotalEquals100;
                EventAggregator.GetEvent<ValidationErrorOccurredEvent>().Unsubscribe(LockOnInvalidEntries);
                EventAggregator.GetEvent<ValidationPassOccurredEvent>().Unsubscribe(UnlockOnValidEntries);
                return;
            }
            _errorHandlerService.HandleValidationError(string.Format(H.Core.Properties.Resources.SumOfMonthlyN2OInputsPercent, Total));
        }

        #endregion

        #region Private Methods

        private void CalculateTotal()
        {
            Total = 0;
            foreach (Months month in Enum.GetValues(typeof(Months)))
            {
                Total += this.Data.MonthlyValues.GetValueByMonth(month);
            }
            if (Total == 100)
            {

                EventAggregator.GetEvent<ValidationPassOccurredEvent>().Publish(new ErrorInformation(string.Format(H.Core.Properties.Resources.SumOfMonthlyN2OInputsPercent, Total)));
                return;
            }
            EventAggregator.GetEvent<ValidationErrorOccurredEvent>().Publish(new ErrorInformation(string.Format(H.Core.Properties.Resources.SumOfMonthlyN2OInputsPercent, Total)));
        }

        #endregion

        #region Event Handlers

        private void ValidateTotalEquals100(object sender, PropertyChangedEventArgs e)
        {
            CalculateTotal();
        }

        private void LockOnInvalidEntries(MessageBase errorInformation)
        {
            _entriesAreValid = false;
        }

        private void UnlockOnValidEntries(MessageBase errorInformation)
        {
            _entriesAreValid = true;
        }

        #endregion
    }
}
