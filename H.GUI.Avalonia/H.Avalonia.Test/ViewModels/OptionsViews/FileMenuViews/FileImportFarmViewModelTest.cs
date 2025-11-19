using H.Avalonia.Services;
using H.Avalonia.ViewModels.OptionsViews.FileMenuViews;
using H.Core.Models;
using H.Core.Services.StorageService;
using Moq;
using Prism.Regions;

namespace H.Avalonia.Test.ViewModels.OptionsViews.FileMenuViews
{
    [TestClass]
    public class FileImportFarmViewModelTest
    {
        #region Fields

        private FileImportFarmViewModel _viewModel = null!;
        private Mock<IRegionManager> _mockRegionManager = null!;
        private Mock<IStorageService> _mockStorageService = null!;
        private Mock<INotificationManagerService> _mockNotificationService = null!;

        #endregion

        #region Initialization

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
            _mockRegionManager = new Mock<IRegionManager>();
            _mockStorageService = new Mock<IStorageService>();
            _mockNotificationService = new Mock<INotificationManagerService>();

            _viewModel = new FileImportFarmViewModel(_mockRegionManager.Object, _mockStorageService.Object, _mockNotificationService.Object);
        }

        #endregion

        #region Basic Constructor Tests

        [TestMethod]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Assert
            Assert.IsNotNull(_viewModel.ImportFarms);
            Assert.IsNotNull(_viewModel.Farms);
            Assert.IsFalse(_viewModel.ShowGrid);
            Assert.IsFalse(_viewModel.IsFarmImported);
            Assert.IsFalse(_viewModel.CanImport);
        }

        #endregion

        #region Basic Property Tests

        [TestMethod]
        public void SelectedFarms_WithFarms_ShouldEnableImport()
        {
            // Arrange
            var farms = new List<Farm> { new Farm { Name = "Test Farm" } };

            // Act
            _viewModel.SelectedFarms = farms;

            // Assert
            Assert.IsTrue(_viewModel.CanImport);
        }

        [TestMethod]
        public void SelectedFarms_EmptyList_ShouldDisableImport()
        {
            // Arrange
            var farms = new List<Farm>();

            // Act
            _viewModel.SelectedFarms = farms;

            // Assert
            Assert.IsFalse(_viewModel.CanImport);
        }

        [TestMethod]
        public void ShowGrid_CanBeSetAndRetrieved()
        {
            // Act
            _viewModel.ShowGrid = true;

            // Assert
            Assert.IsTrue(_viewModel.ShowGrid);
        }

        [TestMethod]
        public void IsFarmImported_CanBeSetAndRetrieved()
        {
            // Act
            _viewModel.IsFarmImported = true;

            // Assert
            Assert.IsTrue(_viewModel.IsFarmImported);
        }

        #endregion

        #region Basic Command Tests

        [TestMethod]
        public void ImportFarms_WithSelectedFarms_ShouldCallStorageService()
        {
            // Arrange
            var testFarm = new Farm { Name = "Test Farm" };
            _viewModel.SelectedFarms = new List<Farm> { testFarm };

            // Act
            _viewModel.ImportFarms.Execute();

            // Assert
            _mockStorageService.Verify(x => x.AddFarm(testFarm), Times.Once);
            Assert.IsTrue(_viewModel.IsFarmImported);
        }

        [TestMethod]
        public void ImportFarms_WithException_ShouldHandleGracefully()
        {
            // Arrange
            var testFarm = new Farm { Name = "Test Farm" };
            _viewModel.SelectedFarms = new List<Farm> { testFarm };
            _mockStorageService.Setup(x => x.AddFarm(It.IsAny<Farm>())).Throws(new Exception("Test error"));

            // Act & Assert - Should not throw
            _viewModel.ImportFarms.Execute();
            Assert.IsFalse(_viewModel.IsFarmImported);
        }

        #endregion

        #region Basic Async Method Tests

        [TestMethod]
        public async Task GetFarmsFromExportFileAsync_WithEmptyPath_ShouldReturnEmpty()
        {
            // Act
            var result = await _viewModel.GetFarmsFromExportFileAsync("");

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetFarmsFromExportFileAsync_WithNonExistentFile_ShouldReturnEmpty()
        {
            // Act
            var result = await _viewModel.GetFarmsFromExportFileAsync("nonexistent.json");

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetFarmsFromExportFileAsync_WithValidFile_ShouldReturnFarms()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var testFarms = new List<Farm> { new Farm { Name = "Test Farm" } };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(testFarms, 
                new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto });
            await File.WriteAllTextAsync(tempFile, json);

            try
            {
                // Act
                var result = await _viewModel.GetFarmsFromExportFileAsync(tempFile);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual("Test Farm", result.First().Name);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public async Task GetExportedFarmsFromDirectoryRecursivelyAsync_WithEmptyDirectory_ShouldReturnEmpty()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                var result = await _viewModel.GetExportedFarmsFromDirectoryRecursivelyAsync(tempDir);

                // Assert
                Assert.AreEqual(0, result.Count());
            }
            finally
            {
                Directory.Delete(tempDir);
            }
        }

        [TestMethod]
        public async Task GetExportedFarmsFromDirectoryRecursivelyAsync_WithJsonFiles_ShouldReturnFarms()
        {
            // Arrange
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var farm = new Farm { Name = "Directory Test Farm" };
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new List<Farm> { farm }, 
                new Newtonsoft.Json.JsonSerializerSettings { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto });
            var filePath = Path.Combine(tempDir, "test.json");
            await File.WriteAllTextAsync(filePath, json);

            try
            {
                // Act
                var result = await _viewModel.GetExportedFarmsFromDirectoryRecursivelyAsync(tempDir);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.AreEqual("Directory Test Farm", result.First().Name);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        #endregion
    }
}
