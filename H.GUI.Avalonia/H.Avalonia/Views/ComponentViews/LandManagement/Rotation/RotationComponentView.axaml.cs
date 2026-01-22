using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

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

    #region Event Handlers

    /// <summary>
    /// Handles the pointer pressed event on a crop card in the timeline.
    /// This allows users to select a crop card for editing or removal.
    /// </summary>
    private void OnCropCardPressed(object? sender, PointerPressedEventArgs e)
    {
        // The selection logic will be handled by the ViewModel
        // This event handler ensures the UI responds to the click
        if (sender is Border border && border.DataContext != null)
        {
            // The DataContext of the Border is the crop item
            // The ViewModel will handle the selection state change through data binding
        }
    }

    #endregion
}