using GTANetworkAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sibaui.Core.Extensions;
using Sibaui.Core.Models;
using Sibaui.Database.Context;
using Sibaui.Database.Entities;
using Sibaui.Game.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Controller
{
    public class PlayerService : BackgroundService
    {
        public string Name => "PlayerController";

        private readonly ILogger<PlayerService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Pools _pools;

        public PlayerService(ILogger<PlayerService> logger, IServiceScopeFactory serviceScopeFactory, Pools pools)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _pools = pools;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.WhenAll(SaveCharactersAsync());
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, string.Empty);
                }

                await Task.Delay(30 * 1000, stoppingToken);
            }
        }

        /// <summary>
        /// Saves everything.
        /// </summary>
        private async Task SaveCharactersAsync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            foreach (var player in _pools.GetAllPlayers())
            {
                var dbPlayer = await db.Players.FindAsync(player.Id);
                if (dbPlayer == null) return;

                dbPlayer.Position = await player.GetPosition();
            }

            await db.SaveChangesAsync();
        }
    }
}
