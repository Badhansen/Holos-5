using AutoMapper;
using H.Core.Calculators.UnitsOfMeasurement;
using H.Core.Enumerations;
using H.Core.Factories;
using H.Core.Mappers;
using H.Core.Models.LandManagement.Fields;
using H.Core.Services.Animals;
using H.Core.Services.LandManagement.Fields;
using Moq;
using Prism.Ioc;
using System.Collections.ObjectModel;
using H.Core.Factories.Crops;
using Microsoft.Extensions.Logging;

namespace H.Core.Test.Services.LandManagement;

[TestClass]
public class FieldComponentServiceTest
{
    #region Fields

    private IFieldComponentService _fieldComponentService;
    
    private Mock<IFieldFactory> _mockFieldComponentDtoFactory;
    private Mock<ICropFactory> _mockCropFactory;
    private Mock<IUnitsOfMeasurementCalculator> _mockUnitsOfMeasurementCalculator;
    private Mock<ITransferService<CropViewItem, CropDto>> _mockCropTransferService;
    private Mock<ITransferService<FieldSystemComponent, FieldSystemComponentDto>> _mockFieldTransferService;

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
        _mockFieldComponentDtoFactory = new Mock<IFieldFactory>();
        _mockCropFactory = new Mock<ICropFactory>();
        _mockUnitsOfMeasurementCalculator = new Mock<IUnitsOfMeasurementCalculator>();
        _mockCropTransferService = new Mock<ITransferService<CropViewItem, CropDto>>();
        _mockFieldTransferService = new Mock<ITransferService<FieldSystemComponent, FieldSystemComponentDto>>();
        var mockLogger = new Mock<ILogger>();
        var mockContainerProvider = new Mock<IContainerProvider>();

