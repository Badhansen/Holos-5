using System;
using System.Collections.ObjectModel;
using H.Avalonia.ViewModels.Styles;
using H.Core;
using H.Core.Enumerations;
using H.Core.Services.StorageService;
using ImTools;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace H.Avalonia.ViewModels.OptionsViews
{
    public abstract class ChartBaseViewModel<T>: ViewModelBase where T : MonthlyValueBase<double>
    {
        #region Fields 

        private ISeries[] _series;
        private Axis[] _xAxes;
        private BarChartStyles _barChartsStyles = new BarChartStyles();

        #endregion

        #region Constructors

        public ChartBaseViewModel(IStorageService storageService) : base(storageService) 
        {
            _series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Fill = _barChartsStyles.SetColumnSeriesFill(),
                    Values = new ObservableCollection<double>()
                }
            };

            _xAxes = new Axis[]
            {
                _barChartsStyles.SetAxisStyling(name: H.Core.Properties.Resources.Months, labels: Enum.GetNames(typeof(Months)))
            };
        }

        #endregion

        #region Properties

        public ISeries[] Series
        {
            get => _series;
            set => SetProperty(ref _series, value);
        }

        public Axis[] XAxes
        {
            get => _xAxes;
            set => SetProperty(ref _xAxes, value);
        }

        /// <summary>
        /// This property is used in <see cref="BuildChart()"/> it provides the underlying values for the Series.
        /// It is abstract so derived classes have to decide how to expose the correct data of type <see cref="MonthlyValueBase{Double}"/>.
        /// This helps with compatibility if a derived class's data comes from a DTO, instead of directly from a <see cref="MonthlyValueBase{Double}"/> subclass.
        /// </summary>
        protected abstract T ChartValuesSource { get; }

        #endregion

        #region Protected Methods

        protected abstract void InitializeData();

        protected void BuildChart()
        {
            var values = new ObservableCollection<double>();
            foreach (Months month in Enum.GetValues(typeof(Months)))
            {
                values.Add(Math.Round(this.ChartValuesSource.GetValueByMonth(month), 2));
            }

            if (base.IsInitialized && !this.Series.IsNullOrEmpty() && this.Series[0] is ColumnSeries<double> columnSeries)
            {
                columnSeries.Values = values;
            }

            else
            {
                this.Series = new ISeries[]
                {
                        new ColumnSeries<double>
                        {
                            Fill = _barChartsStyles.SetColumnSeriesFill(),
                            Values = values,
                        }
                };
            }
        }

        #endregion
    }
}
