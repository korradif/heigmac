using Microsoft.VisualStudio.TestTools.UnitTesting;
using TelegramConsumer;

namespace TestTelegramConsumer
{
    [TestClass]
    public class TestTClient
    {
        [TestMethod]
        public void TestConnectQueue()
        {
            TConsumer client = new TConsumer(Config.ApiKey);
            Assert.AreEqual(Config.ApiKey, client.ApiKey);
        }
    }
}
