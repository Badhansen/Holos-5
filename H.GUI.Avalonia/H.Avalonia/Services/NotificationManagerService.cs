using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Prism.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace H.Avalonia.Services
{
    public class NotificationManagerService : INotificationManagerService
    {
        #region Fields

        private WindowNotificationManager _notificationManager;
        private readonly ILogger _logger;
        private bool _isInitialized = false;
        private readonly ConcurrentBag<Notification> _activeNotifications = new();
        private TimeSpan _successTimeSpan = TimeSpan.FromSeconds(5);
        private TimeSpan _informationTimeSpan = TimeSpan.FromSeconds(5);
        private TimeSpan _warningTimeSpan = TimeSpan.FromSeconds(10);
        private TimeSpan _errorTimeSpan = TimeSpan.FromSeconds(10);

        #endregion

        #region Properties

        public bool IsInitialized
        {
            get => _isInitialized;
        }

        public int maxDisplayedItems
        {
            get => _notificationManager?.MaxItems ?? 0;
            set
            {
                if (value >= 0)
                {
                    _notificationManager.MaxItems = value;
                }
            }
        }

        public IReadOnlyCollection<Notification> ActiveNotifications => _activeNotifications;

        #endregion

        #region Constructors

        public NotificationManagerService(ILogger logger)
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
                    Margin = new(0, 5, 15, 0),
                };
                _isInitialized = true;
            }
            else
            {
                _logger.LogWarning("{Service} attempted reinitialization.", nameof(NotificationManagerService));
            }
        }

        public void ShowToast(string title, string message, NotificationType type = NotificationType.Information)
        {
            if (!_isInitialized)
            {
                _logger.LogWarning("Toast message sent to {Service} before initialization completed.", nameof(NotificationManagerService));
                return;
            }

            // Determine duration based on notification type if not explicitly provided
            TimeSpan duration = _informationTimeSpan;
            switch (type)
            {
                case NotificationType.Success:
                    duration = _successTimeSpan;
                    break;
                case NotificationType.Warning:
                    duration = _warningTimeSpan;
                    break;
                case NotificationType.Error: 
                    duration = _errorTimeSpan;
                    break;
            }

            var notification = new Notification(title, message, type, duration);
            _notificationManager?.Show(notification);
            _activeNotifications.Add(notification);

            // Remove notification from collection once timer expires
            Task.Delay(notification.Expiration).ContinueWith(x =>
            {
                _activeNotifications.TryTake(out Notification discard);
            });
        }

        #endregion
    }
}