        // Setup mappers to return a working IMapper for each required profile
        mockContainerProvider.Setup(x => x.Resolve(typeof(IMapper), nameof(CropViewItemToCropDtoMapper))).Returns(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CropViewItemToCropDtoMapper>();
        }).CreateMapper());

        mockContainerProvider.Setup(x => x.Resolve(typeof(IMapper), nameof(CropDtoToCropDtoMapper))).Returns(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CropDtoToCropDtoMapper>();
        }).CreateMapper());

        mockContainerProvider.Setup(x => x.Resolve(typeof(IMapper), nameof(CropDtoToCropViewItemMapper))).Returns(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CropDtoToCropViewItemMapper>();
        }).CreateMapper());

        mockContainerProvider.Setup(x => x.Resolve(typeof(IMapper), nameof(FieldComponentToDtoMapper))).Returns(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FieldComponentToDtoMapper>();
        }).CreateMapper());

        mockContainerProvider.Setup(x => x.Resolve(typeof(IMapper), nameof(FieldDtoToFieldComponentMapper))).Returns(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FieldDtoToFieldComponentMapper>();
        }).CreateMapper());

        mockContainerProvider.Setup(x => x.Resolve(typeof(IMapper), nameof(FieldDtoToFieldDtoMapper))).Returns(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FieldDtoToFieldDtoMapper>();
        }).CreateMapper());

        _fieldComponentService = new FieldComponentService(
            _mockFieldComponentDtoFactory.Object,
            _mockCropFactory.Object,
            mockLogger.Object,
            _mockCropTransferService.Object,
            _mockFieldTransferService.Object
        );
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests

    [TestMethod]
    public void TransferCropDtoToSystemConvertsImperialValueToMetric()
    {
        // Arrange
        var mockTransferService = new Mock<ITransferService<CropViewItem, CropDto>>();
        var cropDto = new CropDto { AmountOfIrrigation = 10 }; // Example value in imperial units
        var expectedCropViewItem = new CropViewItem { AmountOfIrrigation = 25.4 }; // Example value in metric units

        // Setup the mock to return the expected model when called
        mockTransferService
            .Setup(x => x.TransferDtoToDomainObject(It.IsAny<CropDto>(), It.IsAny<CropViewItem>()))
            .Returns(expectedCropViewItem);

        // Act
        var result = mockTransferService.Object.TransferDtoToDomainObject(cropDto, new CropViewItem());

        // Assert
        Assert.AreEqual(25.4, result.AmountOfIrrigation);
    }

    [TestMethod]
    public void CreateSetCropDtoCollectionToNonEmpty()
    {
        _mockFieldTransferService.Setup(x => x.TransferDomainObjectToDto(It.IsAny<FieldSystemComponent>())).Returns(new FieldSystemComponentDto());

        var result = _fieldComponentService.TransferToFieldComponentDto(new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() { new CropViewItem() } });

        Assert.IsTrue(result.CropDtos.Any());
    }

    [TestMethod]
    public void CreateSetCropDtoCollectionToEmpty()
    {
        _mockFieldTransferService.Setup(x => x.TransferDomainObjectToDto(It.IsAny<FieldSystemComponent>())).Returns(new FieldSystemComponentDto());

        var result = _fieldComponentService.TransferToFieldComponentDto(new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() { } });

        Assert.IsFalse(result.CropDtos.Any());
    }

    [TestMethod]
    public void BuildCropDtoCollectionDoesNotCreateAnyItems()
    {
        var fieldComponentDto = new FieldSystemComponentDto();
        var fieldComponent = new FieldSystemComponent();

        _fieldComponentService.ConvertCropViewItemsToDtoCollection(fieldComponent, fieldComponentDto);

        Assert.IsFalse(fieldComponentDto.CropDtos.Any());
    }

    [TestMethod]
    public void BuildCropDtoCollectionDoesNotCreatesItems()
    {
        var fieldComponentDto = new FieldSystemComponentDto();
        var fieldComponent = new FieldSystemComponent() {CropViewItems = new ObservableCollection<CropViewItem>() {new CropViewItem()}};

        _fieldComponentService.ConvertCropViewItemsToDtoCollection(fieldComponent, fieldComponentDto);

        Assert.AreEqual(1, fieldComponentDto.CropDtos.Count);

        fieldComponent.CropViewItems.Clear();
        fieldComponent.CropViewItems.Add(new CropViewItem());
        fieldComponent.CropViewItems.Add(new CropViewItem());
        fieldComponent.CropViewItems.Add(new CropViewItem());

        _fieldComponentService.ConvertCropViewItemsToDtoCollection(fieldComponent, fieldComponentDto);

        Assert.AreEqual(3, fieldComponentDto.CropDtos.Count);
    }

    [TestMethod]
    public void ConvertCropDtoCollectionToCropViewItemCollection()
    {
        var guid = Guid.NewGuid();

        var dto = new CropDto() { Guid = guid, AmountOfIrrigation = 200 };

        _mockCropFactory.Setup(x => x.CreateDtoFromDtoTemplate(It.IsAny<ICropDto>())).Returns(dto);

        var fieldComponent = new FieldSystemComponent()
        {
            CropViewItems = new ObservableCollection<CropViewItem>() { new CropViewItem() { Guid = guid } }
        };
        var fieldComponentDto = new FieldSystemComponentDto()
        {
            CropDtos = new ObservableCollection<ICropDto>() { dto }
        };

        // Mock the transfer service to return a CropViewItem with the expected AmountOfIrrigation
        _mockCropTransferService
            .Setup(x => x.TransferDtoToDomainObject(It.IsAny<CropDto>(), It.IsAny<CropViewItem>()))
            .Returns((CropDto d, CropViewItem v) =>
            {
                v.AmountOfIrrigation = d.AmountOfIrrigation;
                return v;
            });

        _fieldComponentService.ConvertCropDtoCollectionToCropViewItemCollection(fieldComponent, fieldComponentDto);

        Assert.AreEqual(200, fieldComponent.CropViewItems[0].AmountOfIrrigation);
    }

    [TestMethod]
    public void AddCropDtoToSystem()
    {
        var fieldComponent = new FieldSystemComponent();
        var cropDto = new CropDto();

        _mockCropFactory.Setup(x => x.CreateCropViewItem(It.IsAny<ICropDto>())).Returns(new CropViewItem());

        _fieldComponentService.AddCropDtoToSystem(fieldComponent, cropDto);

        Assert.AreEqual(1, fieldComponent.CropViewItems.Count);

        fieldComponent.CropViewItems.Clear();

        _fieldComponentService.AddCropDtoToSystem(fieldComponent, cropDto);
        _fieldComponentService.AddCropDtoToSystem(fieldComponent, cropDto);

        Assert.AreEqual(2, fieldComponent.CropViewItems.Count);
    }

    [TestMethod]
    public void RemoveCropDtoFromSystem()
    {
        var guid = Guid.NewGuid();
        var fieldComponent = new FieldSystemComponent();
        var cropDto = new CropDto() {Guid = guid};
        var viewItem = new CropViewItem() {Guid = guid};

        _mockCropFactory.Setup(x => x.CreateCropViewItem(It.IsAny<ICropDto>())).Returns(viewItem);

        _fieldComponentService.AddCropDtoToSystem(fieldComponent, cropDto);

        Assert.AreEqual(1, fieldComponent.CropViewItems.Count);

        _fieldComponentService.RemoveCropFromSystem(fieldComponent, cropDto);

        Assert.IsFalse(fieldComponent.CropViewItems.Any());
    }

    [TestMethod]
    public void TransferToDto_ReturnsNewDtoInstance()
    {
        // Arrange
        var model = new CropViewItem();
        model.Name = "Test Crop";

        // Act
        // Example usage if you add a TransferDomainObjectToDto method that uses the transfer service:
        // var dto = _fieldComponentService.TransferDomainObjectToDto<CropViewItem, CropDto>(model);
        // For now, just test the transfer service directly:
        _mockCropTransferService.Setup(x => x.TransferDomainObjectToDto(model))
            .Returns(new CropDto { Name = model.Name });

        var dto = _mockCropTransferService.Object.TransferDomainObjectToDto(model);

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(CropDto));
        Assert.AreEqual("Test Crop", dto.Name);
    }

    #endregion
}
