using System;
using SocialResources;

namespace MovieSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MovieController ctr = new MovieController();
            ctr.Connect();
            
            // test
            ctr.WipeDB();
            
            ctr.InsertUser(0, "simmonde");
            ctr.InsertUser(1, "saumonLeCitron");
            ctr.AddFriend("simmondxe", "saumonLeCitron");
            
            /*
            ctr.AddFriend(1, "saumonlecitron");
            ctr.AddFriend(2, "simmonde");
            ctr.AddFriend(3, "fred");
            ctr.AddFriend(4, "jean-pierre");
            ctr.AddFriend(5, "jean-claude");
            ctr.InsertUser(6, "jean-paul");
            ctr.InsertUser(7, "arnold");
            
            ctr.AddFriend(1, 2);
            ctr.UserIsFriendWith(1, 3);
            ctr.UserIsFriendWith(1, 4);
            ctr.UserIsFriendWith(1, 5);
            ctr.UserIsFriendWith(1, 6);
            ctr.UserIsFriendWith(2, 7);
            
            ctr.InsertMovie(1, "The Adams Family");
            
            ctr.UserRatesMovie(2, 1, 5.0);
            ctr.UserRatesMovie(3, 1, 5.0);
            ctr.UserRatesMovie(4, 1, 5.0);
            ctr.UserRatesMovie(7, 1, 1);
            
            ctr.GetSuggestedMovies(1, 2);
            
            ctr.InsertMovie(1, "taf");
            
            ctr.UserRatesMovie(1, 1, "Nice !");
            ctr.UserRatesMovie(3, 1, "Bullshit !");
            ctr.UserIsFriendWith(1, 2);
            ctr.UserIsFriendWith(2, 1);
            ctr.UserIsFriendWith(1, 3);
            
            Console.WriteLine(ctr.GetUsernameById(1));
            */
        }
    }
}
