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
        }

        [TestMethod]
        public void TestGetMoviesByName()
        {
            var film1 = _moviesGlobalResCtl.GetMoviesByName("Star");//Should first call api
            Trace.WriteLine(film1);
            var film2 = _moviesGlobalResCtl.GetMoviesByName("Star");//Should then use cache
            Trace.WriteLine(film2);
            Assert.IsFalse(String.Equals(film1, film2));
        }
    }
}
