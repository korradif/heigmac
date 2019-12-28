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
        public void TestGetFilm()
        {
            var film = _mongoDBDAO.GetFilm(181812);
            Assert.AreNotEqual(film, null);
        }
    }
}
