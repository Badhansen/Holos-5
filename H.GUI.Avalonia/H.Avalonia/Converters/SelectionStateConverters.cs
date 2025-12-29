using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace H.Avalonia.Converters;

/// <summary>
/// Converts a boolean value to a background brush for selection states
/// </summary>
public class BoolToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected)
        {
            return isSelected ? Brush.Parse("#E3F2FD") : Brush.Parse("White");
        }
        
        return Brush.Parse("White");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean value to a border brush for selection states
/// </summary>
public class BoolToBorderBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected)
        {
            return isSelected ? Brush.Parse("DodgerBlue") : Brush.Parse("LightGray");
        }
        
        return Brush.Parse("LightGray");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean value to a border thickness for selection states
/// </summary>
public class BoolToBorderThicknessConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected)
        {
            return isSelected ? new global::Avalonia.Thickness(3) : new global::Avalonia.Thickness(2);
        }
        
        return new global::Avalonia.Thickness(2);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean value to a render transform for the "pressed in" effect
/// </summary>
public class BoolToScaleTransformConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSelected)
        {
            var scaleValue = isSelected ? 0.98 : 1.0;
            return new ScaleTransform(scaleValue, scaleValue);
        }
        
        return new ScaleTransform(1.0, 1.0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a width value by subtracting margin values
/// </summary>
public class WidthMinusMarginConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double width)
        {
            // DefaultUserControlMargin is typically 6,6,6,6, so subtract 12 (left + right)
            return Math.Max(0, width - 12);
        }
        
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}