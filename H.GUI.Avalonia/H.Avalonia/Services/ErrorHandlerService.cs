using Microsoft.Extensions.Logging;
using Prism.Events;
using System;
using Avalonia.Controls.Notifications;
using H.Core.Events;
using H.Core.Models;

namespace H.Avalonia.Services
{
    public class ErrorHandlerService : IErrorHandlerService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowNotificationManagerService _notificationManager;

        #endregion

        #region Properties

        public WindowNotificationManagerService NotificationManagerService
        {
            get => _notificationManager as WindowNotificationManagerService;
        }

        #endregion

        #region Constructors

        public ErrorHandlerService()
        {

        }

        public ErrorHandlerService(ILogger logger, IEventAggregator eventAggregator, IWindowNotificationManagerService windowNotificationManagerService)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;
            _notificationManager = windowNotificationManagerService;
        }

        #endregion

        #region Public Methods

        public void HandleValidationWarning(string validationTitle, string validationMessage)
        {
            _logger.LogWarning("Validation warning: {ValidationMessage}", validationMessage);

            _eventAggregator.GetEvent<ValidationErrorOccurredEvent>().Publish(new ErrorInformation(validationMessage));

            ShowToastMessage(validationTitle, validationMessage, NotificationType.Warning);
        }

        #endregion

        #region Private Methods

        private void ShowToastMessage(string toastTitle, string toastMessage, NotificationType type)
        {
            NotificationManagerService.ShowToast(toastTitle, toastMessage, type);
        }

        #endregion
    }
}
