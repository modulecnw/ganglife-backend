using GTANetworkAPI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sibaui.Core.Models;
using Sibaui.Game.Controller.Window;
using Sibaui.Game.Events;
using Sibaui.Core.Extensions;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;
using Sibaui.Game.Client.Data;
using Microsoft.Extensions.DependencyInjection;
using Sibaui.Database.Context;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sibaui.Database.Entities;
using System.Threading;
using Sibaui.Game.Module.Login.Extensions;
using Sibaui.Game.Singletons;
using Sibaui.Game.Module.Injury.Interface;
using Sibaui.Game.Module.Gangwar;

// made by Dschahannam.
namespace Sibaui.Game.Module.Injury
{
    public sealed class InjuryModule : IModule
    {
        public string Name => "Injury";

        private readonly ILogger<InjuryModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;

        private GangwarModule _gangwarModule;

        private const int DEATH_TIME_MINUTES = 2;

        public InjuryModule(ILogger<InjuryModule> logger, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider, EventHub eventHub, Pools pools)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
            _eventHub = eventHub;
            _pools = pools;
        }

        public Task StartAsync()
        {
            _gangwarModule = _serviceProvider.GetModule<GangwarModule>();

            _eventHub.OnPlayerDeath += OnPlayerDeath;
            _eventHub.OnEveryMinute += OnEveryMinute;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _eventHub.OnPlayerDeath -= OnPlayerDeath;
            _eventHub.OnEveryMinute -= OnEveryMinute;

            return Task.CompletedTask;
        }

        private async Task OnEveryMinute()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            foreach (var player in _pools.GetAllPlayers().Where(p => p.Injured && p.InjuryTimeLeft > 0))
            {
                if (player.InjuryTimeLeft <= 1)
                {
                    NAPI.Task.Run(() =>
                    {
                        player.StopAnimation();
                        player.Injured = false;
                        player.InjuryTimeLeft = 0;

                        player.TriggerEvent("SetInjured", false);
                        player.TriggerEvent("StopEffect", "DeathFailMPIn");

                        // Module Injuries
                        IModuleInjury injury;

                        // Gangwar:

                        if (player.DimensionType == DimensionTypes.GANGWAR)
                        {
                            injury = (IModuleInjury)_gangwarModule;
                            injury.OnPlayerRevive(player);
                        }
                        else OnPlayerRevive(player);
                        player.SendNotification("Du wurdest wiederbelebt.", NotificationType.INFO);

                    });

                    var dbPlayer = await db.Players.FindAsync(player.Id);
                    if (dbPlayer == null) continue;

                    dbPlayer.Injured = false;

                    continue;
                }

                NAPI.Task.Run(() =>
                {
                    player.InjuryTimeLeft--;
                    player.PlayAnimation("missarmenian2", "corpse_search_exit_ped", 1);
                });
            }

            await db.SaveChangesAsync();
        }

        private Task OnPlayerDeath(SPlayer player, SPlayer killer, uint reason)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            var dbPlayer = db.Players.Find(player.Id);
            if (dbPlayer == null) return Task.CompletedTask;

            dbPlayer.Injured = true;
            db.SaveChanges();

            player.ApplyInjury(DEATH_TIME_MINUTES);
            player.SendNotification("Du bist gestorben.", NotificationType.INFO);

            return Task.CompletedTask;
        }

        private void OnPlayerRevive(SPlayer player)
        {
            player.Spawn(player.TeamInfo.Team.SpawnPosition);
            player.Dimension = 0;
            player.DimensionType = DimensionTypes.WORLD;
        }
    }
}
