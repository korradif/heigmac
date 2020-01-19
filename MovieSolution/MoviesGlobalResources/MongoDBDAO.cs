using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MoviesGlobalResources
{
    public class MongoDBDAO
    {
        private MongoClient _client;
        private IMongoCollection<BsonDocument> _movieCollection;
        private IMongoDatabase _movieDB;
        private const string _connectionString = "mongodb://localhost:27017/admin";

        private const string _collectionName = "MovieCollection";
        private const string _dbName = "Movie";

        public void SetupTest()
        {
            Clear();
        }

        public MongoDBDAO()
        {
            _client = new MongoClient(_connectionString);
            _movieDB = _client.GetDatabase(_dbName);
            _movieCollection = _movieDB.GetCollection<BsonDocument>(_collectionName);
        }


        public BsonDocument GetMovie(int movieID)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("id", movieID);
            return _movieCollection.Find(filter).FirstOrDefault();
        }
        //movieName as to be the exact name as "orginal_title"
        public BsonDocument GetMovie(string movieName)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("original_title", movieName);
            return _movieCollection.Find(filter).FirstOrDefault();
        }

        internal List<BsonDocument> GetMoviesByGenre(int genreId)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.AnyEq("genre_ids", genreId);
            return PrepareResult(_movieCollection.Find(filter).ToList());
        }

        internal List<BsonDocument> GetMoviesByYear(int year)
        {
            var builder = Builders<BsonDocument>.Filter;
            var releaseDateStart = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var releaseDateEnd = new DateTime(year, 12, 31, 0, 0, 0, DateTimeKind.Utc);

            /*         var filter2 = builder.Eq("original_language", "en");
                     return PrepareResult(_movieCollection.Find(filter2).ToList());
         */

            var filter4 = builder.Gte("release_date", "1989-01-01") & builder.Lte("release_date", "1989-12-31");
            return PrepareResult(_movieCollection.Find(filter4).ToList());

            /*var filter = builder.And(builder.Gte("release_date", releaseDateStart),
                                     builder.Lte("release_date", releaseDateEnd));
            return PrepareResult(_movieCollection.Find(filter).ToList());*/
        }

        internal List<BsonDocument> GetMoviesByLanguage(string value)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("original_language", value);
            return PrepareResult(_movieCollection.Find(filter).ToList());
        }

        internal void Clear()
        {
            _movieDB.DropCollection(_collectionName);
        }


        //find movies containing movieName in original_title
        public List<BsonDocument> FindMovies(string movieName)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Regex("original_title", new BsonRegularExpression(movieName));
            return PrepareResult(_movieCollection.Find(filter).ToList());
        }

        public void InsertMovie(string movieJson)
        {
            var document = BsonSerializer.Deserialize<BsonDocument>(movieJson);
            _movieCollection.InsertOne(document);
        }

        public bool MovieExists(int movieID)
        {
            return GetMovie(movieID) != null;
        }
        public bool MovieExists(string movieName)
        {
            return GetMovie(movieName) != null;
        }

        private List<BsonDocument> PrepareResult(List<BsonDocument> listMovies)
        {
            listMovies.ForEach(x => x.Remove("_id"));//remove mongodb id to avoid error on json parsing due to "ObjectID" mongo func
            return listMovies;
        }
    }
}
