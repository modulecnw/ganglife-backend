using Sibaui.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Controller.Window
{
    public class InputDataModel
    {
        public string E { get; set; }
        public string T { get; set; }

        public InputDataModel(string EventName, string Text)
        {
            E = EventName;
            T = Text;
        }
    }
}
