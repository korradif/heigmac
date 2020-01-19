using MoviesGlobalResources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialResources;
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
        private static MovieController _movieController = new MovieController();

        public static void Main()
        {
            _moviesGlobalResCtl.ClearCache();
            _movieController.WipeDB();
            _movieController.LoadInitialData();
            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            //            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            //            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
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
                case "/addFriend":
                    if (arguments.Count == 2)
                    {
                        Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + arguments[1].Substring(1) + " as a friend to " + message.From.Username);
                        _movieController.AddFriend(message.From.Username, arguments[1].Substring(1));
                    }
                    break;
                case "/getFriends":
                    //                    List<string> friends = _movieController.GetFriends(username);
                    break;
                case "/getMovie":
                    await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

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
                default:
                    switch (session.Step)
                    {
                        case Step.InputFilterValue:
                            session.FilterValue = arguments[0].Substring(1);
                            displayFilteredMovies(session);
                            break;
                        case Step.CommentMovie:
                            string comment2 = arguments[0].Substring(1);
                            for (int i = 1; i < arguments.Count; ++i) comment2 += " " + arguments[i];
                            Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + comment2 + " as a comment to " + session.SelectedMovie + " from user " + message.From.Username);
                            _movieController.AddComment(message.From.Username, session.SelectedMovie, comment2);
                            session.Step = Step.Default;
                            break;
                        case Step.RateMovie:
                            bool parsedSucessfully = Double.TryParse(arguments[0].Substring(1).Replace('.', ','), out double rate);
                            if (parsedSucessfully)
                            {
                                _movieController.AddRate(username, session.SelectedMovie, rate);
                                Bot.SendTextMessageAsync(message.Chat.Id, "Adding " + rate + " as a mark to " + session.SelectedMovie + " from user " + message.From.Username);
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
/author   - display authors name
/getMovie - get the Movie (various filters)
/addFriend [username] - add an user as a friend
/getFriends - display friends list
";

                            await Bot.SendTextMessageAsync(
                                message.Chat.Id,
                                usage,
                                replyMarkup: new ReplyKeyboardRemove());
                            break;
                    }
                    break;
            }
        }

        private static void displayFilteredMovies(UserSession session)
        {
            session.Step = Step.SelectMovie;
            string rawJsonMovie = "";
            switch (session.Filter)
            {
                case "Name":
                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByName(session.FilterValue);
                    break;
                case "Cast":
                    rawJsonMovie = _moviesGlobalResCtl.GetMoviesByFilter(TMDBDAO.Filter.CAST, session.FilterValue);
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
            displayJsonResultForChoiceAsync(rawJsonMovie, session, "original_title");
            session.Step = Step.SelectMovie;
        }
        private static async Task displayJsonResultForChoiceAsync(string rawJsonMovie, UserSession session, string fieldToDisplay)
        {
            JObject jsonMovie = JObject.Parse(rawJsonMovie);
            JArray results = (JArray)jsonMovie.SelectToken("results");

            await Bot.SendChatActionAsync(session.Message.Chat.Id, ChatAction.Typing);

            List<List<InlineKeyboardButton>> keyboardButtonsLines = new List<List<InlineKeyboardButton>>();
            for (int i = 0; i < results.Count; ++i)
            {
                List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
                for (int j = 0; j < 3; ++j, ++i)
                {
                    row.Add(InlineKeyboardButton.WithCallbackData((string)results[i].SelectToken(fieldToDisplay)));
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
                    case Step.ChooseGenre:
                        session.FilterValue = getGenreId(callbackQuery.Data).ToString();
                        displayFilteredMovies(session);
                        break;
                    case Step.ChooseFilter:
                        session.Filter = callbackQuery.Data;
                        if (session.Filter == "Genres")
                        {
                            getGenresAsync(session);
                            session.Step = Step.ChooseGenre;
                        }
                        else
                        {
                            session.Step = Step.InputFilterValue;
                        }
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
                                _movieController.AddToWatch(username, session.SelectedMovie);
                                break;
                            case "Comment":
                                session.Step = Step.CommentMovie;
                                break;
                            case "Rate":
                                session.Step = Step.RateMovie;
                                break;
                        }
                        break;


                }
            }
            await Bot.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"{callbackQuery.Data}");
        }

        private static int getGenreId(string data)
        {
            return _moviesGlobalResCtl.GetMovieGenreIdByName(data);
        }

        private static async Task getGenresAsync(UserSession session)
        {
            await Bot.SendChatActionAsync(session.Message.Chat.Id, ChatAction.Typing);
            var genres = _moviesGlobalResCtl.GetMovieGenres();

            List<List<InlineKeyboardButton>> keyboardButtonsLines = new List<List<InlineKeyboardButton>>();
            for (int i = 0; i < genres.Count;)
            {
                List<InlineKeyboardButton> row = new List<InlineKeyboardButton>();
                for (int j = 0; j < 3 && i < genres.Count; ++j, ++i)
                {
                    row.Add(genres.ElementAt(i).Key);
                }
                keyboardButtonsLines.Add(row);
            }
            var inlineKeyboard = new InlineKeyboardMarkup(keyboardButtonsLines);


            await Bot.SendTextMessageAsync(
                session.Message.Chat.Id,
                "Choose",
                replyMarkup: inlineKeyboard);
        }

        private static async Task getMovieActionChoiceAsync(UserSession session)
        {
            await Bot.SendChatActionAsync(session.Message.Chat.Id, ChatAction.Typing);

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

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}
