using System;
using System.Collections.Generic;
using System.Text;
using static MoviesGlobalResources.TMDBDAO;

namespace MoviesGlobalResources
{
    public class MoviesGlobalResourcesController
    {
        //private readonly ICache _cache;
        private readonly TMDBDAO _globalMoviesDAO = new TMDBDAO();

        public string GetMoviesByFilter(Filter filter, string value)
        {
            return _globalMoviesDAO.GetMoviesByFilter(filter, value);
        }
        public string GetMoviesByName(string value)
        {
            return _globalMoviesDAO.GetMoviesByName(value);
        }
    }
}
