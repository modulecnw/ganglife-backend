using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data
{
    public class PlayerLoadDataModel
    {
        public int M { get; set; }

        public PlayerLoadDataModel(int Money)
        {
            M = Money;
        }
    }
}
