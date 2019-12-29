using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace TelegramConsumer
{
    public enum Step
    {
        Default,
        ChooseFilter,
        InputFilterValue,
        SelectMovie,
        ChooseMovieAction,
        CommentMovie,
        RateMovie,
        ChooseGenre
    }
    public class UserSession
    {
        private string _username;

        public UserSession(string username)
        {
            _username = username;
        }

        public Step Step { get; internal set; } = Step.Default;
        public string Filter { get; internal set; }
        public string FilterValue { get; internal set; }
        public Message Message { get; internal set; }
        public string SelectedMovie { get; internal set; }
    }
}
