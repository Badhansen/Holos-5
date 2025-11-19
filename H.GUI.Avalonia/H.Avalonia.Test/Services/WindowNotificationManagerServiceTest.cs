using Avalonia;
using Avalonia.Controls;
using H.Avalonia.Services;
using H.Avalonia.ViewModels.OptionsViews;
using H.Core;
using H.Core.Models;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Platform;
using Avalonia.Headless;
using Avalonia.Markup.Xaml;

namespace H.Avalonia.Test.Services
{
    [TestClass]
    public class NotificationManagerServiceTest
    {
        private NotificationManagerService _service;
        private Mock<ILogger> _mockLogger;
        private ILogger _loggerMock;
        private Window _window;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            var headlessOptions = new AvaloniaHeadlessPlatformOptions();
            AppBuilder.Configure<App>().UseHeadless(headlessOptions).SetupWithoutStarting();
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
            _window = new Window();
        }

        [TestMethod]
        public void TestConstructorInitialization()
        {
            _service = new NotificationManagerService(_loggerMock);
            Assert.IsNotNull(_service);
        }

        [TestMethod]
        public void TestConstructorThrowsExceptionOnNullParameter()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new NotificationManagerService(null));
        }

        [TestMethod]
        public void TestInitializeMethodForNullWindow()
        {
            _service = new NotificationManagerService(_loggerMock);
            Assert.ThrowsException<ArgumentNullException>(() => _service.Initialize(null));
        }

        [TestMethod]
        public void TestInitializationValidWindow()
        {
            _service = new NotificationManagerService(_loggerMock);
            _service.Initialize(_window);
            Assert.IsTrue(_service.IsInitialized);
            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtMostOnce);
        }

        [TestMethod]
        public void TestReinitializationWindow()
        {
            _service = new NotificationManagerService(_loggerMock);
            _service.Initialize(_window);
            _service.Initialize(_window);
            _mockLogger.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeast(2));
        }

        [TestMethod]
        public void TestShowOneToast()
        {
            _service = new NotificationManagerService(_loggerMock);
            _service.Initialize(_window);
            _service.ShowToast("Test Title", "Test Message");
            Assert.AreEqual(1, _service.ActiveNotifications.Count);
        }

        [TestMethod]
        public void TestShowMoreToasts()
        {
            _service = new NotificationManagerService(_loggerMock);
            _service.Initialize(_window);
            _service.ShowToast("Test Title1", "Test Message1");
            _service.ShowToast("Test Title2", "Test Message2");
            _service.ShowToast("Test Title3", "Test Message3");
            _service.ShowToast("Test Title4", "Test Message4");
            _service.ShowToast("Test Title5", "Test Message5");

            Assert.AreEqual(5, _service.ActiveNotifications.Count);
        }

        // Adds unnecessary time to execution of tests, so ignored by default
        [TestMethod]
        [Ignore]
        public void TestMessageTimer()
        {
            _service = new NotificationManagerService(_loggerMock);
            _service.Initialize(_window);
            _service.ShowToast("Test Title1", "Test Message1", NotificationType.Success);
            _service.ShowToast("Test Title2", "Test Message2", NotificationType.Error);

            Thread.Sleep(TimeSpan.FromSeconds(6));

            Assert.AreEqual(1, _service.ActiveNotifications.Count);
        }
    }
}
