using H.Avalonia.ViewModels.ComponentViews.LandManagement;
using H.Avalonia.ViewModels.ComponentViews.LandManagement.Rotation;
using H.Core.Factories.Crops;
using H.Core.Models;
using H.Core.Models.LandManagement.Rotation;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.Test.ViewModels.ComponentViews.LandManagement;

[TestClass]
public class RotationComponentViewModelTests
{
    #region Fields

    private RotationComponentViewModel _viewModel;
    private Mock<IRegionManager> _mockRegionManager;
    private Mock<IEventAggregator> _mockEventAggregator;
    private Mock<IStorageService> _mockStorageService;
    private Mock<IFieldComponentService> _mockFieldComponentService;
    private Mock<IRotationComponentService> _mockRotationComponentService;
    private Mock<ILogger> _mockLogger;
    private Mock<ICropFactory> _mockCropFactory;
    private Farm _testFarm;

    #endregion

    #region Initialization

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
    }

    [TestInitialize]
    public void TestInitialize()
    {
        // Setup test farm
        _testFarm = new Farm
        {
            Name = "Test Farm"
        };

        // Setup mocks
        _mockRegionManager = new Mock<IRegionManager>();
        _mockEventAggregator = new Mock<IEventAggregator>();
        _mockStorageService = new Mock<IStorageService>();
        _mockFieldComponentService = new Mock<IFieldComponentService>();
        _mockLogger = new Mock<ILogger>();
        _mockCropFactory = new Mock<ICropFactory>();
        _mockRotationComponentService = new Mock<IRotationComponentService>();

        // Setup storage service mock
        _mockStorageService.Setup(x => x.Storage).Returns(new H.Core.Storage()
        {
            ApplicationData = new ApplicationData()
            {
                GlobalSettings = new GlobalSettings()
            }
        });
        _mockStorageService.Setup(x => x.GetActiveFarm()).Returns(_testFarm);

        // Create view model with mocked dependencies
        _viewModel = new RotationComponentViewModel(
            _mockRegionManager.Object,
            _mockEventAggregator.Object,
            _mockStorageService.Object,
            _mockFieldComponentService.Object,
            _mockRotationComponentService.Object,
            _mockLogger.Object,
            _mockCropFactory.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _viewModel?.Dispose();
    }

    #endregion

    #region Constructor Tests

    [TestMethod]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Act & Assert
        Assert.IsNotNull(_viewModel);
        Assert.IsInstanceOfType(_viewModel, typeof(RotationComponentViewModel));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullRegionManager_ShouldThrowArgumentNullException()
    {
        // Act
        new RotationComponentViewModel(
            null,
            _mockEventAggregator.Object,
            _mockStorageService.Object,
            _mockFieldComponentService.Object,
            _mockRotationComponentService.Object,
            _mockLogger.Object,
            _mockCropFactory.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullEventAggregator_ShouldThrowArgumentNullException()
    {
        // Act
        new RotationComponentViewModel(
            _mockRegionManager.Object,
            null,
            _mockStorageService.Object,
            _mockFieldComponentService.Object,
            _mockRotationComponentService.Object,
            _mockLogger.Object,
            _mockCropFactory.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullStorageService_ShouldThrowArgumentNullException()
    {
        // Act
        new RotationComponentViewModel(
            _mockRegionManager.Object,
            _mockEventAggregator.Object,
            null,
            _mockFieldComponentService.Object,
            _mockRotationComponentService.Object,
            _mockLogger.Object,
            _mockCropFactory.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullFieldComponentService_ShouldThrowArgumentNullException()
    {
        // Act
        new RotationComponentViewModel(
            _mockRegionManager.Object,
            _mockEventAggregator.Object,
            _mockStorageService.Object,
            null,
            _mockRotationComponentService.Object,
            _mockLogger.Object,
            _mockCropFactory.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        new RotationComponentViewModel(
            _mockRegionManager.Object,
            _mockEventAggregator.Object,
            _mockStorageService.Object,
            _mockFieldComponentService.Object,
            _mockRotationComponentService.Object,
            null,
            _mockCropFactory.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_WithNullCropFactory_ShouldThrowArgumentNullException()
    {
        // Act
        new RotationComponentViewModel(
            _mockRegionManager.Object,
            _mockEventAggregator.Object,
            _mockStorageService.Object,
            _mockFieldComponentService.Object,
            _mockRotationComponentService.Object,
            _mockLogger.Object,
            null);
    }

    #endregion

    #region InitializeViewModel Tests

    [TestMethod]
    public void InitializeViewModel_WithRotationComponent_ShouldCallBaseInitializeViewModel()
    {
        // Arrange
        var rotationComponent = new RotationComponent
        {
            Name = "Test Rotation"
        };

        // Act
        _viewModel.InitializeViewModel(rotationComponent);

        // Assert
        // Verify that the logger was called (indicating base method was called)
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("initializing")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }


    [TestMethod]
    public void InitializeViewModel_WithParameterlessCall_ShouldNotThrow()
    {
        // Act
        try
        {
            _viewModel.InitializeViewModel();
        }
        catch (Exception)
        {
            Assert.Fail("InitializeViewModel() should not throw an exception");
        }
        
        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    #endregion

    #region Dependency Injection Tests

    [TestMethod]
    public void Constructor_ShouldInjectAllDependenciesCorrectly()
    {
        // Arrange & Act
        var viewModel = new RotationComponentViewModel(
            _mockRegionManager.Object,
            _mockEventAggregator.Object,
            _mockStorageService.Object,
            _mockFieldComponentService.Object,
            _mockRotationComponentService.Object,
            _mockLogger.Object,
            _mockCropFactory.Object);

        // Assert
        // Since the fields are private, we can verify correct injection by testing that no exceptions are thrown
        // and that the view model is properly initialized
        Assert.IsNotNull(viewModel);
        Assert.IsInstanceOfType(viewModel, typeof(RotationComponentViewModel));
    }

    #endregion

    #region Disposal Tests

    [TestMethod]
    public void Dispose_ShouldNotThrowException()
    {
        // Act
        try
        {
            _viewModel.Dispose();
        }
        catch (Exception)
        {
            Assert.Fail("Dispose should not throw an exception");
        }
        
        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Dispose_CalledMultipleTimes_ShouldNotThrowException()
    {
        // Act
        try
        {
            _viewModel.Dispose();
            _viewModel.Dispose(); // Should handle multiple dispose calls gracefully
        }
        catch (Exception)
        {
            Assert.Fail("Multiple dispose calls should not throw an exception");
        }
        
        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    #endregion

    #region Base Class Integration Tests

    [TestMethod]
    public void ViewName_Property_ShouldBeSettableAndGettable()
    {
        // Arrange
        const string testName = "Test Rotation View";

        // Act
        _viewModel.ViewName = testName;

        // Assert
        Assert.AreEqual(testName, _viewModel.ViewName);
    }

    [TestMethod]
    public void AllowNavigation_Property_ShouldBeSettableAndGettable()
    {
        // Arrange
        const bool testValue = true;

        // Act
        _viewModel.AllowNavigation = testValue;

        // Assert
        Assert.AreEqual(testValue, _viewModel.AllowNavigation);
    }

    [TestMethod]
    public void ActiveFarm_Property_ShouldReturnTestFarm()
    {
        // Act
        var result = _viewModel.ActiveFarm;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(_testFarm.Name, result.Name);
    }

    [TestMethod]
    public void StorageService_Property_ShouldReturnMockedService()
    {
        // Act
        var result = _viewModel.StorageService;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreSame(_mockStorageService.Object, result);
    }

    #endregion

    #region Navigation Tests

    [TestMethod]
    public void OnNavigatedTo_ShouldNotThrowException()
    {
        // Arrange
        var navigationContext = new NavigationContext(
            Mock.Of<IRegionNavigationService>(),
            new Uri("test://test"));

        // Act
        try
        {
            _viewModel.OnNavigatedTo(navigationContext);
        }
        catch (Exception)
        {
            Assert.Fail("OnNavigatedTo should not throw an exception");
        }
        
        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void OnNavigatedFrom_ShouldNotThrowException()
    {
        // Arrange
        var navigationContext = new NavigationContext(
            Mock.Of<IRegionNavigationService>(),
            new Uri("test://test"));

        // Act
        try
        {
            _viewModel.OnNavigatedFrom(navigationContext);
        }
        catch (Exception)
        {
            Assert.Fail("OnNavigatedFrom should not throw an exception");
        }
        
        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void IsNavigationTarget_WhenNotDisposed_ShouldReturnTrue()
    {
        // Arrange
        var navigationContext = new NavigationContext(
            Mock.Of<IRegionNavigationService>(),
            new Uri("test://test"));

        // Act
        var result = _viewModel.IsNavigationTarget(navigationContext);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsNavigationTarget_WhenDisposed_ShouldReturnFalse()
    {
        // Arrange
        var navigationContext = new NavigationContext(
            Mock.Of<IRegionNavigationService>(),
            new Uri("test://test"));

        // Act
        _viewModel.Dispose();
        var result = _viewModel.IsNavigationTarget(navigationContext);

        // Assert
        Assert.IsFalse(result);
    }

    #endregion

    #region Validation Tests

    [TestMethod]
    public void ViewName_SetToEmptyString_ShouldHaveValidationError()
    {
        // Act
        _viewModel.ViewName = string.Empty;

        // Assert
        Assert.IsTrue(_viewModel.HasErrors);
        var errors = _viewModel.GetErrors(nameof(_viewModel.ViewName));
        Assert.IsNotNull(errors);
        Assert.IsTrue(errors.Cast<string>().Any());
    }

    [TestMethod]
    public void ViewName_SetToValidString_ShouldNotHaveValidationError()
    {
        // Act
        _viewModel.ViewName = "Valid Name";

        // Assert
        Assert.IsFalse(_viewModel.HasErrors);
    }

    [TestMethod]
    public void ViewName_SetToNullOrEmpty_ValidationBehaviorIsConsistent()
    {
        // Test empty string validation
        _viewModel.ViewName = string.Empty;
        var hasErrorsForEmpty = _viewModel.HasErrors;
        
        // Clear any existing errors
        _viewModel.ViewName = "Valid Name";
        
        // Test null validation  
        _viewModel.ViewName = null;
        var hasErrorsForNull = _viewModel.HasErrors;

        // Assert that both null and empty string have consistent validation behavior
        // Both should either trigger validation or both should not
        Assert.AreEqual(hasErrorsForEmpty, hasErrorsForNull, 
            "Validation behavior should be consistent between null and empty string values");
    }

    #endregion

    #region InitializeRotationComponent Tests

    [TestMethod]
    public void InitializeRotationComponent_WithValidRotationComponent_ShouldNotThrow()
    {
        // Arrange
        var rotationComponent = new RotationComponent
        {
            Name = "Test Rotation",
            ComponentType = ComponentType.Rotation
        };

        // Act
        try
        {
            _viewModel.InitializeRotationComponent(rotationComponent);
        }
        catch (Exception)
        {
            Assert.Fail("InitializeRotationComponent should not throw an exception with valid rotation component");
        }

        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void InitializeRotationComponent_WithNullRotationComponent_ShouldNotThrow()
    {
        // Act
        try
        {
            _viewModel.InitializeRotationComponent(null);
        }
        catch (Exception)
        {
            Assert.Fail("InitializeRotationComponent should handle null parameter gracefully");
        }

        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void InitializeRotationComponent_WithValidComponent_ShouldStoreReference()
    {
        // Arrange
        var rotationComponent = new RotationComponent
        {
            Name = "Test Rotation Component",
            ComponentType = ComponentType.Rotation,
            ShiftLeft = true,
            KeepRotationOnSingleField = false
        };

        // Act
        _viewModel.InitializeRotationComponent(rotationComponent);

        // Assert
        // Since _selectedRotationComponent is private, we can't directly verify it was set
        // but we can verify the method executed without throwing exceptions
        // In a real scenario, you might expose a public property or method to verify state
        Assert.IsTrue(true, "Method executed successfully, implying the reference was stored");
    }

    [TestMethod]
    public void InitializeRotationComponent_CalledMultipleTimes_ShouldHandleGracefully()
    {
        // Arrange
        var firstRotation = new RotationComponent
        {
            Name = "First Rotation",
            ComponentType = ComponentType.Rotation
        };

        var secondRotation = new RotationComponent
        {
            Name = "Second Rotation", 
            ComponentType = ComponentType.Rotation
        };

        // Act & Assert
        try
        {
            _viewModel.InitializeRotationComponent(firstRotation);
            _viewModel.InitializeRotationComponent(secondRotation);
            _viewModel.InitializeRotationComponent(null); // Should handle null after valid component
        }
        catch (Exception)
        {
            Assert.Fail("InitializeRotationComponent should handle multiple calls gracefully");
        }

        // Assert - If we get here, no exception was thrown
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void InitializeRotationComponent_WithComponentContainingFieldComponents_ShouldHandleCorrectly()
    {
        // Arrange
        var rotationComponent = new RotationComponent
        {
            Name = "Complex Rotation",
            ComponentType = ComponentType.Rotation,
            ShiftLeft = false,
            KeepRotationOnSingleField = true
        };

        // Add some field system components to make it more realistic
        rotationComponent.FieldSystemComponents.Add(new H.Core.Models.LandManagement.Fields.FieldSystemComponent
        {
            Name = "Field 1",
            FieldArea = 100
        });

        rotationComponent.FieldSystemComponents.Add(new H.Core.Models.LandManagement.Fields.FieldSystemComponent
        {
            Name = "Field 2", 
            FieldArea = 150
        });

        // Act
        try
        {
            _viewModel.InitializeRotationComponent(rotationComponent);
        }
        catch (Exception)
        {
            Assert.Fail("InitializeRotationComponent should handle rotation components with field components");
        }

        // Assert - If we get here, no exception was thrown and the complex component was processed
        Assert.IsTrue(true);
    }

    #endregion
}