using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using H.Avalonia.ViewModels.OptionsViews.FileMenuViews;

namespace H.Avalonia.Views.OptionsViews.FileMenuViews;

public partial class FarmManagementView : UserControl
{
    public FarmManagementView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Get the viewmodel associated with the view.
    /// </summary>
    private FarmManagementViewModel? ViewModel => DataContext as FarmManagementViewModel;
}