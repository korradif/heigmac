using System;
using SocialResources;

namespace MovieSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Neo4JDAO _neo4Jdao = new Neo4JDAO();
            _neo4Jdao.Connect();
            
            // test
            _neo4Jdao.WipeDB();
            
            _neo4Jdao.InsertUser(1, "saumonlecitron");
            _neo4Jdao.InsertUser(2, "simmonde");
            _neo4Jdao.InsertUser(3, "fred");
            _neo4Jdao.InsertUser(4, "jean-pierre");
            _neo4Jdao.InsertUser(5, "jean-claude");
            _neo4Jdao.InsertUser(6, "jean-paul");
            _neo4Jdao.InsertUser(7, "arnold");
            
            _neo4Jdao.UserIsFriendWith(1, 2);
            _neo4Jdao.UserIsFriendWith(1, 3);
            _neo4Jdao.UserIsFriendWith(1, 4);
            _neo4Jdao.UserIsFriendWith(1, 5);
            _neo4Jdao.UserIsFriendWith(1, 6);
            _neo4Jdao.UserIsFriendWith(2, 7);
            
            _neo4Jdao.InsertMovie(1, "The Adams Family");
            
            _neo4Jdao.UserRatesMovie(2, 1, 5.0);
            _neo4Jdao.UserRatesMovie(3, 1, 5.0);
            _neo4Jdao.UserRatesMovie(4, 1, 5.0);
            _neo4Jdao.UserRatesMovie(7, 1, 1);
            
            _neo4Jdao.GetSuggestedMovies(1, 2);
            
            _neo4Jdao.InsertMovie(1, "taf");
            
            _neo4Jdao.UserRatesMovie(1, 1, "Nice !");
            _neo4Jdao.UserRatesMovie(3, 1, "Bullshit !");
            _neo4Jdao.UserIsFriendWith(1, 2);
            _neo4Jdao.UserIsFriendWith(2, 1);
            _neo4Jdao.UserIsFriendWith(1, 3);
            
            Console.WriteLine(_neo4Jdao.GetUsernameById(1));
        }
    }
}
