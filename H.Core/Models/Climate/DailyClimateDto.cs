using H.Core.Factories;

namespace H.Core.Models.Climate
{
    /// <summary>
    /// Contains properties that are tied to the Grid shown for the climate results page.
    /// </summary>
    public class DailyClimateDto : DtoBase
    {
        #region Fields

        private int _year;
        private double _totalPET;
        private double _totalPPT;
        private double _monthlyPPT;
        private double _longitude;
        private double _latitude; 

        #endregion

        #region Properties

        /// <summary>
        /// The year for which data is extracted.
        /// </summary>
        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        /// <summary>
        /// The latitude value specified by the user.
        /// </summary>
        public double Latitude
        {
            get => _latitude;
            set
            {
                if (value is < -90 or > 90) value = 0;
                SetProperty(ref _latitude, value);
            }
        }

        /// <summary>
        /// The longitude value specified by the user.
        /// </summary>
        public double Longitude
        {
            get => _longitude;
            set
            {
                if (value is < -180 or > 180) value = 0;
                SetProperty(ref _longitude, value);
            }
        }

        /// <summary>
        /// The evapotranspiration amount for the given year.
        /// </summary>
        public double TotalPET
        {
            get => _totalPET;
            set => SetProperty(ref _totalPET, value);
        }

        /// <summary>
        /// The total precipitation amount for the given year.
        /// </summary>
        public double TotalPPT
        {
            get => _totalPPT;
            set => SetProperty(ref _totalPPT, value);
        }

        /// <summary>
        /// A monthly precipitation amount for a range of months specified by the user.
        /// </summary>
        public double MonthlyPPT
        {
            get => _monthlyPPT;
            set => SetProperty(ref _monthlyPPT, value);
        } 

        #endregion
    }
}
