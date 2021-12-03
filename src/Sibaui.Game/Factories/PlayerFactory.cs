using GTANetworkAPI;
using Microsoft.Extensions.DependencyInjection;
using Sibaui.Core.Models;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Factories
{
    internal class PlayerFactory : ISingleton
    {
        public string Name => "PlayerFactory";

        private readonly IServiceProvider _serviceProvider;

        public PlayerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RAGE.Entities.Players.CreateEntity = netHandle => Create(netHandle);
        }

        public SPlayer Create(NetHandle netHandle)
        {
            try
            {
                var player = ActivatorUtilities.CreateInstance<SPlayer>(_serviceProvider, netHandle);
                if (player is null)
                    Console.WriteLine("Unable to create player.");

                return player!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }
    }
}
