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
    internal static class PlayerLoginExtensions
    {
        public static void Login(this SPlayer player)
        {
            NAPI.Task.Run(() =>
            {
                player.Transparency = 255;
                player.SetInvincible(false);

                foreach (var weapon in player.Weapons)
                    player.GiveWeapon((WeaponHash)weapon.WeaponHash, weapon.Ammo);

                player.LoadPlayer(player.Money);

                player.CloseInterface();
            });
        }

        public static void LoadPlayer(this SPlayer player, int Money) => player.TriggerEvent("PlayerLoaded", new PlayerLoadDataModel(Money).ToJson());
    }
}
