using System;
using System.Collections.Generic;
using System.Text;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Message
{
    public class Message
    {
        public Message(string text, int duration, NotificationType type)
        {
            Text = text;
            Title = String.Empty;
            Duration = duration;
            Type = type;
        }

        public Message(string text, string title, int duration, NotificationType type)
        {
            Text = text;
            Title = title;
            Duration = duration;
            Type = type;
        }

        public string Text { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public NotificationType Type { get; set; }
    }
}
