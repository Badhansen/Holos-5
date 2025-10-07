using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Castle.Core.Logging;
using Prism.Events;
using System;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace H.Avalonia.Services
{
    public class WindowNotificationManagerService : IWindowNotificationManagerService
    {
        #region Fields

        private WindowNotificationManager? _notificationManager;
        private readonly ILogger _logger;
        private bool _isInitialized = false;

        #endregion

        #region Properties

        public bool IsInitialized
        {
            get => _isInitialized;
        }

        #endregion

        #region Constructors

        public WindowNotificationManagerService()
        {
        }

        public WindowNotificationManagerService(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Public Methods

        public void Initialize(TopLevel targetWindow)
        {
            _logger.LogInformation("Initializing " + this + " to " + targetWindow);
            if (targetWindow == null)
            {
                throw new ArgumentNullException(nameof(targetWindow));
            }

            _notificationManager = new WindowNotificationManager(targetWindow)
            {
                Position = NotificationPosition.TopRight,
                MaxItems = 1,
                Margin = new(0, 5, 15, 0)
            };
            _isInitialized = true;
        }

        public void ShowToast(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? duration = null)
        {
            _notificationManager?.Show(new Notification(title, message, type, duration ?? TimeSpan.FromSeconds(5)));
        }

        #endregion
    }
}