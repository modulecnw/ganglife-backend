using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sibaui.Core.Enumerations;
using Sibaui.Core.Extensions;
using Sibaui.Core.Models;
using Sibaui.Database.Context;
using Sibaui.Database.Entities;
using Sibaui.Game.Client.Data.Char;
using Sibaui.Game.Client.Data.Garage;
using Sibaui.Game.Client.Data.Team;
using Sibaui.Game.Client.Data.Team.Hideout;
using Sibaui.Game.Events;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Module.Login.Extensions;
using Sibaui.Game.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Module.Weapon
{
    public sealed class WeaponModule : IModule
    {
        public string Name => "Weapon";

        private readonly ILogger<WeaponModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;

        public WeaponModule(ILogger<WeaponModule> logger, IServiceScopeFactory serviceScopeFactory, EventHub eventHub, Pools pools)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _eventHub = eventHub;
            _pools = pools;
        }

        public Task StartAsync() => Task.CompletedTask;
        public Task StopAsync() => Task.CompletedTask;
       
        public async Task GiveWeapon(SPlayer player, WeaponHash weaponHash, int ammo = 9999)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();
         
            NAPI.Task.Run(() => player.GiveWeapon(weaponHash, ammo));
            
            if (await db.PlayerWeapons.FirstOrDefaultAsync(w => w.PlayerId == player.Id && w.WeaponHash == (uint)weaponHash) != null) return;

            var toInsert = new PlayerWeapon()
            {
                PlayerId = player.Id,
                WeaponHash = (uint)weaponHash,
                Ammo = ammo
            };

            await db.PlayerWeapons.AddAsync(toInsert);
            await db.SaveChangesAsync();

            player.Weapons.Add(toInsert);
        }
    }
}
