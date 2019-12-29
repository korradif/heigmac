using MoviesGlobalResources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramConsumer
{
    public static class Program
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("1021492488:AAHzn9Sw4g8Ntyh8p7hr6GWA40nb0639sVU");
        private static readonly MoviesGlobalResourcesController _moviesGlobalResCtl = new MoviesGlobalResourcesController();
        private static Dictionary<string, string> _moviesContext = new Dictionary<string, string>(); //([UserName, movieId])
        private static Dictionary<string, UserSession> _userSessions = new Dictionary<string, UserSession>();
        public static void Main()
        {
            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {

            var message = messageEventArgs.Message;
            string username = message.From.Username;
            UserSession session;
            if (!_userSessions.TryGetValue(username, out session))
            {
                session = new UserSession(username);
                _userSessions.Add(username, session);
            }
            session.Message = message;
            if (message == null || message.Type != MessageType.Text) return;
            var arguments = message.Text.Split(' ').ToList();
            switch (arguments.First())
            {
                case "/author":
                    Bot.SendTextMessageAsync(message.Chat.Id, "Frédéric Korradi, Simon Flückiger, Simon Jobin");
                    break;
                case "/repo":
                    Bot.SendTextMessageAsync(message.Chat.Id, "https://github.com/korradif/heigmac");
                    break;
                case "/getMovieByName":
                    if (arguments.Count >= 2)
                    {

                        //int id = MovieApi.Movie.GetMovie("name", arguments[1]);

                        string rawJsonMovie = _moviesGlobalResCtl.GetMoviesByName(arguments[1]);
                        JObject jsonMovie = JObject.Parse(rawJsonMovie);
                        JArray results = (JArray)jsonMovie.SelectToken("results");
                        foreach (JToken result in results)
                        {
                            string name = (string)result.SelectToken("original_title");
                        }

                        await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                        //     await Task.Delay(500); // simulate longer running task

                        List<List<InlineKeyboardButton>> keyboardButtonsLines = new List<List<InlineKeyboardButton>>();
                        for (int i = 0; i < results.Count; ++i)
                        {
                            List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
                            for (int j = 0; j < 3; ++j, ++i)
                            {
                                row.Add(InlineKeyboardButton.WithCallbackData((string)results[i].SelectToken("original_title")));
                            }
                            keyboardButtonsLines.Add(row);
                        }
                        var inlineKeyboard1 = new InlineKeyboardMarkup(keyboardButtonsLines);

                        /*var inlineKeyboard1 = new InlineKeyboardMarkup(new[]
                        {
                            new [] // first row
                            {
                                InlineKeyboardButton.WithCallbackData("Name"),
                                InlineKeyboardButton.WithCallbackData("Cast"),
                                InlineKeyboardButton.WithCallbackData("Crew"),
                            },
                            new [] // second row
                            {
                                InlineKeyboardButton.WithCallbackData("Genres"),
                                InlineKeyboardButton.WithCallbackData("Year"),
                                InlineKeyboardButton.WithCallbackData("Language"),
                            }
                        });*/

                        await Bot.SendTextMessageAsync(
                            message.Chat.Id,
                            "Choose",
                            replyMarkup: inlineKeyboard1);
                    }
                    break;
                case "/getMovieByCast":
                    if (arguments.Count >= 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat.Id, _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CAST, arguments[1]));
                        //MovieApi.Movie.GetMovie("cast", arguments[1]);
                    }
                    break;
                case "/getMovieByCrew":
                    if (arguments.Count >= 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat.Id, _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CREW, arguments[1]));
                        //MovieApi.Movie.GetMovie("crew", arguments[1]);
                    }
                    break;
                case "/getMovieByGenres":
                    if (arguments.Count >= 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat.Id, _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.GENRES, arguments[1]));
                        //MovieApi.Movie.GetMovie("genres", arguments[1]);
                    }
                    break;
                case "/getMovieByYear":
                    if (arguments.Count >= 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat.Id, _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.YEAR, arguments[1]));
                        //MovieApi.Movie.GetMovie("year", arguments[1]);
                    }
                    break;
                case "/getMovieByLanguage":
                    if (arguments.Count >= 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat.Id, _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.LANGUAGE, arguments[1]));
                        //MovieApi.Movie.GetMovie("year", arguments[1]);
                    }
                    break;
                case "/addFriend":
                    if (arguments.Count == 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + arguments[1].Substring(1) + " as a friend to " + message.From.Username);
                        //SocialAPI.AddFriend(message.From.Username, arguments[1].Substring(1));
                    }
                    break;
                case "/rate":
                    _moviesContext.TryGetValue(message.From.Username, out string movieName);
                    if (arguments.Count >= 2)
                    {
                        bool parsedSucessfully = Double.TryParse(arguments[1].Replace('.', ','), out double rate);
                        if (parsedSucessfully)
                        {
                            Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + rate + " as a mark to " + movieName + " from user " + message.From.Username);
                            //SocialAPI.AddRate(message.From.Username, movieId, rate);
                        }
                        else
                        {
                            Bot.SendTextMessageAsync(message.Chat.Id, "Please enter a valid rate");
                        }
                    }
                    break;
                case "/addComment":
                    _moviesContext.TryGetValue(message.From.Username, out string movieName2);
                    string comment = "";
                    for (int i = 1; i < arguments.Count; ++i) comment += arguments[i];
                    Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + comment + " as a comment to " + movieName2 + " from user " + message.From.Username);
                    //SocialAPI.AddComment(message.From.Username, movieId, comment);
                    break;
                case "/Name":
                    Bot.SendTextMessageAsync(message.Chat.Id, message.Chat.Username);
                    break;
                // send inline keyboard
                case "/getMovie":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);


                    await Task.Delay(500); // simulate longer running task
                    session.Step = Step.ChooseFilter;
                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("Name"),
                            InlineKeyboardButton.WithCallbackData("Cast"),
                            InlineKeyboardButton.WithCallbackData("Crew"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("Genres"),
                            InlineKeyboardButton.WithCallbackData("Year"),
                            InlineKeyboardButton.WithCallbackData("Language"),
                        }
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: inlineKeyboard);
                    break;

                // send custom keyboard
                case "/keyboard":
                    ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "1.1", "1.2" },
                        new[] { "2.1", "2.2" },
                    };

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: ReplyKeyboard);

                    break;

                // send a photo
                case "/photo":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string file = @"Files/tux.png";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    /*using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await Bot.SendPhotoAsync(
                            message.Chat.Id,
                            fileStream,
                            "Nice Picture");
                    }*/
                    break;

                // request location or contact
                case "/request":
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestLocation("Location"),
                        KeyboardButton.WithRequestContact("Contact"),
                    });

                    await Bot.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: RequestReplyKeyboard);
                    break;

                default:
                    switch (session.Step)
                    {
                        case Step.InputFilterValue:
                            session.FilterValue = arguments[0].Substring(1);
                            session.Step = Step.SelectMovie;
                            string rawJsonMovie="";
                            switch (session.Filter)
                            {
                                case "Name":
                                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByName(session.FilterValue);
                                    break;
                                case "Cast":
                                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CAST,session.FilterValue);
                                    break;
                                case "Crew":
                                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CREW, session.FilterValue);
                                    break;
                                case "Genres":
                                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.GENRES, session.FilterValue);
                                    break;
                                case "Year":
                                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.YEAR, session.FilterValue);
                                    break;
                                case "Language":
                                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.LANGUAGE, session.FilterValue);
                                    break;
                            }
                            displayMovieResultAsync(rawJsonMovie, session);
                            session.Step = Step.SelectMovie;
                            break;
                        case Step.CommentMovie:
                            string comment2 = arguments[0].Substring(1);
                            for (int i = 1; i < arguments.Count; ++i) comment2 += " " + arguments[i];
                            Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + comment2 + " as a comment to " + session.SelectedMovie + " from user " + message.From.Username);
                            //SocialAPI.AddComment(message.From.Username, movieId, comment);
                            session.Step = Step.Default;
                            break;
                        case Step.RateMovie:
                            bool parsedSucessfully = Double.TryParse(arguments[0].Substring(1).Replace('.', ','), out double rate);
                            if (parsedSucessfully)
                            {
                                Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + rate + " as a mark to " + session.SelectedMovie + " from user " + message.From.Username);
                                //SocialAPI.AddRate(message.From.Username, movieId, rate);
                                session.Step = Step.Default;
                            }
                            else
                            {
                                Bot.SendTextMessageAsync(message.Chat.Id, "Please enter a valid rate");
                            }
                            break;
                        default:
                            const string usage = @"
