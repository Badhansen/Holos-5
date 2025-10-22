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
    public class WindowNotificationManagerService : IWindowNotificationManagerService
    {
        #region Fields

        private WindowNotificationManager _notificationManager;
        private readonly ILogger _logger;
        private bool _isInitialized = false;
        private readonly ConcurrentBag<Notification> _activeNotifications = new();

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
                    Margin = new(0, 5, 15, 0),
                };
                _isInitialized = true;
            }
            else
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

            var notification = new Notification(title, message, type, duration ?? TimeSpan.FromSeconds(5));
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