using GTANetworkAPI;
using Sibaui.Core.Enumerations;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Events.Game
{
    public delegate Task PlayerConnectAsyncDelegate(SPlayer player);
    public delegate Task PlayerKeyPressAsyncDelegate(SPlayer player, Keys key);
    public delegate Task PlayerDeathAsyncDelegate(SPlayer player, SPlayer killer, uint reason);
    public delegate Task PlayerEnterColshapeAsyncDelegate(SColShape colShape, SPlayer player);
    public delegate Task PlayerLeaveColshapeAsyncDelegate(SColShape colShape, SPlayer player);
    public delegate Task PlayerEnterVehicleAsyncDelegate(SPlayer player, SVehicle vehicle, sbyte seatID);

    public sealed class RageEventHandler : Script
    {
        [RemoteEvent("PlayerConnect")]
        private Task OnPlayerConnectAsync(SPlayer player) => EventHub.PlayerConnectAsyncEventHandler.InvokeAsync(d => d(player));

        [RemoteEvent("Press")]
        private Task OnPlayerKeyPress(SPlayer player, int key) => EventHub.PlayerKeyPressAsyncEventHandler.InvokeAsync(d => d(player, (Keys)key));

        [ServerEvent(Event.PlayerEnterColshape)]
        private void OnPlayerEnterColshape(SColShape colShape, SPlayer player)
        {
            if (!colShape.Players.ContainsKey(player.Id))
                colShape.Players.Add(player.Id, player);

            EventHub.PlayerEnterColshapeAsyncEventHandler.InvokeAsync(d => d(colShape, player));
        }

        [ServerEvent(Event.PlayerExitColshape)]
        private void OnPlayerLeaveColshape(SColShape colShape, SPlayer player)
        {
            if (colShape.Players.ContainsKey(player.Id))
                colShape.Players.Remove(player.Id);

            EventHub.PlayerLeaveColshapeAsyncEventHandler.InvokeAsync(d => d(colShape, player));
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        private void OnPlayerEnterVehicle(SPlayer player, SVehicle vehicle, sbyte seatID) => EventHub.PlayerEnterVehicleAsyncEventHandler.InvokeAsync(d => d(player, vehicle, seatID));

        [ServerEvent(Event.PlayerDeath)]
        private void OnPlayerDeath(SPlayer player, SPlayer killer, uint reason)
        {
            player.Spawn(player.Position);

            if (player.Injured) return;
            player.Injured = true;

            EventHub.PlayerDeathAsyncEventHandler.InvokeAsync(d => d(player, killer, reason));
        }
    }
}
