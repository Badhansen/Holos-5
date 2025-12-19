using System.ComponentModel;
using H.Core.Enumerations;

namespace H.Core.Calculators.UnitsOfMeasurement
{
    public interface IUnitsOfMeasurementCalculator : INotifyPropertyChanged
    {
        string KilogramsPerHectareString { get; set; }

        bool IsMetric { get; set; }

        string GetUnitsOfMeasurementString(MeasurementSystemType measurementSystemType,
                                           MetricUnitsOfMeasurement unitsOfMeasurement);

        /// <summary>
        /// Based on the measurement type, returns the proper value (metric or imperial) rounded to 4 decimal points
        /// Takes in Metric Units
        /// First argument corresponds to the user inputed measurement system (Metric or Imperial).
        /// The second argument is set to MetricUnitsOfMeasurement, you can change it to Imperial if needed, but there is no need
        /// 
        /// </summary>
        double GetUnitsOfMeasurementValue(MeasurementSystemType measurementSystemType,
            MetricUnitsOfMeasurement unitsOfMeasurement, double value, bool exportedFromFarm);

        /// <summary>
        /// Converts input from Metric to Imperial units
        /// </summary>
        double GetUnitsOfMeasurementValue(MeasurementSystemType measurementSystemType, MetricUnitsOfMeasurement metricUnits, double value);
        
        /// <summary>
        /// Converts input from Imperial to Metric units
        /// </summary>
        double GetUnitsOfMeasurementValue(MeasurementSystemType measurementSystemType, ImperialUnitsOfMeasurement imperialUnits, double value);

        MeasurementSystemType GetUnitsOfMeasurement();
    }
}