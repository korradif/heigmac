using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TelegramConsumer;

namespace TestTelegramConsumer
{
    [TestClass]
    public class TestTConsumer
    {
        [TestMethod]
        public void TestConnectQueue()
        {
            TelegramConsumer.Program.Main();
            //Assert.AreEqual(Config.TelegramBotToken, client.TelegramBotToken);
        }
        [TestMethod]
        public void TestGetMovie()
        {
            
        }
    }
}
