using GTANetworkAPI;
using Sibaui.Core.Extensions;
using Sibaui.Database.Entities;
using Sibaui.Game.Client.Data;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Module.Login.Extensions
{
    internal static class PlayerInjuryExtensions
    {
        public static void ApplyInjury(this SPlayer player, int InjuryTime)
        {
            NAPI.Task.Run(() =>
            {
                player.RemoveAllWeapons();

                player.TriggerEvent("SetInjured", true);
                player.InjuryTimeLeft = InjuryTime;

                player.TriggerEvent("StartEffect", "DeathFailMPIn", InjuryTime * 1000, true);
                player.PlayAnimation("missarmenian2", "corpse_search_exit_ped", 1);
            });
        }
    }
}
