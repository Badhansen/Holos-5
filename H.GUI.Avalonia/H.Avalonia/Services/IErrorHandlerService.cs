using System;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace H.Avalonia.Services;
public interface IErrorHandlerService
{
    /// <summary>
    /// Handles a validation warning by logging warning, publishing event, and draw warning (orange) toast message to screen.
    /// </summary>
    /// /// <param name="validationTitle">The validation warning title.</param>
    /// <param name="validationMessage">The validation warning message.</param>
    void HandleValidationWarning(string validationTitle, string validationMessage);
    /// <summary>
    /// Handles a non interrupting error by logging the error, and draw warning (red) toast message to screen.
    /// </summary>
    /// /// <param name="validationTitle">The validation warning title.</param>
    /// <param name="validationMessage">The validation warning message.</param>
    void HandleNonInterruptingError(string errorTitle, string errorMessage);
}
