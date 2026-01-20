using Avalonia;
using Avalonia.Controls;

namespace H.Avalonia.Views.ComponentViews.LandManagement.Rotation;

public partial class RotationComponentView : UserControl
{
    #region Properties

    public static readonly StyledProperty<bool> ShowAdvancedOptionsProperty =
        AvaloniaProperty.Register<RotationComponentView, bool>(nameof(ShowAdvancedOptions), defaultValue: false);

    public bool ShowAdvancedOptions
    {
        get => GetValue(ShowAdvancedOptionsProperty);
        set => SetValue(ShowAdvancedOptionsProperty, value);
    }

    #endregion

    #region Constructors

    public RotationComponentView()
    {
        InitializeComponent();
        
        // Set default value for design time
        if (Design.IsDesignMode)
        {
            ShowAdvancedOptions = true;
        }
    } 

    #endregion
}