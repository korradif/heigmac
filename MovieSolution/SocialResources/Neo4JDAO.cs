﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Neo4j.Driver.V1;
using Neo4j.Driver;

namespace SocialResources
{
    public class Neo4JDAO : IDisposable
    {
        private IDriver _driver;

        public void Connect()
        {
            _driver = GraphDatabase.Driver("bolt://localhost:7687", 
                AuthTokens.Basic("neo4j", "test_pwd"));
        }

        public void WipeDB()
        {
            using (var session = _driver.Session())
            {
                session.Run("MATCH(n) DETACH DELETE n");
            }
        }
        
        public void InsertUser(long tId, string username)
        {
            using (var session = _driver.Session())
            {
                session.Run("CREATE ( :User {tId:" + tId + ", username: '" + username + "'})");
            }
        }

        public void InsertMovie(long mId, string originalTitle)
        {
            using (var session = _driver.Session())
            {
                session.Run("CREATE ( :Movie {mId:" + mId + ", originalTitle : '" + originalTitle + "'})");
            }
        }

        public void UserRatesMovie(long tId, long mId, string description)
        {
            using (var session = _driver.Session())
            {
                session.Run("MATCH (user:User {username : '" + GetUsernameById(tId) 
                            + "'}), (movie:Movie {originalTitle : '" 
                            + GetOriginaltitleById(mId) + "'}) MERGE (user)-[:COMMENTS{description : '" 
                            + description + "'}]->(movie)");
            }
        }

        public void UserRatesMovie(long tId, long mId, double rating)
        {
            using (var session = _driver.Session())
            {
                session.Run("MATCH (user:User {username : '" + GetUsernameById(tId) 
                             + "'}), (movie:Movie {originalTitle : '" 
                             + GetOriginaltitleById(mId) + "'}) MERGE (user)-[:RATES{rating : " 
                             + rating + "}]->(movie)");
            }
        }

        internal bool UserExists(string username)
        {
            throw new NotImplementedException();
        }
        internal bool MovieExists(string movieName)
        {
            throw new NotImplementedException();
        }

        internal void UserRatesMovie(string username, string movieName, double rate)
        {
            throw new NotImplementedException();
        }

        internal void UserIsFriendWith(string username, string friendUsername)
        {
            throw new NotImplementedException();
        }

        public void UserIsFriendWith(long tId1, long tId2)
        {
            using (var session = _driver.Session())
            {
                session.Run("MATCH (user1:User {username : '" + GetUsernameById(tId1)
                            + "'}), (user2:User {username : '"
                            + GetUsernameById(tId2) + "'}) MERGE (user1)-[:IS_FRIEND]->(user2)");
            }
        }

  

        public string GetUsernameById(long tId)
        {
            string username = "";
            using (var session = _driver.Session())
            {
                var result = session.Run("MATCH (u:User) WHERE u.tId = " + tId + " RETURN u.username");
                foreach (var record in result)
                {
                    username = record["u.username"].ToString();
                }
            }
            
            return username;
        }

        public string GetOriginaltitleById(long mId)
        {
            string originalTitle = "";
            using (var session = _driver.Session())
            {
                var result = session.Run("MATCH (m:Movie) WHERE m.mId = " + mId + " RETURN m.originalTitle");
                foreach (var record in result)
                {
                    originalTitle = record["m.originalTitle"].ToString();
                }

                return originalTitle;
            }
        }

        public List<string> GetFriends(int tId)
        {
            List<string> ret = new List<string>();
            using (var session = _driver.Session())
            {
                var friends = session.Run("MATCH (a)-[:IS_FRIEND]->(b) WHERE a.tId = "
                                         + tId + " RETURN b.tId");

                foreach (var friend in friends)
                {
                    ret.Add(GetUsernameById((int)friend["b.tId"]));
                }
            }

            return ret;
        }
        
      
        
        internal void GetSuggestedMovies(long tId, int depth, Dictionary<string, RatedMovie> moviesRatings)
        {
            if (depth > 0)
            {
                using (var session = _driver.Session())
                {
                    // look for all neighbor friends
                    var friends = session.Run("MATCH (a)-[:IS_FRIEND]->(b) WHERE a.tId = "
                                             + tId + " RETURN b.tId");

                    foreach (var friend in friends)
                    {
                        // complete the map movie - rating
                        var movies = session.Run("MATCH (u)-[r:RATES]->(m) WHERE u.tId = "
                                                + (long)friend["b.tId"]
                                                + " RETURN m.originalTitle, r.rating");

                        GetSuggestedMovies((long)friend["b.tId"], depth - 1, moviesRatings);

                        if (movies != null)
                        {
                            foreach (var movie in movies)
                            {
                                if (!moviesRatings.ContainsKey((string) movie["m.originalTitle"]))
                                {
                                    moviesRatings.Add((string) movie["m.originalTitle"], new RatedMovie(depth));
                                }

                                // Add the rating to the sum
                                moviesRatings[(string) movie["m.originalTitle"]].RatingsAtDepthLevel[depth - 1, 0] 
                                        += Convert.ToDouble(movie["r.rating"]);

                                // increment nbRating at depth level for the movie
                                moviesRatings[(string)movie["m.originalTitle"]].RatingsAtDepthLevel[depth - 1, 1] += 1;
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }

        public class RatedMovie
        {
            public int Depth { get; }
            public double[,] RatingsAtDepthLevel { get; }
            public double Score { get; set; }

            public RatedMovie(int depth)
            {
                Depth = depth;
                RatingsAtDepthLevel = new double[Depth, 2];
            }
        }
    }
}
