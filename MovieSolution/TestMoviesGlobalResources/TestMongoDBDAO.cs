using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesGlobalResources;

namespace TestMoviesGlobalResources
{
    [TestClass]
    public class TestMongoDBDAO
    {
        private MongoDBDAO _mongoDBDAO;

        [TestInitialize]
        public void Setup()
        {
            _mongoDBDAO = new MongoDBDAO();
        }

        [TestMethod]
        public void TestGetMovie()
        {
            var film1 = _mongoDBDAO.GetMovie(181812);//id exists
            Assert.AreNotEqual(film1, null);
            var film2 = _mongoDBDAO.GetMovie(1231231231);//id not exists
            Assert.AreEqual(film2, null);
        }

        [TestMethod]
        public void TestFindMovies()
        {
            var film1 = _mongoDBDAO.FindMovies("Star Wa");//name exists
            Assert.AreNotEqual(film1.Count, 0);
            var film2 = _mongoDBDAO.FindMovies("Star Wa123");//name not exists
            Assert.AreEqual(film2.Count, 0);
        }

    }
}
