using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using System;
using System.Collections.Generic;

namespace H.Avalonia.Services
{
    /// <summary>
    /// Manages and displays notifications within an avalonia window it is initialized in.
    /// </summary>
    public interface IWindowNotificationManagerService
    {
        /// <summary>
        /// Check if the notification manager has been initialized with a target window.
        /// </summary>
        bool IsInitialized { get; }
        /// <summary>
        /// Allows getting or setting the maximum number of notifications that can be displayed at once.
        /// </summary>
        int maxDisplayedItems { get; set; }
        /// <summary>
        /// Gets the collection of currently active notifications.
        /// Clears out automatically as notifications expire.
        /// </summary>
        IReadOnlyCollection<Notification> ActiveNotifications { get; }

        /// <summary>
        /// Initializes the notification manager with the window it will operate within.
        /// This method should be called once after the window is available from within the code-behind.
        /// </summary>
        /// <param name="targetWindow">The Avalonia Window instance to display notifications on. Should be the MainWindow so notifications can appear throughout application.</param>
        void Initialize(TopLevel targetWindow);

        /// <summary>
        /// Shows a non-blocking, temporary toast notification within the managed window.
        /// </summary>
        /// <param name="title">Title for the toast.</param>
        /// <param name="message">The message content of the toast.</param>
        /// <param name="type">The visual type of the notification (e.g., Information, Warning, Error). Defaults to an "Information" tier toast.</param>
        void ShowToast(string title, string message, NotificationType type = NotificationType.Information);

    }
}