using H.CLI.Processors;

namespace H.CLI.Test.Processors
{
    [TestClass]
    public class ProccessorHandlerTest
    {
        [TestMethod]
        public void TestSetProcessor()
        {
            var processorHandler = new ProcessorHandler();
            processorHandler.SetProccessor(new ShelterbeltProcessor());
            Assert.IsInstanceOfType(processorHandler._processor, typeof(ShelterbeltProcessor));
        }
    }
}
