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

        public MoviesGlobalResourcesController()
        {
            _cachedRequests.Add(Request.GetByName, new List<string>());
            _cachedRequests.Add(Request.GetByFilter, new List<string>());
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
                _cacheDAO.FindMovies(value).ForEach(x => result += x.ToString());
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
        private void InsertRequestInCache(Request req, string value, string result)
        {
            //todo add data in mongodb
            _cachedRequests[req].Add(value);
        }

    }
}
