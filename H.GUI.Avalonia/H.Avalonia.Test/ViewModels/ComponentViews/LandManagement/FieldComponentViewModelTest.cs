using System.Collections.ObjectModel;
using H.Avalonia.ViewModels.ComponentViews.LandManagement;
using H.Avalonia.ViewModels.ComponentViews.LandManagement.Field;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Factories;
using H.Core.Factories.Crops;
using H.Core.Models;
using H.Core.Models.Animals.Beef;
using H.Core.Models.LandManagement.Fields;
using H.Core.Providers.Feed;
using H.Core.Services.LandManagement.Fields;
using H.Core.Services.StorageService;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Events;
using Prism.Regions;

namespace H.Avalonia.Test.ViewModels.ComponentViews.LandManagement;

[TestClass]
public class FieldComponentViewModelTest
{
    #region Fields

    private FieldComponentViewModel _viewModel;
    private Mock<IFieldFactory> _mockFieldComponentDtoFactory;
    private Mock<IFieldComponentService> _mockFieldComponentService;
    private Mock<ICropFactory> _mockCropFactory;

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
        var mockRegionManager = new Mock<IRegionManager>();
        var mockEventAggregator = new Mock<IEventAggregator>();
        var mockStorageService = new Mock<IStorageService>();
        var mockLogger = new Mock<ILogger>();

        mockStorageService.Setup(x => x.Storage).Returns(new H.Core.Storage() { ApplicationData = new ApplicationData() });

        _mockFieldComponentDtoFactory = new Mock<IFieldFactory>();
        _mockFieldComponentService = new Mock<IFieldComponentService>();
        _mockCropFactory = new Mock<ICropFactory>();

        _mockFieldComponentDtoFactory.Setup(x => x.CreateDto(It.IsAny<Farm>())).Returns(new FieldSystemComponentDto());
        _mockFieldComponentService.Setup(x => x.TransferToFieldComponentDto(It.IsAny<FieldSystemComponent>())).Returns(new FieldSystemComponentDto());

        _viewModel = new FieldComponentViewModel(mockRegionManager.Object, mockEventAggregator.Object,
            mockStorageService.Object, _mockFieldComponentService.Object, mockLogger.Object, _mockCropFactory.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void InitializeViewModelSetFieldSystemComponentToNonNull()
    {
        _mockCropFactory.Setup(x => x.CreateDto(It.IsAny<Farm>())).Returns(new CropDto());
        _mockFieldComponentDtoFactory.Setup(factory => factory.CreateDto(It.IsAny<Farm>())).Returns(new FieldSystemComponentDto());

        _viewModel.InitializeViewModel(new FieldSystemComponent());

        Assert.IsNotNull(_viewModel.SelectedFieldSystemComponentDto);
    }

    [TestMethod]
    public void InitializeViewModelSetFieldSystemComponentToNull()
    {
        _viewModel.InitializeViewModel(new BackgroundingComponent());

        Assert.IsNull(_viewModel.SelectedFieldSystemComponentDto);
    }

    [TestMethod]
    public void InitializeViewModelSetCropViewItemToNotNull()
    {
        _mockCropFactory.Setup(x => x.CreateDto(It.IsAny<Farm>())).Returns(new CropDto());

        _viewModel.InitializeViewModel(new FieldSystemComponent() {CropViewItems = new ObservableCollection<CropViewItem>()});

        Assert.IsNotNull(_viewModel.SelectedCropDto);
    }

    [TestMethod]
    public void InitializeViewModelSetCropDtoCollectionToEmpty()
    {
        _mockCropFactory.Setup(x => x.CreateDto(It.IsAny<Farm>())).Returns(new CropDto());

        _viewModel.InitializeViewModel(new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() {  } });

        Assert.IsFalse(_viewModel.SelectedFieldSystemComponentDto.CropDtos.Any());
    }

    #endregion
}