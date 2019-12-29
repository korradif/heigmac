using MongoDB.Bson;
using MongoDB.Driver;
using System;

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


    }
}
