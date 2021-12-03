using GTANetworkAPI;
using Microsoft.Extensions.DependencyInjection;
using Sibaui.Core.Models;
using Sibaui.Game.Events;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Factories
{
    internal class MarkerFactory : ISingleton
    {
        public string Name => "MarkerFactory";

        private readonly IServiceProvider _serviceProvider;
        private readonly EventHub _eventHub;

        public MarkerFactory(IServiceProvider serviceProvider, EventHub eventHub)
        {
            _serviceProvider = serviceProvider;
            _eventHub = eventHub;

            RAGE.Entities.Markers.CreateEntity = netHandle => Create(netHandle);
            _eventHub.OnPlayerEnterColshape += OnPlayerEnterColshape;
        }

        private Task OnPlayerEnterColshape(SColShape colShape, SPlayer player)
        {
            var message = colShape.Message;
            if (message == null)
                return Task.CompletedTask;

            player.SendNotification(message.Text, message.Type, message.Title, message.Duration);

            return Task.CompletedTask;
        }

        public SMarker Create(NetHandle netHandle)
        {
            try
            {
                var marker = ActivatorUtilities.CreateInstance<SMarker>(_serviceProvider, netHandle);
                if (marker is null)
                    Console.WriteLine("Unable to create marker.");

                return marker!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }
    }
}
