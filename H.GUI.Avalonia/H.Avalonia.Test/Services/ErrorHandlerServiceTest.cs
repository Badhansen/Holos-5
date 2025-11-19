using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using H.Avalonia.Services;
using H.Core.Events;
using Microsoft.Extensions.Logging;
using Prism.Events;

namespace H.Avalonia.Test.Services
{
    [TestClass]
    public class ErrorHandlerServiceTest
    {
        private ErrorHandlerService _service;
        private Mock<ILogger> _mockLogger;
        private ILogger _loggerMock;
        private Mock<IEventAggregator> _mockEventAggregator;
        private IEventAggregator _eventAggregatorMock;
        private Mock<INotificationManagerService> _mockNotificationManager;
        private INotificationManagerService _notificationManagerMock;
        private Mock<PubSubEvent<ValidationErrorOccurredEvent>> _mockEvent;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _mockLogger = new Mock<ILogger>();
            _loggerMock = _mockLogger.Object;
            _mockEventAggregator = new Mock<IEventAggregator>();
            _eventAggregatorMock = _mockEventAggregator.Object;
            _mockNotificationManager = new Mock<INotificationManagerService>();
            _notificationManagerMock = _mockNotificationManager.Object;

            var mockEvent = new Mock<PubSubEvent<ValidationErrorOccurredEvent>>();
            _mockEventAggregator.Setup(x => x.GetEvent<ValidationErrorOccurredEvent>()).Returns(new ValidationErrorOccurredEvent());
        }

        [TestMethod]
        public void TestConstructorNullLogger()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ErrorHandlerService(null, _eventAggregatorMock, _notificationManagerMock));
        }
        [TestMethod]
        public void TestConstructorNullEventAggregator()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ErrorHandlerService(_loggerMock, null, _notificationManagerMock));
        }
        [TestMethod]
        public void TestConstructorNullNotificationManager()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ErrorHandlerService(_loggerMock, _eventAggregatorMock, null));
        }
        [TestMethod]
        public void TestConstructorInitialization()
        {
            _service = new ErrorHandlerService(_loggerMock, _eventAggregatorMock, _notificationManagerMock);
            Assert.IsNotNull(_service);
        }

        [TestMethod]
        public void TestHandleValidationWarning()
        {
            _service = new ErrorHandlerService(_loggerMock, _eventAggregatorMock, _notificationManagerMock);
            _service.HandleValidationWarning("Test Title", "Test Message");

            _mockLogger.Verify(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.AtMostOnce);

            _mockNotificationManager.Verify(x => x.ShowToast("Test Title", "Test Message", NotificationType.Information), Times.AtMostOnce);
        }

        [TestMethod]
        public void TestHandleNonInterruptingError()
        {
            _service = new ErrorHandlerService(_loggerMock, _eventAggregatorMock, _notificationManagerMock);
            _service.HandleNonInterruptingError("Error Title", "Error Message");
            _mockLogger.Verify(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.AtMostOnce);
            _mockNotificationManager.Verify(x => x.ShowToast("Error Title", "Error Message", NotificationType.Error), Times.AtMostOnce);
        }
    }
}
