using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace H.Avalonia.Services;
public interface IErrorHandlerService
{
    public WindowNotificationManagerService NotificationManagerService { get;  }

    /// <summary>
    /// Handles a validation warning, draws warning (orange) toast message to screen.
    /// </summary>
    /// /// <param name="validationTitle">The validation warning title.</param>
    /// <param name="validationMessage">The validation warning message.</param>
    void HandleValidationWarning(string validationTitle, string validationMessage);
}
