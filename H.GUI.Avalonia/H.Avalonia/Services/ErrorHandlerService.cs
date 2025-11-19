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
        private readonly INotificationManagerService _notificationManager;

        #endregion


        #region Constructors

        public ErrorHandlerService(ILogger logger, IEventAggregator eventAggregator, INotificationManagerService notificationManagerService)
        {
            if (logger != null)
            {
                _logger = logger;
            }
            else
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (eventAggregator != null)
            {
                _eventAggregator = eventAggregator;
            }
            else
            {
                throw new ArgumentNullException(nameof(eventAggregator));
            }

            if (notificationManagerService != null)
            {
                _notificationManager = notificationManagerService;
            }
            else
            {
                throw new ArgumentNullException(nameof(notificationManagerService));
            }
        }

        #endregion

        #region Public Methods

        public void HandleValidationWarning(string validationTitle, string validationMessage)
        {
            _logger.LogWarning("Validation warning: {ValidationMessage}", validationMessage);

            _eventAggregator.GetEvent<ValidationErrorOccurredEvent>().Publish(new ErrorInformation(validationMessage));

            ShowToastMessage(validationTitle, validationMessage, NotificationType.Warning);
        }

        public void HandleNonInterruptingError(string errorTitle, string errorMessage)
        {
            _logger.LogError("Error: {ErrorMessage}", errorMessage);

            ShowToastMessage(errorTitle, errorMessage, NotificationType.Error);
        }

        #endregion

        #region Private Methods

        private void ShowToastMessage(string toastTitle, string toastMessage, NotificationType type)
        {
            _notificationManager.ShowToast(toastTitle, toastMessage, type);
        }

        #endregion
    }
}
