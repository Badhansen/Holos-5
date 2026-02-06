using Avalonia;
using Avalonia.Controls;

namespace H.Avalonia.Views.ComponentViews.Shared;

/// <summary>
/// A reusable user control for displaying an advanced options toggle switch
/// with customizable title and description text.
/// </summary>
public partial class AdvancedOptionsToggleView : UserControl
{
    /// <summary>
    /// Styled property for the title text displayed in the toggle control.
    /// </summary>
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<AdvancedOptionsToggleView, string>(
            nameof(Title), 
            defaultValue: "Show Advanced Options");

    /// <summary>
    /// Styled property for the description text displayed below the title.
    /// </summary>
    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<AdvancedOptionsToggleView, string>(
            nameof(Description), 
            defaultValue: "Display additional configuration options");

    /// <summary>
    /// Styled property for the checked state of the toggle switch.
    /// </summary>
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<AdvancedOptionsToggleView, bool>(
            nameof(IsChecked), 
            defaultValue: false);

    /// <summary>
    /// Gets or sets the title text displayed in the toggle control.
    /// </summary>
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the description text displayed below the title.
    /// </summary>
    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the toggle switch is checked.
    /// </summary>
    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public AdvancedOptionsToggleView()
    {
        InitializeComponent();
        
        // Set default checked state for design time
        if (Design.IsDesignMode)
        {
            IsChecked = true;
        }
    }
}
