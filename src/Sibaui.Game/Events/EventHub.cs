using GTANetworkAPI;
using Microsoft.Extensions.DependencyInjection;
using Sibaui.Core.Events;
using Sibaui.Core.Models;
using Sibaui.Game.Events.Game;
using Sibaui.Game.Singletons;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Events
{
    public sealed class EventHub : ISingleton
    {
        public string Name => "EventHub";

        private readonly IServiceProvider _serviceProvider;

        public static AsyncEventHandler<EverySubscribedMinuteAsyncDelgate> EverySubscribedMinuteAsyncEventHandler;
        public static AsyncEventHandler<EveryMinuteAsyncDelgate> EveryMinuteAsyncEventHandler;

        public static AsyncEventHandler<PlayerConnectAsyncDelegate> PlayerConnectAsyncEventHandler;
        public static AsyncEventHandler<PlayerKeyPressAsyncDelegate> PlayerKeyPressAsyncEventHandler;
        public static AsyncEventHandler<PlayerDeathAsyncDelegate> PlayerDeathAsyncEventHandler;
        public static AsyncEventHandler<PlayerEnterColshapeAsyncDelegate> PlayerEnterColshapeAsyncEventHandler;
        public static AsyncEventHandler<PlayerLeaveColshapeAsyncDelegate> PlayerLeaveColshapeAsyncEventHandler;
        public static AsyncEventHandler<PlayerEnterVehicleAsyncDelegate> PlayerEnterVehicleAsyncEventHandler;

        public EventHub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            EverySubscribedMinuteAsyncEventHandler = new AsyncEventHandler<EverySubscribedMinuteAsyncDelgate>();
            EveryMinuteAsyncEventHandler = new AsyncEventHandler<EveryMinuteAsyncDelgate>();

            PlayerConnectAsyncEventHandler = new AsyncEventHandler<PlayerConnectAsyncDelegate>();
            PlayerKeyPressAsyncEventHandler = new AsyncEventHandler<PlayerKeyPressAsyncDelegate>();
            PlayerDeathAsyncEventHandler = new AsyncEventHandler<PlayerDeathAsyncDelegate>();
            PlayerEnterColshapeAsyncEventHandler = new AsyncEventHandler<PlayerEnterColshapeAsyncDelegate>();
            PlayerLeaveColshapeAsyncEventHandler = new AsyncEventHandler<PlayerLeaveColshapeAsyncDelegate>();
            PlayerEnterVehicleAsyncEventHandler = new AsyncEventHandler<PlayerEnterVehicleAsyncDelegate>();
        }

        public event EverySubscribedMinuteAsyncDelgate OnEverySubscribedMinute
        {
            add
            {
                var timer = TimerEventHandler.Instance._minuteTimer;
                timer.Start();

                EverySubscribedMinuteAsyncEventHandler.Add(value);
            }
            remove
            {
                var timer = TimerEventHandler.Instance._minuteTimer;
                timer.Stop();

                EverySubscribedMinuteAsyncEventHandler.Remove(value);
            }
        }

        public event EveryMinuteAsyncDelgate OnEveryMinute
        {
            add => EveryMinuteAsyncEventHandler.Add(value);
            remove => EveryMinuteAsyncEventHandler.Remove(value);
        }

        public event PlayerConnectAsyncDelegate OnPlayerConnect
        {
            add => PlayerConnectAsyncEventHandler.Add(value);
            remove => PlayerConnectAsyncEventHandler.Remove(value);
        }

        public event PlayerKeyPressAsyncDelegate OnKeyPress
        {
            add => PlayerKeyPressAsyncEventHandler.Add(value);
            remove => PlayerKeyPressAsyncEventHandler.Remove(value);
        }

        public event PlayerEnterColshapeAsyncDelegate OnPlayerEnterColshape
        {
            add => PlayerEnterColshapeAsyncEventHandler.Add(value);
            remove => PlayerEnterColshapeAsyncEventHandler.Remove(value);
        }

        public event PlayerDeathAsyncDelegate OnPlayerDeath
        {
            add => PlayerDeathAsyncEventHandler.Add(value);
            remove => PlayerDeathAsyncEventHandler.Remove(value);
        }

        public event PlayerLeaveColshapeAsyncDelegate OnPlayerLeaveColshape
        {
            add => PlayerLeaveColshapeAsyncEventHandler.Add(value);
            remove => PlayerLeaveColshapeAsyncEventHandler.Remove(value);
        }

        public event PlayerEnterVehicleAsyncDelegate OnPlayerEnterVehicle
        {
            add => PlayerEnterVehicleAsyncEventHandler.Add(value);
            remove => PlayerEnterVehicleAsyncEventHandler.Remove(value);
        }
    }
}
