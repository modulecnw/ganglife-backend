using System;
using System.Diagnostics;
using System.Threading;
using GTANetworkAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sibaui.Core;
using Sibaui.Core.Models;
using Sibaui.Database;
using Sibaui.Game.Events;
using Sibaui.Game.Factories;
using Sibaui.Game.Module.Gangwar;
using Sibaui.Game.Singletons;

// made by Dschahannam.
namespace Sibaui.Game
{
    /*
     * Troll Project, made in 1-2 weeks.
     * Dont judge the code.
     */

    public class Resource : Script
    {
        private readonly Startup _startup;

        public IServiceProvider _serviceProvider;

        public static Resource Instance { get; set; }

        public Resource()
        {
            var services = new ServiceCollection();
            _startup = new Startup();
            _startup.InitializeServices(services);

            _serviceProvider = services.BuildServiceProvider();
            Instance = this;
        }

        [ServerEvent(Event.ResourceStart)]
        public void Start()
        {
            InitializeNAPI();
            InitializeFactories();

            var stopwatch = Stopwatch.StartNew();
            var fac = _serviceProvider.GetService<InventoryFactory>();
            fac.Inventories.Add(1, new Factories.Entities.SInventory());

            foreach (var service in _serviceProvider.GetServices<IModule>())
            {
                service.StartAsync();
            }

            foreach (var service in _serviceProvider.GetServices<IHostedService>())
            {
                service.StartAsync(CancellationToken.None);
            }

            stopwatch.Stop();

            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<Resource>();

            logger.LogInformation($"Resource started in {stopwatch.Elapsed.TotalMilliseconds}ms.");
        }

        [ServerEvent(Event.ResourceStop)]
        public void Stop()
        {
            foreach (var service in _serviceProvider.GetServices<IModule>())
            {
                service.StopAsync();
            }
        }

        private void InitializeNAPI()
        {
            NAPI.Server.SetAutoRespawnAfterDeath(false);
        }

        private void InitializeFactories()
        {
            _ = _serviceProvider.GetService<PlayerFactory>();
            _ = _serviceProvider.GetService<ColShapeFactory>();
            _ = _serviceProvider.GetService<VehicleFactory>();
            _ = _serviceProvider.GetService<MarkerFactory>();
        }
    }
}
