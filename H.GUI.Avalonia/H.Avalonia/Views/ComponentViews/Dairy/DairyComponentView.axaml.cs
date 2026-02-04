using Avalonia;
using Avalonia.Controls;

namespace H.Avalonia.Views.ComponentViews.Dairy;

public partial class DairyComponentView : UserControl
{
    #region Properties

    /// <summary>
    /// Avalonia styled property for controlling the visibility of advanced configuration options.
    /// This property is registered with Avalonia's property system to enable data binding
    /// and is bound to the AdvancedOptionsToggleView control in the XAML.
    /// </summary>
    public static readonly StyledProperty<bool> ShowAdvancedOptionsProperty =
        AvaloniaProperty.Register<DairyComponentView, bool>(nameof(ShowAdvancedOptions), defaultValue: false);

    /// <summary>
    /// Gets or sets whether advanced options should be displayed in the dairy operation configuration UI.
    /// This controls the visibility of additional settings that are useful for power users
    /// but may overwhelm new users.
    /// </summary>
    public bool ShowAdvancedOptions
    {
        get => GetValue(ShowAdvancedOptionsProperty);
        set => SetValue(ShowAdvancedOptionsProperty, value);
    }

    #endregion

    #region Constructors

    public DairyComponentView()
    {
        InitializeComponent();
        
        // Set default value for design time - makes it easier to see all options in the designer
        if (Design.IsDesignMode)
        {
            ShowAdvancedOptions = true;
        }
    }

    #endregion
}