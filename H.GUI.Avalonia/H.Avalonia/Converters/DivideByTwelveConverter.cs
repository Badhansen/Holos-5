using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace H.Avalonia.Converters;

/// <summary>
/// Converts an annual value to a monthly value by dividing by 12.
/// Useful for displaying flow rates in different time units.
/// </summary>
public class DivideByTwelveConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue / 12.0;
        }
        
        if (value is double doubleValue)
        {
            return doubleValue / 12.0;
        }
        
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("DivideByTwelveConverter only supports one-way conversion.");
    }
}
