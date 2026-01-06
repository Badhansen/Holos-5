using H.CLI.Factories;
using H.CLI.Processors;
using H.Core.Models.LandManagement.Shelterbelt;

namespace H.CLI.Test.Factorys
{
    [TestClass]
    public class ComponentProcessorFactoryTest
    {
        [TestMethod]
        public void TestComponentProcessorFactory()
        {
            var componentProcessorFactory = new ComponentProcessorFactory();
            var result = componentProcessorFactory.GetComponentProcessor(new ShelterbeltComponent().GetType());
            Assert.IsInstanceOfType(result, typeof(ShelterbeltProcessor));

        }

    }
}
