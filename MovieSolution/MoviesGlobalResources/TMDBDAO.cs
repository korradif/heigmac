using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MoviesGlobalResources
{
    public class TMDBDAO
    {
        private const string _apiUrl = @"https://api.themoviedb.org/3";
        private const string _apiKey = @"dca39aa4da3c154aa1c1b0d293e9ba5b";

        //getRequest example: /movie/123     
        //additionalParams example: &language=en-US&query=harry potter
        private string ExecuteRequest(string getRequest, string additionalParams = "")
        {
            string html = string.Empty;
            string url = _apiUrl + getRequest + "?api_key=" + _apiKey + additionalParams;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            return html;
        }

        public string GetMoviesByName(string name)
        {
            return ExecuteRequest("/search/movie", "&query=" + name);
        }

        public enum Operator
        {
            AND, OR
        }

        public string FilterToString(Filter filter)
        {
            switch (filter)
            {
                case Filter.YEAR: return "primary_release_year";
                case Filter.CAST: return "with_cast";
                case Filter.CREW: return "with_crew";
                case Filter.GENRES: return "with_genres";
                case Filter.LANGUAGE: return "with_original_language";
                default: throw new NotImplementedException();
            }
        }

        public enum Filter
        {
            YEAR,
            CAST,
            CREW,
            GENRES,
            LANGUAGE
        }
        
        //multiples values can be used in the filter, use operators and others values to build the filter query
        public string GetMoviesByFilter(Filter filter, string value, Operator? op = null, string value2 = null, Operator? op2 = null, string value3 = null, Operator? op3 = null, string value4 = null,
             Operator? op4 = null, string value5 = null, Operator? op5 = null, string value6 = null)
        {
            return ExecuteRequest("/discover/movie", "&" + FilterToString(filter) + "=" + value + 
               (op is null ? "" : (op == Operator.AND) ? "," : "|") + value2 + (op2 is null ? "" : (op2 == Operator.AND) ? "," : "|") + value3 + (op3 is null ? "" : (op3 == Operator.AND) ? "," : "|") + value4 +
               (op4 is null ? "" : (op4 == Operator.AND) ? "," : "|") + value5 + (op5 is null ? "" : (op5 == Operator.AND) ? "," : "|") + value6);
        }

        internal string GetMoviesGenres()
        {
            return ExecuteRequest("/genre/movie/list");
        }
    }
}
