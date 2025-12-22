using Avalonia;
using Avalonia.Controls;

namespace H.Avalonia.Views.ComponentViews.LandManagement.Field;

public partial class FieldComponentView : UserControl
{
    public static readonly StyledProperty<bool> ShowAdvancedOptionsProperty =
        AvaloniaProperty.Register<FieldComponentView, bool>(nameof(ShowAdvancedOptions), defaultValue: false);

    public bool ShowAdvancedOptions
    {
        get => GetValue(ShowAdvancedOptionsProperty);
        set => SetValue(ShowAdvancedOptionsProperty, value);
    }

    public FieldComponentView()
    {
        InitializeComponent();
        
        // Set default value for design time
        if (Design.IsDesignMode)
        {
            ShowAdvancedOptions = true;
        }
    }
}