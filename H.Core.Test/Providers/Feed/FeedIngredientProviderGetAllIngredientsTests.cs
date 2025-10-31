using Microsoft.VisualStudio.TestTools.UnitTesting;
using H.Core.Enumerations;
using H.Core.Providers.Feed;
using System.Linq;

namespace H.Core.Test.Providers.Feed
{
    /// <summary>
    /// Unit tests for the new GetAllIngredientsForAnimalType method
    /// </summary>
    [TestClass]
    public class FeedIngredientProviderTests
    {
        [TestMethod]
        public void GetAllIngredientsForAnimalType_BeefAnimalType_ReturnsIngredients()
        {
            // Arrange
            var provider = new FeedIngredientProvider();

            // Act
            var result = provider.GetAllIngredientsForAnimalType(AnimalType.Beef);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            foreach (var ingredient in result)
            {
                Assert.IsNotNull(ingredient);
            }
        }

        [TestMethod]
        public void GetAllIngredientsForAnimalType_DairyAnimalType_ReturnsIngredients()
        {
            // Arrange
            var provider = new FeedIngredientProvider();

            // Act
            var result = provider.GetAllIngredientsForAnimalType(AnimalType.Dairy);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            foreach (var ingredient in result)
            {
                Assert.IsNotNull(ingredient);
            }
        }

        [TestMethod]
        public void GetAllIngredientsForAnimalType_SwineAnimalType_ReturnsIngredients()
        {
            // Arrange
            var provider = new FeedIngredientProvider();

            // Act
            var result = provider.GetAllIngredientsForAnimalType(AnimalType.Swine);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
            Assert.AreEqual(123, result.Count());

        }
    }
}

