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
using Sibaui.Game.Module.Gangwar;
using Sibaui.Game.Module.Login.Extensions;
using Sibaui.Game.Module.Weapon;
using Sibaui.Game.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Module.Team
{
    // after add to db, add here
    public enum TeamTypes
    {
        CIVILIANS = 1,
    }

    public sealed class TeamModule : IModule
    {
        public string Name => "Team";

        private readonly ILogger<TeamModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;
        private readonly SContext _sContext;

        private WeaponModule _weaponModule;
        private GangwarModule _gangwarModule;

        public static Dictionary<int, Database.Entities.Team> Teams;
        public static Dictionary<int, Database.Entities.TeamHideoutWeapon> TeamHideoutWeapons;

        public TeamModule(ILogger<TeamModule> logger, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider, EventHub eventHub, Pools pools, SContext sContext)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
            _eventHub = eventHub;
            _pools = pools;
            _sContext = sContext;

            Teams = new Dictionary<int, Database.Entities.Team>();
            TeamHideoutWeapons = new Dictionary<int, Database.Entities.TeamHideoutWeapon>();
        }

        public Task StartAsync()
        {
            _weaponModule = _serviceProvider.GetModule<WeaponModule>();
            _gangwarModule = _serviceProvider.GetModule<GangwarModule>();

            _sContext.Teams.Include(g => g.Gangwars)
                            .Include(gw => gw.Gangwars)
                            .Include(t => t.TeamInfo)
                            .ForEach(LoadTeam);

            _sContext.TeamHideoutWeapons.Include(weapon => weapon.Weapon)
                                        .ForEach((w) => TeamHideoutWeapons.Add(w.Id, w));

            NAPI.ClientEvent.Register<SPlayer>("OpenTeam", this, OpenTeam);
            NAPI.ClientEvent.Register<SPlayer, int, int>("BuyWeapon", this, BuyWeapon);
            NAPI.ClientEvent.Register<SPlayer>("EnterGangwar", this, EnterGangwar);

            _eventHub.OnKeyPress += OnKeyPress;

            return Task.CompletedTask;
        }

        private void EnterGangwar(SPlayer player)
        {
            var teamInfo = player.TeamInfo;
            if (teamInfo == null) return;

            var team = teamInfo.Team;
            if (team == null) return;

            var activeGangwar = _gangwarModule.Gangwars.Values.FirstOrDefault(g => g.Game != null && (g.Game.AttackerTeam.Id == team.Id || g.Game.DefenderTeam.Id == team.Id));
            if (activeGangwar == null) return;

            player.Dimension = (uint)_gangwarModule.Name.GetHashCode();
            player.DimensionType = DimensionTypes.GANGWAR;
            player.Spawn(team.SpawnPosition);

            player.SendNotification($"Du bist dem Gangwar: {activeGangwar.Gangwar.Name} beigetreten.", NotificationType.SUCCESS, "Gangwar");
        }

        private void LoadTeam(Database.Entities.Team team)
        {
            if (team.TeamInfo == null)
            {
                var toInsert = new TeamInfo()
                {
                    TeamId = team.Id,
                    WeaponPackets = 1000
                };
                _sContext.TeamInfos.Add(toInsert);
                team.TeamInfo = toInsert;
            }

            Teams.Add(team.Id, team);

            NAPI.Blip.CreateBlip(770, team.SpawnPosition, 1, (byte)team.ColorBlip, team.Name, 255, 0, true);
        }

        public Task StopAsync()
        {
            NAPI.ClientEvent.Unregister("FinishChar");

            return Task.CompletedTask;
        }

        private Task OnKeyPress(SPlayer player, Keys key)
        {
            if (key != Keys.KEY_E) return Task.CompletedTask;

            Database.Entities.PlayerTeamInfo teamInfo = player.TeamInfo;
            if (teamInfo == null) return Task.CompletedTask;

            Database.Entities.Team team = teamInfo.Team;
            if (team == null) return Task.CompletedTask;

            if (team.ManagePosition.DistanceTo(player.Position) > 3) return Task.CompletedTask;

            string gangwarZone = null;
            var activeGangwar = _gangwarModule.Gangwars.Values.FirstOrDefault(g => g.Game != null && (g.Game.AttackerTeam.Id == team.Id || g.Game.DefenderTeam.Id == team.Id));
            if (activeGangwar != null) gangwarZone = activeGangwar.Gangwar.Name;

            if (Teams.TryGetValue(team.Id, out Database.Entities.Team dbTeam))
                player.TriggerEvent("ShowIF", "Base", new TeamHideoutDataModel(dbTeam.TeamInfo.WeaponPackets, teamInfo.WeaponPackets, gangwarZone).ToJson());

            return Task.CompletedTask;
        }

        private async void BuyWeapon(SPlayer player, int weaponId, int amount)
        {
            if (TeamHideoutWeapons.TryGetValue(weaponId, out TeamHideoutWeapon teamHideoutWeapon))
            {
                Database.Entities.PlayerTeamInfo teamInfo = player.TeamInfo;
                if (teamInfo == null) return;

                if (teamInfo.WeaponPackets < (teamHideoutWeapon.Packets * amount))
                {
                    player.SendNotification("Du hast zu wenige Waffenpakete, du h0nd.", NotificationType.ERROR);
                    return;
                }

                if (Teams.TryGetValue(teamInfo.Team.Id, out Database.Entities.Team dbTeam))
                {
                    if (dbTeam.TeamInfo.WeaponPackets < (teamHideoutWeapon.Packets * amount))
                    {
                        player.SendNotification("Deine Fraktion hat nicht genügend Waffenpakete.", NotificationType.ERROR);
                        return;
                    }

                    var price = (teamHideoutWeapon.Price * amount);
                    if (price < 0) return;

                    var packets = (teamHideoutWeapon.Packets * amount);
                    if (packets < 0) return;

                    dbTeam.TeamInfo.WeaponPackets -= packets;
                    teamInfo.WeaponPackets -= packets;

                    await player.PlayProgressbar(TimeSpan.FromSeconds(2), async () =>
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<SContext>();

                        var dbPlayerTeamInfo = await db.PlayerTeamInfos.FindAsync(teamInfo.Id);
                        dbPlayerTeamInfo.WeaponPackets = teamInfo.WeaponPackets;

                        var dbTeamInfo = await db.TeamInfos.FindAsync(dbTeam.TeamInfo.Id);
                        dbTeamInfo.WeaponPackets = dbTeam.TeamInfo.WeaponPackets;

                        await db.SaveChangesAsync();

                        await _weaponModule.GiveWeapon(player, (WeaponHash)teamHideoutWeapon.Weapon.WeaponHash);
                        player.SendNotification($"Du hast {amount}x {teamHideoutWeapon.Name} für {price}$ & {packets} Pakete gekauft.", NotificationType.SUCCESS);
                    }, () => Task.CompletedTask);
                }
            }
        }

        private void OpenTeam(SPlayer player)
        {
            Database.Entities.PlayerTeamInfo teamInfo = player.TeamInfo;
            if (teamInfo == null) return;

            Database.Entities.Team team = teamInfo.Team;
            if (team == null) return;

            if (team.Id == (int)TeamTypes.CIVILIANS)
            {
                player.SendNotification("Du bist in keiner Fraktion.", NotificationType.ERROR, Name);
                return;
            }

            var teamMembers = new List<TeamPlayerDataModel>();
            team.PlayerTeamInfos.ForEach((p) => teamMembers.Add(new TeamPlayerDataModel(p.PlayerId, p.Player.CharacterName, p.Rank, false, false, 0, DateTime.Now, false)));

            player.TriggerEvent("ShowIF", "Team", new TeamDataModel(team.Name, teamMembers, teamInfo.Rank >= 10, teamInfo.Rank).ToJson());
        }
    }
}
