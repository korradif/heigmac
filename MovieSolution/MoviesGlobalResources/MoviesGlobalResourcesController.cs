using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using static MoviesGlobalResources.TMDBDAO;

namespace MoviesGlobalResources
{
    public class MoviesGlobalResourcesController
    {
        private readonly MongoDBDAO _cacheDAO = new MongoDBDAO();//private readonly ICache _cacheDAO;
        private readonly TMDBDAO _globalMoviesDAO = new TMDBDAO();

        private readonly Dictionary<Request, List<string>> _cachedRequests = new Dictionary<Request, List<string>>();
        private Dictionary<string, int> _moviesGenres;

        public MoviesGlobalResourcesController()
        {
            _cachedRequests.Add(Request.GetByName, new List<string>());
            _cachedRequests.Add(Request.GetByFilter, new List<string>());
        }
        
        public void ClearCache()
        {
            _cacheDAO.Clear();
        }

        public string GetMoviesByFilter(Filter filter, string value)
        {
            return _globalMoviesDAO.GetMoviesByFilter(filter, value);
        }
        public string GetMoviesByName(string value)
        {
            string result = String.Empty;
            if (RequestExistsInCache(Request.GetByName, value))
            {
                result += "{\"results\":[";
                _cacheDAO.FindMovies(value).ForEach(x => result += x.ToString() + ", ");
                result = result.Remove(result.Length - 2);
                result += "]}";
            }
            else
            {
                result = _globalMoviesDAO.GetMoviesByName(value);
                InsertRequestInCache(Request.GetByName, value, result);
            }
            return result; 
        }
        private enum Request
        {
            GetByName,
            GetByFilter
        }
        private bool RequestExistsInCache(Request req, string value)
        {
            return !String.IsNullOrEmpty(_cachedRequests[req].Find(x => String.Equals(value, x)));
        }
        private void InsertRequestInCache(Request req, string value, string requestResult)
        {
            //todo add data in mongodb
            JObject jsonMovie = JObject.Parse(requestResult);
            JArray jsonResults = (JArray)jsonMovie.SelectToken("results");
            foreach (JToken jResult in jsonResults)
            {
                _cacheDAO.InsertMovie(jResult.ToString());
            }
            _cachedRequests[req].Add(value);
        }

        public Dictionary<string,int> GetMovieGenres()
        {
            if (_moviesGenres is null)
            {
                string jsonMoviesGenres = _globalMoviesDAO.GetMoviesGenres();
                _moviesGenres = new Dictionary<string, int>();
                
                JObject jsonMovie = JObject.Parse(jsonMoviesGenres);
                JArray jsonResults = (JArray)jsonMovie.SelectToken("genres");
                foreach (JToken jResult in jsonResults)
                {
                    _moviesGenres.Add((string)jResult.SelectToken("name"), (int)jResult.SelectToken("id"));    
                }

            }

            return _moviesGenres;
        }

        public int GetMovieGenreIdByName(string genreName)
        {
            int id = 0;
            //TODO return the id of the genre from it's name
            _moviesGenres.TryGetValue(genreName,out id);
            return id;
        }
    }
}
