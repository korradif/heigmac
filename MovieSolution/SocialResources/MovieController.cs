﻿using System;
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
            _neo4JDAO.WipeDB();
        }

        public void InsertUser(long tId, string username)
        {
            _neo4JDAO.InsertUser(tId, username);
        }

        public void InsertMovie(long mId, string movieName)
        {
            _neo4JDAO.InsertMovie(mId, movieName);
        }

        public List<string> GetFriendTowatchMovies(string friendUsername)
        {
            throw new NotImplementedException();
            // todo : link with GetSuggestedMovies
        }

        public double GetAverageRateByMovie(string movieName)
        {
            throw new NotImplementedException();
        }

        public List<string> GetCommentsByMovie(string movieName)
        {
            throw new NotImplementedException();
        }
        
        public List<KeyValuePair<string, Neo4JDAO.RatedMovie>> GetSuggestedMovies(long tId, int depth)
        {
            // map movies with the class that allow to compute
            // to cumpute ratings at a specific depth value
            var moviesRatings = new Dictionary<string, Neo4JDAO.RatedMovie>();

           // _neo4JDAO.GetSuggestedMovies(tId, depth, moviesRatings);

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
            if (ensureUserExists(username) && ensureUserExists(friendUsername))
            {
                _neo4JDAO.UserIsFriendWith(username, friendUsername);
            }
             
        }

        public void AddToWatch(string username, string movieName)
        {
            if (ensureUserExists(username) && ensureMovieExists(movieName))
            {
                _neo4JDAO.UserWillwatchMovie(username, movieName);
            }
        }
        public void AddRate(string username, string movieName, double rate)
        {
            if (ensureUserExists(username) && ensureMovieExists(movieName))
            {
                _neo4JDAO.UserRatesMovie(username, movieName, rate);
            }
            
        }
        public void AddComment(string username, string movieName, string comment)
        {
            if (ensureUserExists(username) && ensureMovieExists(movieName))
            {
                _neo4JDAO.UserCommentsMovie(username, movieName, comment);
            }
        }
        private bool ensureUserExists(string username)
        {
            if (!_neo4JDAO.UserExists(username))
            {
                _neo4JDAO.InsertUser(0, username);
            }
            return true;            
        }
        private bool ensureMovieExists(string movieName)
        {
            if (!_neo4JDAO.MovieExists(movieName))
            {
                _neo4JDAO.InsertMovie(0, movieName);
            }
            return true;
        }

      
    }
}
