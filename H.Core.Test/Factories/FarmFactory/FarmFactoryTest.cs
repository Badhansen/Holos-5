using H.Core.Factories;
using H.Core.Factories.FarmFactory;
using H.Core.Providers.Feed;
using H.Core.Services.DietService;
using H.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Prism.Ioc;

namespace H.Core.Test;

[TestClass]
public class FarmFactoryTest
{
    #region Fields

    private IFarmFactory _sut;

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
        var mockDietService = new Mock<IDietService>();
        var mockCacheService = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger>();

        _sut = new FarmFactory(mockDietService.Object, mockCacheService.Object, mockLogger.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    [TestMethod]
    public void Create_AddsDietsToFarmDto()
    {
        // Arrange
        var mockDietService = new Mock<IDietService>();
        var mockCacheService = new Mock<ICacheService>();
        var mockLogger = new Mock<ILogger>();

        var mockDiets = new List<IDietDto>
        {
            new Mock<IDietDto>().Object,
            new Mock<IDietDto>().Object
        };

        mockDietService.Setup(x => x.GetDiets()).Returns(mockDiets);

        var sut = new FarmFactory(mockDietService.Object, mockCacheService.Object, mockLogger.Object);

        // Act
        var farm = sut.Create();

        // Assert
        Assert.IsNotNull(farm);
        Assert.IsNotNull(farm.Diets);
        Assert.AreEqual(mockDiets.Count, farm.Diets.Count);
        foreach (var diet in mockDiets)
        {
            Assert.IsTrue(farm.Diets.Contains(diet));
        }
    }
}