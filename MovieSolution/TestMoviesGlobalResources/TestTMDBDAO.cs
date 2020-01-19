using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesGlobalResources;
using System;
using System.Diagnostics;

namespace TestMoviesGlobalResources
{
    [TestClass]
    public class TestTMDBDAO
    {
        private TMDBDAO _tmdbDAO;

        [TestInitialize]
        public void Setup()
        {
            _tmdbDAO = new TMDBDAO();
        }

        [TestMethod]
        public void TestGetMoviesByName()
        {
            var film = _tmdbDAO.GetMoviesByName("harry potter");
            Trace.WriteLine(film);
            Assert.AreNotEqual(film, null);
        }
        [TestMethod]
        public void TestGetMoviesByFilter()
        {
            var film1 = _tmdbDAO.GetMoviesByFilter(TMDBDAO.Filter.GENRES, "28", TMDBDAO.Operator.AND, "16", TMDBDAO.Operator.AND, "35", TMDBDAO.Operator.AND, "80", TMDBDAO.Operator.AND, "10749", TMDBDAO.Operator.AND, "12");
            var film2 = _tmdbDAO.GetMoviesByFilter(TMDBDAO.Filter.GENRES, "28", TMDBDAO.Operator.AND, "16", TMDBDAO.Operator.AND, "35000", TMDBDAO.Operator.AND, "80");
            Trace.WriteLine(film2);
            Assert.AreNotEqual(film1, null);
            Assert.IsTrue(film1.Length > 100);//length > 100 means that the request return results
            Assert.IsTrue(film2.Length < 100);//length < 100 means that the request return no result
        }
    }
}
