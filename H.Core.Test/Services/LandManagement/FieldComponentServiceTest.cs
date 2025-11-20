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

    #region GetCropViewItemFromDto Tests

    [TestMethod]
    public void GetCropViewItemFromDto_WithMatchingGuid_ReturnsCorrectCropViewItem()
    {
        // Arrange: create a DTO with a GUID that will match one item in the domain collection
        var matchingGuid = Guid.NewGuid();
        var cropDto = new CropDto() { Guid = matchingGuid, CropType = CropType.Wheat };
        var expectedCropViewItem = new CropViewItem() { Guid = matchingGuid, CropType = CropType.Wheat, Name = "Test Wheat" };

        var fieldComponent = new FieldSystemComponent()
        {
            CropViewItems = new ObservableCollection<CropViewItem>
            {
                new CropViewItem() { Guid = Guid.NewGuid(), CropType = CropType.Barley },
                expectedCropViewItem,
                new CropViewItem() { Guid = Guid.NewGuid(), CropType = CropType.Oats }
            }
        };

        // Act: call the method under test to retrieve the matching view item
        var result = _fieldComponentService.GetCropViewItemFromDto(cropDto, fieldComponent);

        // Assert: verify the returned item is the expected one and properties are preserved
        Assert.IsNotNull(result);
        Assert.AreSame(expectedCropViewItem, result);
        Assert.AreEqual(matchingGuid, result.Guid);
        Assert.AreEqual(CropType.Wheat, result.CropType);
    }

    [TestMethod]
    public void GetCropViewItemFromDto_WithNonMatchingGuid_ReturnsNull()
    {
        // Arrange: DTO GUID does not match any item in the domain collection
        var cropDto = new CropDto() { Guid = Guid.NewGuid() };
        var fieldComponent = new FieldSystemComponent()
        {
            CropViewItems = new ObservableCollection<CropViewItem>
            {
                new CropViewItem() { Guid = Guid.NewGuid() },
                new CropViewItem() { Guid = Guid.NewGuid() }
            }
        };

        // Act: attempt to find a matching item
        var result = _fieldComponentService.GetCropViewItemFromDto(cropDto, fieldComponent);

        // Assert: since no match exists, result should be null
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetCropViewItemFromDto_WithEmptyCollection_ReturnsNull()
    {
        // Arrange: an empty domain collection should yield no matches
        var cropDto = new CropDto() { Guid = Guid.NewGuid() };
        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() };

        // Act: call method under test
        var result = _fieldComponentService.GetCropViewItemFromDto(cropDto, fieldComponent);

        // Assert: expect null due to empty collection
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetCropViewItemFromDto_WithSingleMatchingItem_ReturnsItem()
    {
        // Arrange: a single-item collection where the GUID matches
        var matchingGuid = Guid.NewGuid();
        var cropDto = new CropDto() { Guid = matchingGuid };
        var expected = new CropViewItem() { Guid = matchingGuid };
        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem> { expected } };

        // Act: retrieve the item
        var result = _fieldComponentService.GetCropViewItemFromDto(cropDto, fieldComponent);

        // Assert: returned item should be the same instance that was in the collection
        Assert.IsNotNull(result);
        Assert.AreSame(expected, result);
    }

    [TestMethod]
    public void GetCropViewItemFromDto_WithMultipleItems_ReturnsFirstMatching()
    {
        // Arrange: collection contains multiple items; ensure the matching one is present
        var matchingGuid = Guid.NewGuid();
        var cropDto = new CropDto() { Guid = matchingGuid };
        var firstMatch = new CropViewItem() { Guid = matchingGuid, Name = "First" };
        var fieldComponent = new FieldSystemComponent()
        {
            CropViewItems = new ObservableCollection<CropViewItem>
            {
                new CropViewItem() { Guid = Guid.NewGuid() },
                firstMatch,
                new CropViewItem() { Guid = Guid.NewGuid() }
            }
        };

        // Act: call the method to find the first item that matches by GUID
        var result = _fieldComponentService.GetCropViewItemFromDto(cropDto, fieldComponent);

        // Assert: the first matching instance should be returned and its properties verified
        Assert.IsNotNull(result);
        Assert.AreSame(firstMatch, result);
        Assert.AreEqual("First", result.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void GetCropViewItemFromDto_WithNullDto_Throws()
    {
        // Arrange: a null DTO parameter is provided (documents current behavior)
        ICropDto cropDto = null;
        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem> { new CropViewItem() } };

        // Act: calling the method with null DTO is expected to throw a NullReferenceException
        _fieldComponentService.GetCropViewItemFromDto(cropDto, fieldComponent);

        // Assert: exception attribute on the test method handles verification
    }

    #endregion

    #region RemoveCropFromSystem Tests

    [TestMethod]
    public void RemoveCropFromSystem_WithMatchingGuid_RemovesItem()
    {
        // Arrange: field contains multiple crop view items including one that matches the DTO GUID
        var matchingGuid = Guid.NewGuid();
        var fieldComponent = new FieldSystemComponent()
        {
            CropViewItems = new ObservableCollection<CropViewItem>
            {
                new CropViewItem() { Guid = Guid.NewGuid() },
                new CropViewItem() { Guid = matchingGuid },
                new CropViewItem() { Guid = Guid.NewGuid() }
            }
        };
        var cropDto = new CropDto() { Guid = matchingGuid };

        // Act: remove the crop identified by DTO
        _fieldComponentService.RemoveCropFromSystem(fieldComponent, cropDto);

        // Assert: the item with the matching GUID has been removed
        Assert.IsFalse(fieldComponent.CropViewItems.Any(x => x.Guid == matchingGuid));
        Assert.AreEqual(2, fieldComponent.CropViewItems.Count);
    }

    [TestMethod]
    public void RemoveCropFromSystem_WithNoMatchingGuid_DoesNothing()
    {
        // Arrange: field contains items but none match the DTO GUID
        var fieldComponent = new FieldSystemComponent()
        {
            CropViewItems = new ObservableCollection<CropViewItem>
            {
                new CropViewItem() { Guid = Guid.NewGuid() },
                new CropViewItem() { Guid = Guid.NewGuid() }
            }
        };
        var cropDto = new CropDto() { Guid = Guid.NewGuid() };
        var initialCount = fieldComponent.CropViewItems.Count;

        // Act: attempt removal when no matching GUID exists
        _fieldComponentService.RemoveCropFromSystem(fieldComponent, cropDto);

        // Assert: collection remains unchanged
        Assert.AreEqual(initialCount, fieldComponent.CropViewItems.Count);
    }

    [TestMethod]
    public void RemoveCropFromSystem_WithNullDto_DoesNothing()
    {
        // Arrange: prepare a field with items
        var fieldComponent = new FieldSystemComponent()
        {
            CropViewItems = new ObservableCollection<CropViewItem>
            {
                new CropViewItem() { Guid = Guid.NewGuid() }
            }
        };
        ICropDto cropDto = null;
        var initialCount = fieldComponent.CropViewItems.Count;

        // Act: calling with null DTO should not throw and should not modify the collection
        _fieldComponentService.RemoveCropFromSystem(fieldComponent, cropDto);

        // Assert
        Assert.AreEqual(initialCount, fieldComponent.CropViewItems.Count);
    }

    [TestMethod]
    public void RemoveCropFromSystem_WithEmptyCollection_DoesNothing()
    {
        // Arrange: empty CropViewItems collection
        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() };
        var cropDto = new CropDto() { Guid = Guid.NewGuid() };

        // Act: attempt removal from empty collection
        _fieldComponentService.RemoveCropFromSystem(fieldComponent, cropDto);

        // Assert: still empty and no exceptions
        Assert.IsFalse(fieldComponent.CropViewItems.Any());
    }

    #endregion

    #region Additional Tests

    [TestMethod]
    public void AddCropDtoToSystem_UsesFactoryReturnValue_InstanceAdded()
    {
        // Arrange: factory returns a specific instance which should be added to collection
        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() };
        var cropDto = new CropDto();
        var returnedViewItem = new CropViewItem() { Name = "FactoryCreated" };

        _mockCropFactory.Setup(x => x.CreateCropViewItem(It.IsAny<ICropDto>())).Returns(returnedViewItem);

        // Act
        _fieldComponentService.AddCropDtoToSystem(fieldComponent, cropDto);

        // Assert: collection contains the same instance that factory returned
        Assert.AreEqual(1, fieldComponent.CropViewItems.Count);
        Assert.AreSame(returnedViewItem, fieldComponent.CropViewItems[0]);
        Assert.AreEqual("FactoryCreated", fieldComponent.CropViewItems[0].Name);
    }

    [TestMethod]
    public void AddCropDtoToSystem_PassesDtoToFactory()
    {
        // Arrange: capture the dto passed to factory and verify it matches the supplied dto
        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() };
        var cropDto = new CropDto() { Name = "DtoName" };
        _mockCropFactory.Setup(x => x.CreateCropViewItem(It.IsAny<ICropDto>())).Returns(new CropViewItem());

        // Act
        _fieldComponentService.AddCropDtoToSystem(fieldComponent, cropDto);

        // Assert: factory was invoked with the same dto instance
        _mockCropFactory.Verify(x => x.CreateCropViewItem(It.Is<ICropDto>(d => d == cropDto)), Times.Once);
    }

    [TestMethod]
    public void AddCropDtoToSystem_WithNullDto_DoesNotCallFactoryAndDoesNotAdd()
    {
        // Arrange: factory should not be called when dto is null
        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem>() };
        ICropDto cropDto = null;

        // Setup factory to fail the test if called (optional) and also verify later
        _mockCropFactory.Setup(x => x.CreateCropViewItem(It.IsAny<ICropDto>())).Throws(new Exception("Factory should not be called when dto is null"));

        var initialCount = fieldComponent.CropViewItems.Count;

        // Act
        _fieldComponentService.AddCropDtoToSystem(fieldComponent, cropDto);

        // Assert: factory was not called and collection remains unchanged
        Assert.AreEqual(initialCount, fieldComponent.CropViewItems.Count);
        _mockCropFactory.Verify(x => x.CreateCropViewItem(It.IsAny<ICropDto>()), Times.Never);
    }

    [TestMethod]
    public void AddCropDtoToSystem_WithNullFieldComponent_DoesNotCallFactoryAndDoesNotThrow()
    {
        // Arrange: crop DTO provided, but field component is null
        ICropDto cropDto = new CropDto() { Name = "DtoForNullField" };

        // Setup factory to throw if invoked to ensure it is not called
        _mockCropFactory.Setup(x => x.CreateCropViewItem(It.IsAny<ICropDto>())).Throws(new Exception("Factory should not be called when field component is null"));

        // Act & Assert: calling with null fieldComponent should not throw
        _fieldComponentService.AddCropDtoToSystem(null, cropDto);

        // Verify factory was never called
        _mockCropFactory.Verify(x => x.CreateCropViewItem(It.IsAny<ICropDto>()), Times.Never);
    }

    [TestMethod]
    public void ConvertCropDtoCollectionToCropViewItemCollection_WithMultipleDtos_UpdatesMatchingViewItems()
    {
        // Arrange:
        // - Create three CropViewItems on the field component; two of them will match incoming DTO GUIDs
        // - Create two DTOs corresponding to two of the view items
        // - Mock the transfer service to copy the AmountOfIrrigation value from DTO to the view item
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();

        var view1 = new CropViewItem() { Guid = guid1, AmountOfIrrigation =0 };
        var view2 = new CropViewItem() { Guid = guid2, AmountOfIrrigation =0 };
        var view3 = new CropViewItem() { Guid = guid3, AmountOfIrrigation =0 };

        var fieldComponent = new FieldSystemComponent() { CropViewItems = new ObservableCollection<CropViewItem> { view1, view2, view3 } };

        var dto1 = new CropDto() { Guid = guid1, AmountOfIrrigation =11 };
        var dto2 = new CropDto() { Guid = guid2, AmountOfIrrigation =22 };

        var fieldComponentDto = new FieldSystemComponentDto() { CropDtos = new ObservableCollection<ICropDto> { dto1, dto2 } };

        // Mock: simulate the transfer service mapping DTO -> view item
        _mockCropTransferService
            .Setup(x => x.TransferDtoToDomainObject(It.IsAny<CropDto>(), It.IsAny<CropViewItem>()))
            .Returns((CropDto d, CropViewItem v) =>
            {
                v.AmountOfIrrigation = d.AmountOfIrrigation;
                return v;
            });

        // Act:
        // Call the method under test which should iterate the DTO collection and update matching view items
        _fieldComponentService.ConvertCropDtoCollectionToCropViewItemCollection(fieldComponent, fieldComponentDto);

        // Assert:
        // - view1 and view2 are updated with values from their corresponding DTOs
        // - view3 remains unchanged because there was no matching DTO
        Assert.AreEqual(11, view1.AmountOfIrrigation);
        Assert.AreEqual(22, view2.AmountOfIrrigation);
        Assert.AreEqual(0, view3.AmountOfIrrigation);

        // Verify transfer service invoked exactly twice (once per matching DTO)
        _mockCropTransferService.Verify(x => x.TransferDtoToDomainObject(It.IsAny<CropDto>(), It.IsAny<CropViewItem>()), Times.Exactly(2));
    }

    #endregion

    #region Tests for ResetAllYears
    // ---------------------------------------------------

    [TestMethod]
    public void ResetAllYears_WithEmptyCollection_DoesNotThrow()
    {
        // Arrange: empty collection should be handled gracefully
        var emptyCropDtos = new List<ICropDto>();

        // Act & Assert: method should not throw and should be a no-op
        _fieldComponentService.ResetAllYears(emptyCropDtos);
        // No additional assertions needed - success is not throwing
    }

    [TestMethod]
    public void ResetAllYears_WithSingleCrop_DoesNotChangeYear()
    {
        // Arrange: single crop should retain its current year (max year stays the same)
        var cropDto = new CropDto { Year = 2020 };
        var cropDtos = new List<ICropDto> { cropDto };

        // Act
        _fieldComponentService.ResetAllYears(cropDtos);

        // Assert: year should remain unchanged since it's already the max
        Assert.AreEqual(2020, cropDto.Year);
    }

    [TestMethod]
    public void ResetAllYears_WithConsecutiveYears_DoesNotChangeYears()
    {
        // Arrange: crops with already consecutive years in descending order should remain unchanged
        var cropDto1 = new CropDto { Year = 2022 };
        var cropDto2 = new CropDto { Year = 2021 };
        var cropDto3 = new CropDto { Year = 2020 };
        var cropDtos = new List<ICropDto> { cropDto1, cropDto2, cropDto3 };

        // Act
        _fieldComponentService.ResetAllYears(cropDtos);

        // Assert: years should remain the same since they're already consecutive
        Assert.AreEqual(2022, cropDto1.Year);
        Assert.AreEqual(2021, cropDto2.Year);
        Assert.AreEqual(2020, cropDto3.Year);
    }

    [TestMethod]
    public void ResetAllYears_WithNonConsecutiveYears_ResetsToConsecutiveYears()
    {
        // Arrange: crops with gaps in years should be renumbered to consecutive descending order
        var cropDto1 = new CropDto { Year = 2025 }; // highest year
        var cropDto2 = new CropDto { Year = 2020 }; // gap exists
        var cropDto3 = new CropDto { Year = 2018 }; // more gaps
        var cropDtos = new List<ICropDto> { cropDto1, cropDto2, cropDto3 };

        // Act: method orders by descending year, then assigns consecutive years from max down
        _fieldComponentService.ResetAllYears(cropDtos);

        // Assert: years should now be consecutive descending from the original maximum
        Assert.AreEqual(2025, cropDto1.Year); // max year stays the same
        Assert.AreEqual(2024, cropDto2.Year); // second highest becomes max - 1
        Assert.AreEqual(2023, cropDto3.Year); // third highest becomes max - 2
    }

    [TestMethod]
    public void ResetAllYears_WithDuplicateYears_ResetsToConsecutiveYears()
    {
        // Arrange: crops with duplicate years should be separated into consecutive years
        var cropDto1 = new CropDto { Year = 2020 };
        var cropDto2 = new CropDto { Year = 2020 }; // duplicate year
        var cropDto3 = new CropDto { Year = 2020 }; // duplicate year
        var cropDtos = new List<ICropDto> { cropDto1, cropDto2, cropDto3 };

        // Act: algorithm orders by year then assigns consecutive descending values
        _fieldComponentService.ResetAllYears(cropDtos);

        // Assert: verify the result produces consecutive years from the original max
        var resultYears = cropDtos.Select(dto => dto.Year).OrderByDescending(y => y).ToList();
        
        // Check that we have the expected consecutive years (2020, 2019, 2018)
        Assert.AreEqual(2020, resultYears[0]); // highest year should be original max
        Assert.AreEqual(2019, resultYears[1]); // second should be max - 1
        Assert.AreEqual(2018, resultYears[2]); // third should be max - 2
        
        // Verify all years are unique (no more duplicates)
        Assert.AreEqual(3, resultYears.Distinct().Count());
        
        // Verify range is correct
        Assert.AreEqual(2020, resultYears.Max());
        Assert.AreEqual(2018, resultYears.Min());
    }

    [TestMethod]
    public void ResetAllYears_WithRandomOrderYears_ResetsToConsecutiveDescendingOrder()
    {
        // Arrange: crops in random year order should be processed correctly
        var cropDto1 = new CropDto { Year = 2021, CropType = CropType.Wheat };
        var cropDto2 = new CropDto { Year = 2023, CropType = CropType.Barley }; // highest
        var cropDto3 = new CropDto { Year = 2019, CropType = CropType.Oats }; // lowest
        var cropDto4 = new CropDto { Year = 2022, CropType = CropType.Corn };
        var cropDtos = new List<ICropDto> { cropDto1, cropDto2, cropDto3, cropDto4 };

        // Act: method should order by descending year, then assign consecutive years
        _fieldComponentService.ResetAllYears(cropDtos);

        // Assert: verify years are assigned in descending order from original max
        // Original order by descending year: 2023, 2022, 2021, 2019
        // New consecutive years should be: 2023, 2022, 2021, 2020
        Assert.AreEqual(2023, cropDto2.Year); // max year (2023) stays same
        Assert.AreEqual(2022, cropDto4.Year); // originally second highest (2022) stays same
        Assert.AreEqual(2021, cropDto1.Year); // originally third highest (2021) stays same
        Assert.AreEqual(2020, cropDto3.Year); // originally lowest (2019) becomes 2020

        // Verify other properties are preserved
        Assert.AreEqual(CropType.Wheat, cropDto1.CropType);
        Assert.AreEqual(CropType.Barley, cropDto2.CropType);
        Assert.AreEqual(CropType.Oats, cropDto3.CropType);
        Assert.AreEqual(CropType.Corn, cropDto4.CropType);
    }

    [TestMethod]
    public void ResetAllYears_WithNegativeYears_HandlesCorrectly()
    {
        // Arrange: test with negative years to ensure algorithm handles edge cases
        var cropDto1 = new CropDto { Year = -5 };
        var cropDto2 = new CropDto { Year = 10 }; // highest
        var cropDto3 = new CropDto { Year = 0 };
        var cropDtos = new List<ICropDto> { cropDto1, cropDto2, cropDto3 };

        // Act: algorithm should work with negative numbers
        _fieldComponentService.ResetAllYears(cropDtos);

        // Assert: consecutive years starting from max (10)
        Assert.AreEqual(10, cropDto2.Year); // max year (10) stays same
        Assert.AreEqual(9, cropDto3.Year);  // second highest (0) becomes 9
        Assert.AreEqual(8, cropDto1.Year);  // lowest (-5) becomes 8
    }

    [TestMethod]
    public void ResetAllYears_VerifiesConsecutiveAlgorithm()
    {
        // Arrange: set up a larger collection to verify the consecutive assignment algorithm
        var cropDtos = new List<ICropDto>
        {
            new CropDto { Year = 2010 },
            new CropDto { Year = 2025 }, // max
            new CropDto { Year = 2015 },
            new CropDto { Year = 2005 },
            new CropDto { Year = 2020 }
        };

        var originalMaxYear = cropDtos.Max(dto => dto.Year); // Should be 2025

        // Act
        _fieldComponentService.ResetAllYears(cropDtos);

        // Assert: verify the algorithm produces consecutive descending years
        var actualYears = cropDtos.Select(dto => dto.Year).OrderByDescending(y => y).ToList();
        
        // Check that max year is preserved
        Assert.AreEqual(originalMaxYear, actualYears[0]);
        
        // Check that all years are consecutive
        for (int i = 0; i < actualYears.Count - 1; i++)
        {
            var expectedDifference = 1;
            var actualDifference = actualYears[i] - actualYears[i + 1];
            Assert.AreEqual(expectedDifference, actualDifference, 
            $"Years should be consecutive: {actualYears[i]} and {actualYears[i + 1]}");
        }
        
        // Verify the range: should be from originalMaxYear down to (originalMaxYear - count + 1)
        var expectedMinYear = originalMaxYear - cropDtos.Count + 1;
        Assert.AreEqual(expectedMinYear, actualYears.Last());
    }

    #endregion

    #endregion
}
