using MongoDB.Bson;
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
            GetMovieGenres();
            _cachedRequests.Add(Request.GetByName, new List<string>());
            _cachedRequests.Add(Request.GetByFilter, new List<string>());
        }
        
        public void ClearCache()
        {
            _cacheDAO.Clear();
        }

        public string GetMoviesByFilter(Filter filter, string value)
        {
            if(RequestExistsInCache(Request.GetByFilter, value))
            {
                switch (filter)
                {
                    case Filter.CAST:
                        return "";
                    case Filter.CREW:
                        return "";
                    case Filter.GENRES:
                        return ParseBsonDocumentListAsJson(_cacheDAO.GetMoviesByGenre(GetMovieGenreIdByName(value)));
                    case Filter.LANGUAGE:
                        return ParseBsonDocumentListAsJson(_cacheDAO.GetMoviesByLanguage(value));
                    case Filter.YEAR:
                        return ParseBsonDocumentListAsJson(_cacheDAO.GetMoviesByYear(Int32.Parse(value)));
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                string requestResult;
                switch (filter)
                {
                    case Filter.GENRES:
                        requestResult = _globalMoviesDAO.GetMoviesByFilter(filter, GetMovieGenreIdByName(value).ToString());
                        break;
                    default:
                        requestResult = _globalMoviesDAO.GetMoviesByFilter(filter, value);
                        break;
                }
                InsertRequestInCache(Request.GetByFilter, value, requestResult);
                return requestResult;
            }
        }
        public string GetMoviesByName(string value)
        {
            string result = String.Empty;
            if (RequestExistsInCache(Request.GetByName, value))
            {
                result += ParseBsonDocumentListAsJson(_cacheDAO.FindMovies(value));
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

        private string ParseBsonDocumentListAsJson(List<BsonDocument> movieList)
        {
            string result = "{\"results\":[";
            movieList.ForEach(x => result += x.ToString() + ", ");
            result = result.Remove(result.Length - 2);
            result += "]}";
            return result;
        }
    }
}
