using H.Core.CustomAttributes;
using H.Core.Enumerations;
using H.Infrastructure;
using System;

namespace H.Core.Providers.Climate
{
    public class DailyClimateData : ModelBase
    {
        #region Properties

        public int Year { get; set; }

        public int JulianDay { get; set; }

        /// <summary>
        /// (degrees C)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.DegreesCelsius)]
        public double MeanDailyAirTemperature { get; set; }

        /// <summary>
        /// (mm)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Millimeters)]
        public double MeanDailyPrecipitation { get; set; }

        /// <summary>
        /// (mm)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.Millimeters)]
        public double MeanDailyPET { get; set; }

        [Units(MetricUnitsOfMeasurement.Percentage)]
        public double RelativeHumidity { get; set; }

        /// <summary>
        /// (MJ m^-2 day^-1)
        /// </summary>
        [Units(MetricUnitsOfMeasurement.MegaJoulesPerSquareMeterPerDay)]
        public double SolarRadiation { get; set; }

        public DateTime Date { get;  set; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{nameof(Year)}: {Year}, {nameof(JulianDay)}: {JulianDay}, {nameof(MeanDailyAirTemperature)}: {MeanDailyAirTemperature}, {nameof(MeanDailyPrecipitation)}: {MeanDailyPrecipitation}, {nameof(MeanDailyPET)}: {MeanDailyPET}";
        }

        public string ToCustomFileFormatString()
        {
            return $"{Year},{JulianDay},{MeanDailyAirTemperature},{MeanDailyPrecipitation},{MeanDailyPET}";
        }

        #endregion
    }
}