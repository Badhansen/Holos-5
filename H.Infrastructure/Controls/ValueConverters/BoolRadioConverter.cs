#region Imports

using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

#endregion

namespace H.Infrastructure.Controls.ValueConverters
{
    /// <summary>
    /// </summary>
    public class BoolRadioConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool == false)
            {
                return false;
            }

            var boolValue = (bool) value;

            return this.Inverse ? !boolValue : boolValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
            {
                return BindingOperations.DoNothing;
            }

            if (!boolValue)
            {
                return BindingOperations.DoNothing;
            }

            return !this.Inverse;
        }
    }
}