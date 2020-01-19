using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialResources
{
    public class MovieController
    {
        Neo4JDAO _neo4JDAO;
        public MovieController()
        {
            _neo4JDAO = new Neo4JDAO();
        }

        public void Connect()
        {
            _neo4JDAO.Connect();    
        }
        
        public void WipeDB()
        {
            Connect();
            _neo4JDAO.WipeDB();
        }

        public void InsertUser(long tId, string username)
        {
            Connect();
            _neo4JDAO.InsertUser(tId, username);
        }

        public void InsertMovie(long mId, string movieName)
        {
            Connect();
            _neo4JDAO.InsertMovie(mId, movieName);
        }

        public List<string> GetFriendTowatchList(string friendUsername)
        {
            Connect();
            return _neo4JDAO.GetFriendTowatchList(friendUsername);
        }

        public double GetAverageRateByMovie(string movieName)
        {
            Connect();
            return _neo4JDAO.GetAverageRateByMovie(movieName);
        }

        public List<string> GetCommentsByMovie(string movieName)
        {
            Connect();
            return _neo4JDAO.GetCommentsByMovie(movieName);
        }
        
        public List<KeyValuePair<string, Neo4JDAO.RatedMovie>> GetSuggestedMovies(long tId, int depth)
        {
            Connect();
            // map movies with the class that allow to compute
            // to cumpute ratings at a specific depth value
            var moviesRatings = new Dictionary<string, Neo4JDAO.RatedMovie>();

            _neo4JDAO.GetSuggestedMovies(tId, depth, moviesRatings);

            foreach (var suggestedMovie in moviesRatings)
            {
                double numerator = 0;
                double denominator = 0;
                for (int i = 0; i < depth; ++i)
                {
                    // Divide rating sum by the total number of rates
                    double nbRating = suggestedMovie.Value.RatingsAtDepthLevel[depth - i - 1, 1];
                    if (nbRating != 0)
                    {
                        suggestedMovie.Value.RatingsAtDepthLevel[depth - i - 1, 0]
                            *= Math.Pow(2, depth - i - 1) / nbRating;
                        numerator += suggestedMovie.Value.RatingsAtDepthLevel[depth - i - 1, 0];
                        denominator += Math.Pow(2, depth - i - 1);
                    }
                }

                suggestedMovie.Value.Score = numerator / denominator;
            }
            var sortedMoviesByRating = moviesRatings.ToList();
            sortedMoviesByRating.Sort((pair1, pair2) => pair1.Value.Score.CompareTo(pair2.Value.Score));
            return sortedMoviesByRating;
        }

        public void AddFriend(string username, string friendUsername)
        {
            Connect();
            if (ensureUserExists(username) && ensureUserExists(friendUsername))
            {
                _neo4JDAO.UserIsFriendWith(username, friendUsername);
            }
             
        }

        public List<string> GetFriends(string username)
        {
            Connect();
            if (ensureUserExists(username))
            {
                return _neo4JDAO.GetFriends(username);
            }
            return new List<string>();
        }

        public void AddToWatch(string username, string movieName)
        {
            Connect();
            if (ensureUserExists(username) && ensureMovieExists(movieName))
            {
                _neo4JDAO.UserWillwatchMovie(username, movieName);
            }
        }
        public void AddRate(string username, string movieName, double rate)
        {
            Connect();
            if (ensureUserExists(username) && ensureMovieExists(movieName))
            {
                _neo4JDAO.UserRatesMovie(username, movieName, rate);
            }
            
        }
        public void AddComment(string username, string movieName, string comment)
        {
            Connect();
            if (ensureUserExists(username) && ensureMovieExists(movieName))
            {
                _neo4JDAO.UserCommentsMovie(username, movieName, comment);
            }
        }
        private bool ensureUserExists(string username)
        {
            Connect();
            if (!_neo4JDAO.UserExists(username))
            {
                _neo4JDAO.InsertUser(0, username);
            }
            return true;            
        }
        private bool ensureMovieExists(string movieName)
        {
            Connect();
            if (!_neo4JDAO.MovieExists(movieName))
            {
                _neo4JDAO.InsertMovie(0, movieName);
            }
            return true;
        }


        public void LoadInitialData()
        {

            _neo4JDAO.InsertUser(0, "Simmonde");
            _neo4JDAO.InsertUser(1, "Saumonlecitron");
            _neo4JDAO.InsertUser(2, "FredericKorradi");
            _neo4JDAO.UserIsFriendWith("Simmonde", "Saumonlecitron");
            _neo4JDAO.InsertMovie(0, "Star Wars: The Rise of Skywalker");
            _neo4JDAO.UserRatesMovie("Simmonde", "Star Wars: The Rise of Skywalker", 2);
            _neo4JDAO.UserRatesMovie("Saumonlecitron", "Star Wars: The Rise of Skywalker", 2);
        }
    }
}
