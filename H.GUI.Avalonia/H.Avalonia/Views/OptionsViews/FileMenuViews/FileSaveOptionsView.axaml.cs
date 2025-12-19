using Avalonia.Controls;
using H.Avalonia.ViewModels.OptionsViews.FileMenuViews;

namespace H.Avalonia.Views.OptionsViews.FileMenuViews;

public partial class FileSaveOptionsView : UserControl
{
    public FileSaveOptionsView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Get the viewmodel associated with the view.
    /// </summary>
    private FileSaveOptionsViewModel? ViewModel => DataContext as FileSaveOptionsViewModel;
}