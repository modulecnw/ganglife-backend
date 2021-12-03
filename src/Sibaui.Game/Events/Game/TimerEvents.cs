using GTANetworkAPI;
using Sibaui.Core.Enumerations;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

// made by Dschahannam.
namespace Sibaui.Game.Events.Game
{
    public delegate Task EverySubscribedMinuteAsyncDelgate();
    public delegate Task EveryMinuteAsyncDelgate();

    public sealed class TimerEventHandler : Script
    {
        public static TimerEventHandler Instance { get; private set; }
        public System.Timers.Timer _minuteTimer;

        public TimerEventHandler()
        {
            _minuteTimer = new System.Timers.Timer(60 * 1000) { AutoReset = true };
            _minuteTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnEverySubscribedMinute);

            var timer = new System.Timers.Timer(60 * 1000) { AutoReset = true };
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnEveryMinute);
            timer.Start();

            Instance = this;
        }

        private void OnEverySubscribedMinute(object sender, ElapsedEventArgs e) => EventHub.EverySubscribedMinuteAsyncEventHandler.InvokeAsync(d => d());
        private void OnEveryMinute(object sender, ElapsedEventArgs e) => EventHub.EveryMinuteAsyncEventHandler.InvokeAsync(d => d());
    }
}
