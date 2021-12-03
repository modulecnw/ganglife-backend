using Sibaui.Game.Models;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Module.Gangwar.Model
{
    public class GangwarGameModel
    {
        public Database.Entities.Team AttackerTeam { get; set; }
        public Database.Entities.Team DefenderTeam { get; set; }

        public int AttackerPoints { get; set; }
        public int DefenderPoints { get; set; }

        public int AttackerFlags { get; set; }
        public int DefenderFlags { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.Now;
    }
}
