using AutoMapper;
using H.Core.Calculators.Infrastructure;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Emissions.Results;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Mappers;
using H.Core.Models;
using H.Core.Models.Animals;
using H.Core.Models.Infrastructure;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Ioc;

namespace H.Core.Test.Services.Animals;

[TestClass]
public class AnimalComponentServiceTests
{
    #region Fields

    private AnimalComponentService _service;
    private Mock<IAnimalComponentFactory> _mockAnimalComponentFactory;
    private Mock<ITransferService<AnimalComponentBase, AnimalComponentDto>> _mockTransferService;

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
        _mockAnimalComponentFactory = new Mock<IAnimalComponentFactory>();
        _mockTransferService = new Mock<ITransferService<AnimalComponentBase, AnimalComponentDto>>();
        var mockLogger = new Mock<ILogger>();
        var mockContainerProvider = new Mock<IContainerProvider>();

        mockContainerProvider.Setup(x => x.Resolve(typeof(IMapper), It.IsAny<string>())).Returns(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AnimalComponentDtoToAnimalComponentMapper>();
        }).CreateMapper());

        // Updated constructor to include the transfer service
        _service = new AnimalComponentService(
            mockLogger.Object,
            _mockAnimalComponentFactory.Object,
            _mockTransferService.Object
        );
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    [TestMethod]
    public void InitializeAnimalComponent_SetsIsInitialized_WhenNotAlreadyInitialized()
    {
        // Arrange
        var farm = new Farm();
        var mockAnimalComponent = new Mock<AnimalComponentBase>();
        mockAnimalComponent.CallBase = true;
        mockAnimalComponent.Object.IsInitialized = false;

        // Act
        _service.InitializeComponent(farm, mockAnimalComponent.Object);

        // Assert
        Assert.IsTrue(mockAnimalComponent.Object.IsInitialized);
    }

    [TestMethod]
    public void InitializeAnimalComponent_DoesNotSetIsInitialized_WhenAlreadyInitialized()
    {
        // Arrange
        var farm = new Farm();
        var mockAnimalComponent = new Mock<AnimalComponentBase>();
        mockAnimalComponent.CallBase = true;
        mockAnimalComponent.Object.IsInitialized = true;

        // Act
        _service.InitializeComponent(farm, mockAnimalComponent.Object);

        // Assert
        Assert.IsTrue(mockAnimalComponent.Object.IsInitialized); // Remains true
    }
}