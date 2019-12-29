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
    }
}
