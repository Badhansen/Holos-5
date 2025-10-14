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

        private WindowNotificationManager _notificationManager;
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

        public WindowNotificationManagerService(ILogger logger)
        {
            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(TopLevel targetWindow)
        {
            if (targetWindow == null)
            {
                throw new ArgumentNullException(nameof(targetWindow));
            }

            if (!_isInitialized)
            {
                _logger.LogInformation("Initializing " + this + " to " + targetWindow);

                _notificationManager = new WindowNotificationManager(targetWindow)
                {
                    Position = NotificationPosition.TopRight,
                    MaxItems = 1,
                    Margin = new(0, 5, 15, 0)
                };
                _isInitialized = true;
                return;
            }

            if (_isInitialized)
            {
                _logger.LogWarning("{Service} attempted reinitialization.", nameof(WindowNotificationManagerService));
            }
        }

        public void ShowToast(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? duration = null)
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("Toast message sent to {Service} before initialization completed.", nameof(WindowNotificationManagerService));
                return;
            }
            _notificationManager?.Show(new Notification(title, message, type, duration ?? TimeSpan.FromSeconds(5)));
        }

        #endregion
    }
}