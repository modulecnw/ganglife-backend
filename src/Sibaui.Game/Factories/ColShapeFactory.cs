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
    internal class ColShapeFactory : ISingleton
    {
        public string Name => "ColShapeFactory";

        private readonly IServiceProvider _serviceProvider;

        public ColShapeFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            RAGE.Entities.Colshapes.CreateEntity = netHandle => Create(netHandle);
        }

        public SColShape Create(NetHandle netHandle)
        {
            try
            {
                var col = ActivatorUtilities.CreateInstance<SColShape>(_serviceProvider, netHandle);
                if (col is null)
                    Console.WriteLine("Unable to create colShape.");

                return col!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }
    }
}
