using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MoviesGlobalResources
{
    public class MongoDBDAO
    {
        private MongoClient _client;
        private IMongoCollection<BsonDocument> _movieCollection;
        private const string _connectionString = "mongodb://SAUCOMPUTER01:27017/admin";
        private const string _collectionName = "MovieCollection";
        private const string _dbName = "Movie";

        public MongoDBDAO()
        {
            _client = new MongoClient(_connectionString);
            _movieCollection = _client.GetDatabase(_dbName).GetCollection<BsonDocument>(_collectionName);
        }

        public BsonDocument GetMovie(int movieID)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("id", movieID);
            return _movieCollection.Find(filter).FirstOrDefault();
        }

        //find movies containing movieName in title
        public List<BsonDocument> FindMovies(string movieName)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Regex("title", new BsonRegularExpression(movieName));
            return _movieCollection.Find(filter).ToList();
        }

        public bool MovieExists(int movieID)
        {
            return GetMovie(movieID) != null;
        }
        public bool MovieExists(string movieName)
        {
            //  return GetMovie(movieName) != null;
            return true;
        }

    }
}
