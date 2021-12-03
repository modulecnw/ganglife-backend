using GTANetworkAPI;
using Sibaui.Core.Extensions;
using Sibaui.Database.Context;
using Sibaui.Database.Entities;
using Sibaui.Game.Module.Inventory.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Factories.Entities
{
    public enum DimensionTypes
    {
        WORLD,
        HOUSE,
        CAMPER,
        STORAGEROOM,
        PAINTBALL,
        GANGWAR
    }

    public class SPlayer : GTANetworkAPI.Player
    {
        public new int Id { get; set; }
        public int Money { get; set; }
        public DimensionTypes DimensionType { get; set; }
        public SInventory Inventory { get; set; }
        public LocalInventoryData LocalInventoryData { get; set; }
        public PlayerCharacter Character { get; set; }
        public PlayerTeamInfo TeamInfo { get; set; }
        public new ICollection<PlayerWeapon> Weapons { get; set; }
        public bool Injured { get; set; }
        public int InjuryTimeLeft { get; set; }
        public CancellationTokenSource? CancellationToken { get; set; }

        public SPlayer(NetHandle handle) : base(handle)
        {
            Id = -1;
            Money = 0;
            TeamInfo = null;
            DimensionType = DimensionTypes.WORLD;
            Inventory = null;
            Injured = false;
            InjuryTimeLeft = 0;
            LocalInventoryData = null;
            CancellationToken = null;
        }

        public void Spawn(Vector3 pos) => NAPI.Task.Run(() => NAPI.Player.SpawnPlayer(this, pos));
        public void CloseInterface() => NAPI.Task.Run(() => TriggerEvent("CloseIF"));
        public void HidePlayerHud(bool state) => NAPI.Task.Run(() => TriggerEvent("HidePlayerHud", state));
        public void SetInvincible(bool state) => NAPI.Task.Run(() => TriggerEvent("SetInvincible", state));
        public void UpdateView(string eventName, string arg) => NAPI.Task.Run(() => TriggerEvent("UpdateView", eventName, arg));

        public enum NotificationType
        {
            INFO,
            SUCCESS,
            ERROR,
            OOC,
        }

        private String GetNotificationString(NotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationType.INFO => "blue",
                NotificationType.SUCCESS => "green",
                NotificationType.ERROR => "red",
                NotificationType.OOC => "orange",
                _ => "",

            };
        }

        public void SendNotification(String message, NotificationType notificationType, string title = "", int duration = 5000)
        {
            NAPI.Task.Run(() => TriggerEvent("UpdateView", "AddNotify", message, GetNotificationString(notificationType), title, duration));
        }

        public void SendNotification(String message, string color, string title = "", int duration = 5000)
        {
            NAPI.Task.Run(() => TriggerEvent("UpdateView", "AddNotify", message, color, title, duration));
        }

        public async Task PlayProgressbar(TimeSpan timeSpan, Func<Task> successTask, Func<Task> failedTask)
        {
            CancellationToken = new CancellationTokenSource();
            TriggerEventSafe("UpdateView", "Bar", timeSpan.Milliseconds);

            bool delayCanceled = await Task.Delay(timeSpan.Milliseconds, CancellationToken.Token).ContinueWith(task => task.IsCanceled);
            if (delayCanceled) await failedTask();
            else await successTask();

            Console.WriteLine($"Progressbar canceled: {delayCanceled}");
        }

        public void StopProgressbar()
        {
            if (CancellationToken == null) return;

            CancellationToken.Cancel();
            CancellationToken = null;

            TriggerEventSafe("UpdateView", "Bar", 0);
        }


        public async Task PlayAnimation(string animDict, string animName, int flag, int duration)
        {
            if (await GetIsInVehicle()) return;

            NAPI.Task.Run(() => PlayAnimation(animDict, animName, flag));
            await Task.Delay(duration);
            NAPI.Task.Run(() => StopAnimation());
        }

        public async Task<string> GetName() => await NAPI.Task.RunReturn(() => Name);
        public async Task<string> GetSocialClubName() => await NAPI.Task.RunReturn(() => SocialClubName);
        public async Task<string> GetSerial() => await NAPI.Task.RunReturn(() => Serial);
        public async Task<Vector3> GetPosition() => await NAPI.Task.RunReturn(() => Position);
        public async Task<bool> GetIsInVehicle() => await NAPI.Task.RunReturn(() => IsInVehicle);
        public async Task SetPosition(Vector3 position) => await NAPI.Task.RunReturn(() => Position = position);
        public async Task SetDimension(uint dimension) => await NAPI.Task.RunReturn(() => Dimension = dimension);


        public void TriggerEventSafe(string eventName, params object[] args)
        {
            NAPI.Task.Run(async () =>
            {
                await NAPI.Task.WaitForMainThread(0);
                TriggerEvent(eventName, args);
            });
        }
    }
}
