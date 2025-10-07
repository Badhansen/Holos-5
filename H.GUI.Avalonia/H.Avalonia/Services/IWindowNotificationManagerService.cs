using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using System;

namespace H.Avalonia.Services
{
    /// <summary>
    /// Manages and displays notifications (like toasts or in-window alerts)
    /// within the context of a specific Avalonia Window.
    /// </summary>
    public interface IWindowNotificationManagerService
    {
        bool IsInitialized { get; }
        /// <summary>
        /// Initializes the notification manager with the window it will operate within.
        /// This method should be called once after the window is available from within the code-behind.
        /// </summary>
        /// <param name="targetWindow">The Avalonia Window instance to display notifications on.</param>
        void Initialize(TopLevel targetWindow);

        /// <summary>
        /// Shows a non-blocking, temporary toast notification within the managed window.
        /// </summary>
        /// <param name="title">Optional title for the toast.</param>
        /// <param name="message">The message content of the toast.</param>
        /// <param name="type">The visual type of the notification (e.g., Information, Error).</param>
        /// <param name="duration">Optional duration for how long the toast should be visible. Defaults to 5 seconds.</param>
        void ShowToast(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? duration = null);

    }
}