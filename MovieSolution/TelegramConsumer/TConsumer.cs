using System;

namespace TelegramConsumer
{
    public class TConsumer
    {
        private string _apiKey;

        public TConsumer(string apiKey)
        {
            _apiKey = apiKey;
        }

        public string ApiKey => _apiKey;
    }
}
