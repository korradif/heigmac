using MoviesGlobalResources;
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
        private static Dictionary<string, int> _filmsContext = new Dictionary<string, int>(); //([UserName, filmId])
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
                        if (_filmsContext.TryGetValue(message.From.Username, out int value))
                        {
                            _filmsContext[message.From.Username] = 2; //TODO: replace id by result from getmovie
                        }
                        else
                        {
                            _filmsContext.Add(message.From.Username, 1); //TODO: replace id by result from getmovie
                        }
                        //int id = MovieApi.Movie.GetMovie("name", arguments[1]);

                        string jsonMovie = _moviesGlobalResCtl.GetMoviesByName(arguments[1]);
                        Bot.SendTextMessageAsync(message.Chat.Id, jsonMovie.Substring(0, 10)); //displaying only the first 10 chars of the json. 
                                                                                               //MovieApi.Movie.GetMovie("name", arguments[1]);_mongoDBDAO
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
                    _filmsContext.TryGetValue(message.From.Username, out int movieId);
                    if (arguments.Count >= 2)
                    {
                        bool parsedSucessfully = Double.TryParse(arguments[1], out double rate);
                        if (parsedSucessfully)
                        {
                            Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + rate + " as a comment to " + movieId.ToString() + " from user " + message.From.Username);
                            //SocialAPI.AddRate(message.From.Username, movieId, rate);
                        }
                        else
                        {
                            Bot.SendTextMessageAsync(message.Chat.Id, "Please enter a valid rate");
                        }
                    }
                    break;
                case "/addComment":
                    _filmsContext.TryGetValue(message.From.Username, out int id);
                    string comment = "";
                    for (int i = 1; i < arguments.Count; ++i) comment += arguments[i];
                    Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + comment + " as a comment to " + id.ToString() + " from user " + message.From.Username);
                    //SocialAPI.AddComment(message.From.Username, movieId, comment);
                    break;
                case "/Name":
                    Bot.SendTextMessageAsync(message.Chat.Id, "test");
                    break;
                // send inline keyboard
                case "/getMovie":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                    await Task.Delay(500); // simulate longer running task

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
        }

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                $"Received2 {callbackQuery.Data}");
            await Bot.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"{callbackQuery.Data}");
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
