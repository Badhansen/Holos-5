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
    public class SoilN2OBreakdownSettingsViewModel : ViewModelBase
    {
        #region Fields

        private SoilN2OBreakdownSettingsDTO _data;
        private readonly IErrorHandlerService _errorHandlerService;
        private double _total;
        private bool _entriesAreValid;

        #endregion

        #region Constructors

        public SoilN2OBreakdownSettingsViewModel(IStorageService storageService, IEventAggregator eventAggregator, IErrorHandlerService errorHandlerService) : base(storageService, eventAggregator)
        {
            if (errorHandlerService != null)
            {
                _errorHandlerService = errorHandlerService;
            }
            else
            {
                throw new ArgumentNullException(nameof(errorHandlerService));
            }
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
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (_entriesAreValid)
            {
                Data.PropertyChanged -= ValidateTotalEquals100;
            }
        }

        #endregion

        #region Private Methods

        private void CalculateTotal()
        {
            double previousTotal = Total;
            Total = 0;
            foreach (Months month in Enum.GetValues(typeof(Months)))
            {
                Total += this.Data.MonthlyValues.GetValueByMonth(month);
            }
            if (Total == 100)
            {
                EventAggregator.GetEvent<ValidationPassOccurredEvent>().Publish(new ErrorInformation(string.Format(H.Core.Properties.Resources.SumOfMonthlyN2OInputsPercent, Total)));
                _entriesAreValid = true;
                return;
            }
            // To avoid multiple warnings sent to ErrorHandlerService when user adjusts values above or below 100% threshold
            if (previousTotal == 100)
            {
                string warningString = Total < 100 ? H.Core.Properties.Resources.N2OPercentageLessThan100 : H.Core.Properties.Resources.N2OPercentageGreaterThan100;
                _errorHandlerService.HandleValidationWarning(H.Core.Properties.Resources.NavigationLocked, warningString);
            }
            _entriesAreValid = false;
        }

        #endregion

        #region Event Handlers

        private void ValidateTotalEquals100(object sender, PropertyChangedEventArgs e)
        {
            CalculateTotal();
        }

        #endregion
    }
}
