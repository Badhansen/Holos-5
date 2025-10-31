using H.Core.Factories;
using H.Core.Models.LandManagement.Fields;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.ObjectModel;
using H.Core.Calculators.UnitsOfMeasurement;
using Prism.Ioc;

namespace H.Core.Test.Factories;

[TestClass]
public class FieldComponentDtoFactoryTest
{
    #region Fields

    private IFieldFactory _factory;

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
        var mockContainerProvider = new Mock<IContainerProvider>();

        _factory = new FieldFactory(mockContainerProvider.Object);
    }

    [TestCleanup]
    public void TestCleanup()
    {
    }

    #endregion

    #region Tests



    #endregion
}