Usage:
/author   - display author name
/getMovieByName - get the Movie 
/repo     - display the link to the repo
/inline   - send inline keyboard
/keyboard - send custom keyboard
/photo    - send a photo
/request  - request location or contact";

                            await Bot.SendTextMessageAsync(
                                message.Chat.Id,
                                usage,
                                replyMarkup: new ReplyKeyboardRemove());
                            break;
                    }
                    break;

            }
        }

        private static async Task getMovieByNameAsync(UserSession session)
        {
             
        }

        private static async Task displayMovieResultAsync(string rawJsonMovie,UserSession session)
        {
            JObject jsonMovie = JObject.Parse(rawJsonMovie);
            JArray results = (JArray)jsonMovie.SelectToken("results");
            foreach (JToken result in results)
            {
                string name = (string)result.SelectToken("original_title");
            }

            await Bot.SendChatActionAsync(session.Message.Chat.Id, ChatAction.Typing);

            //     await Task.Delay(500); // simulate longer running task

            List<List<InlineKeyboardButton>> keyboardButtonsLines = new List<List<InlineKeyboardButton>>();
            for (int i = 0; i < results.Count; ++i)
            {
                List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
                for (int j = 0; j < 3; ++j, ++i)
                {
                    row.Add(InlineKeyboardButton.WithCallbackData((string)results[i].SelectToken("original_title")));
                }
                keyboardButtonsLines.Add(row);
            }
            var inlineKeyboard1 = new InlineKeyboardMarkup(keyboardButtonsLines);


            await Bot.SendTextMessageAsync(
                session.Message.Chat.Id,
                "Choose",
                replyMarkup: inlineKeyboard1);

        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Selected {callbackQuery.Data}");
            string username = callbackQuery.From.Username;
            UserSession session;
            if (_userSessions.TryGetValue(username, out session))
            {
                switch (session.Step)
                {
                    case Step.ChooseFilter:
                        session.Filter = callbackQuery.Data;
                        session.Step = Step.InputFilterValue;
                        break;
                    case Step.SelectMovie:
                        session.SelectedMovie = callbackQuery.Data;
                        session.Step = Step.ChooseMovieAction;
                        getMovieActionChoiceAsync(session);
                        break;
                    case Step.ChooseMovieAction:
                        switch (callbackQuery.Data)
                        {
                            case "Add to WatchList":
                                //SocialAPI.AddToWatchList(message.From.Username, movieId);
                                break;
                            case "Comment":
                                //Comment
                                session.Step = Step.CommentMovie;
                                break;
                            case "Rate":
                                //rate
                                session.Step = Step.RateMovie;
                                break;
                        }
                        break;


                }
            }
            else
            {
                if (_moviesContext.TryGetValue(username, out string value))
                {
                    _moviesContext[username] = callbackQuery.Data; //TODO: replace id by result from getmovie
                }
                else
                {
                    _moviesContext.Add(username, callbackQuery.Data); //TODO: replace id by result from getmovie
                }
            }
            await Bot.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"{callbackQuery.Data}");
        }

        private static async Task getMovieActionChoiceAsync(UserSession session)
        {
            //AddToWatchList
            //Comment
            //Rate
            await Bot.SendChatActionAsync(session.Message.Chat.Id, ChatAction.Typing);


            await Task.Delay(500); // simulate longer running task
            session.Step = Step.ChooseMovieAction;
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                    new [] // first row
                    {
                        InlineKeyboardButton.WithCallbackData("Add to WatchList"),
                        InlineKeyboardButton.WithCallbackData("Comment"),
                        InlineKeyboardButton.WithCallbackData("Rate")
                    }
                });

            await Bot.SendTextMessageAsync(
                session.Message.Chat.Id,
                "Choose",
                replyMarkup: inlineKeyboard);
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                new InlineQueryResultLocation(
                    id: "1",
                    latitude: 40.7058316f,
                    longitude: -74.2581888f,
                    title: "New York")   // displayed result
                    {
                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 40.7058316f,
                            longitude: -74.2581888f)    // message if result is selected
                    },

                new InlineQueryResultLocation(
                    id: "2",
                    latitude: 13.1449577f,
                    longitude: 52.507629f,
                    title: "Berlin") // displayed result
                    {
                        InputMessageContent = new InputLocationMessageContent(
                            latitude: 13.1449577f,
                            longitude: 52.507629f)   // message if result is selected
                    }
            };

            await Bot.AnswerInlineQueryAsync(
                inlineQueryEventArgs.InlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0);
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}
