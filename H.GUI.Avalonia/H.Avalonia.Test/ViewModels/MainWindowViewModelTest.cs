using H.Avalonia.Services;
using Moq;
using H.Avalonia.ViewModels;

namespace H.Avalonia.Test.ViewModels
{
    [TestClass]
    public class MainWindowViewModelTest
    {
        private MainWindowViewModel _viewModel;
        private Mock<INotificationManagerService> _mockNotificationManager;
        private INotificationManagerService _notificationManagerMock;

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
            _mockNotificationManager = new Mock<INotificationManagerService>();
            _notificationManagerMock = _mockNotificationManager.Object;
        }

        [TestMethod]
        public void TestConstructorNullParameters()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MainWindowViewModel(null));
        }

        [TestMethod]
        public void TestConstructorSetsNotificationService()
        {
            _viewModel = new MainWindowViewModel(_notificationManagerMock);
            Assert.AreSame(_viewModel.NotificationManagerService, _notificationManagerMock);
        }
    }
}
