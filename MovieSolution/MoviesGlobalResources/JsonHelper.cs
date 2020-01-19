using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesGlobalResources
{
    public static class JsonHelper
    {
        public static JArray GetMoviesJArrayFromRawJson(string rawJson)
        {
            JObject jsonMovie = JObject.Parse(rawJson);
            return (JArray)jsonMovie.SelectToken("results");
        }

    }
}
