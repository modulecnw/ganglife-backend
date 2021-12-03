using Newtonsoft.Json;
using Sibaui.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Controller.Window
{
    public class ConfirmDataModel
    {
        public ConfirmDataModel(string text, string firstText, string secondText, string acceptEvent, string declineEvent)
        {
            Text = text;
            FirstText = firstText;
            SecondText = secondText;
            AcceptEvent = acceptEvent;
            DeclineEvent = declineEvent;
        }

        public ConfirmDataModel(string text, string firstText, string secondText, string acceptEvent)
        {
            Text = text;
            FirstText = firstText;
            SecondText = secondText;
            AcceptEvent = acceptEvent;
            DeclineEvent = String.Empty;
        }

        [JsonProperty("t")]
        public string Text { get; set; }

        [JsonProperty("ft")]
        public string FirstText { get; set; }

        [JsonProperty("st")]
        public string SecondText { get; set; }

        [JsonProperty("fe")]
        public string AcceptEvent { get; set; }

        [JsonProperty("se")]
        public string DeclineEvent { get; set; }
    }
}
