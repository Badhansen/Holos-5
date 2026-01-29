using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using H.Avalonia.ViewModels.ComponentViews.LandManagement.Rotation;

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

        // Subscribe to DataContext changes
        this.DataContextChanged += OnDataContextChanged;
    }

    #endregion

    #region Private Methods

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        // Unsubscribe from old ViewModel if it exists
        if (sender is RotationComponentView view && view.DataContext is RotationComponentViewModel oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        // Subscribe to new ViewModel
        if (this.DataContext is RotationComponentViewModel newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // When SelectedCropDto changes, check if we should scroll
        if (e.PropertyName == nameof(RotationComponentViewModel.SelectedCropDto))
        {
            var viewModel = sender as RotationComponentViewModel;
            
            // Only scroll if the selection came from a grid cell (ShouldTriggerAutoScroll is true)
            if (viewModel?.SelectedCropDto != null && viewModel.ShouldTriggerAutoScroll)
            {
                // Use dispatcher to ensure layout has updated before scrolling
                Dispatcher.UIThread.Post(() =>
                {
                    ScrollToBottom();
                    
                    // Reset the flag after scrolling
                    if (viewModel != null)
                    {
                        viewModel.ShouldTriggerAutoScroll = false;
                    }
                }, DispatcherPriority.Background);
            }
        }
    }

    private void ScrollToBottom()
    {
        // Find the ScrollViewer in the visual tree
        var scrollViewer = this.FindControl<ScrollViewer>("MainScrollViewer");
        if (scrollViewer != null)
        {
            // Scroll to the end with animation
            scrollViewer.ScrollToEnd();
        }
    }

    #endregion
}