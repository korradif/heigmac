using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesGlobalResources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TestMoviesGlobalResources
{
    [TestClass]
    public class TestMoviesGlobalResourcesController
    {
        private MoviesGlobalResourcesController _moviesGlobalResCtl;


        [TestInitialize]
        public void Setup()
        {
            _moviesGlobalResCtl = new MoviesGlobalResourcesController();
            _moviesGlobalResCtl.ClearCache();
        }

        [TestMethod]
        public void TestGetMoviesByName()
        {
            var filmFromApi = _moviesGlobalResCtl.GetMoviesByName("Star");//Should first call api
            Trace.WriteLine(filmFromApi);
            var filmFromCache = _moviesGlobalResCtl.GetMoviesByName("Star");//Should then use cache
            Trace.WriteLine(filmFromCache);
            int filmFromApiCount = JsonHelper.GetMoviesJArrayFromRawJson(filmFromApi).Count;
            int filmFromCacheCount = JsonHelper.GetMoviesJArrayFromRawJson(filmFromCache).Count;
            Trace.WriteLine("Film from Api count:" + filmFromApiCount);
            Trace.WriteLine("Film from Cache count:" + filmFromCacheCount);
            Assert.AreEqual(filmFromApiCount, filmFromCacheCount);
        }
        
        [TestMethod]
        public void TestGetMovieGenres()
        {
            var genres = _moviesGlobalResCtl.GetMovieGenres();
            Assert.AreNotEqual(genres.Count, 0);
        }

        [TestMethod]
        public void TestGetMoviesByFilterCast()
        {
            //todo
            /*
            var moviesFromApi = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CAST, "Action");//Should first call api
            Trace.WriteLine(moviesFromApi);
            var moviesFromCache = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CAST, "Action");//Should then use cache
            Trace.WriteLine(moviesFromCache);
            int moviesFromApiCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromApi).Count;
            int moviesFromCacheCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromCache).Count;
            Trace.WriteLine("movies from Api count:" + moviesFromApiCount);
            Trace.WriteLine("movies from Cache count:" + moviesFromCacheCount);
            Assert.AreEqual(moviesFromApiCount, moviesFromCacheCount);
            Assert.AreNotEqual(0, moviesFromCacheCount);
            */
        }

        [TestMethod]
        public void TestGetMoviesByFilterCrew()
        {
            //todo
            /*
            var moviesFromApi = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CREW, "Action");//Should first call api
            Trace.WriteLine(moviesFromApi);
            var moviesFromCache = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CREW, "Action");//Should then use cache
            Trace.WriteLine(moviesFromCache);
            int moviesFromApiCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromApi).Count;
            int moviesFromCacheCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromCache).Count;
            Trace.WriteLine("movies from Api count:" + moviesFromApiCount);
            Trace.WriteLine("movies from Cache count:" + moviesFromCacheCount);
            Assert.AreEqual(moviesFromApiCount, moviesFromCacheCount);
            Assert.AreNotEqual(0, moviesFromCacheCount);
            */
        }

        [TestMethod]
        public void TestGetMoviesByFilterGenre() 
        { 
            var moviesFromApi = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.GENRES, "Action");//Should first casll api
            Trace.WriteLine(moviesFromApi);
            var moviesFromCache = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.GENRES, "Action");//Should then use cache
            Trace.WriteLine(moviesFromCache);
            int moviesFromApiCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromApi).Count;
            int moviesFromCacheCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromCache).Count;
            Trace.WriteLine("movies from Api count:" + moviesFromApiCount);
            Trace.WriteLine("movies from Cache count:" + moviesFromCacheCount);
            Assert.AreEqual(moviesFromApiCount, moviesFromCacheCount);
            Assert.AreNotEqual(0, moviesFromCacheCount);
        }
        [TestMethod]
        public void TestGetMoviesByFilterLanguage()
        {
            var moviesFromApi = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.LANGUAGE, "en");//Should first call api
            Trace.WriteLine(moviesFromApi);
            var moviesFromCache = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.LANGUAGE, "en");//Should then use cache
            Trace.WriteLine(moviesFromCache);
            int moviesFromApiCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromApi).Count;
            int moviesFromCacheCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromCache).Count;
            Trace.WriteLine("movies from Api count:" + moviesFromApiCount);
            Trace.WriteLine("movies from Cache count:" + moviesFromCacheCount);
            Assert.AreEqual(moviesFromApiCount, moviesFromCacheCount);
            Assert.AreNotEqual(0, moviesFromCacheCount);
        }
        [TestMethod]
        public void TestGetMoviesByFilterYear()
        {
            //use release date
            
            var moviesFromApi = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.YEAR, "1989");//Should first call api
            Trace.WriteLine(moviesFromApi);
            var moviesFromCache = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.YEAR, "1989");//Should then use cache
            Trace.WriteLine(moviesFromCache);
            int moviesFromApiCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromApi).Count;
            int moviesFromCacheCount = JsonHelper.GetMoviesJArrayFromRawJson(moviesFromCache).Count;
            Trace.WriteLine("movies from Api count:" + moviesFromApiCount);
            Trace.WriteLine("movies from Cache count:" + moviesFromCacheCount);
            Assert.AreEqual(moviesFromApiCount > 5, moviesFromCacheCount > 5); //this test is not relevant, but the exact count won't be the same as the externe api return movies "around" the provided year
            Assert.AreNotEqual(0, moviesFromCacheCount);
            
        }
    }
}
