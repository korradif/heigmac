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

        public Dictionary<string,int> GetMovieGenres()
        {
            //TODO return the available genres in a string if possible formatted as follow:
            if (_moviesGenres is null)
            {
                _moviesGenres = new Dictionary<string, int>();
                //TODO: query and parse into dictionary this this https://api.themoviedb.org/3/genre/movie/list?api_key=dca39aa4da3c154aa1c1b0d293e9ba5b&language=en-US
                _moviesGenres.Add("Action",28 );
                _moviesGenres.Add("Adventure",12);
                _moviesGenres.Add("Horror", 27);
                _moviesGenres.Add("History", 36);
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
