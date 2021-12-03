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
using Sibaui.Game.Controller.Window;
using Sibaui.Game.Events;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Models;
using Sibaui.Game.Module.Gangwar.Model;
using Sibaui.Game.Module.Garage.Interface;
using Sibaui.Game.Module.Injury.Interface;
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
namespace Sibaui.Game.Module.Gangwar
{
    public sealed class GangwarModule : IModule, IModuleGarage, IModuleInjury
    {
        public string Name => "GangwarModule";

        private readonly ILogger<GangwarModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;
        private readonly SContext _sContext;

        public Dictionary<int, GangwarInfoModel> Gangwars = new Dictionary<int, GangwarInfoModel>();

        private const int GANGWAR_TIME_MINUTES = 16;

        public GangwarModule(ILogger<GangwarModule> logger, IServiceScopeFactory serviceScopeFactory, EventHub eventHub, Pools pools, SContext sContext)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _eventHub = eventHub;
            _pools = pools;
            _sContext = sContext;
        }

        public Task StartAsync()
        {
            _sContext.Gangwars.ForEach(Load);

            _eventHub.OnPlayerEnterColshape += OnPlayerEnterColshape;
            _eventHub.OnPlayerLeaveColshape += OnPlayerLeaveColshape;
            _eventHub.OnPlayerDeath += OnPlayerDeath;
            _eventHub.OnKeyPress += OnKeyPress;

            NAPI.ClientEvent.Register<SPlayer>("AttackGangwar", this, AttackGangwar);

            return Task.CompletedTask;
        }

        public Task OnPlayerDeath(SPlayer player, SPlayer killer, uint reason)
        {
            if (player.DimensionType != DimensionTypes.GANGWAR) return Task.CompletedTask;

            killer = player;

            // if (killer == null || !killer.Exists || killer.Type != EntityType.Player) return Task.CompletedTask;
            Database.Entities.PlayerTeamInfo teamInfo = killer.TeamInfo;
            if (teamInfo == null) return Task.CompletedTask;

            Database.Entities.Team team = teamInfo.Team;
            if (team == null) return Task.CompletedTask;

            var activeGangwar = Gangwars.Values.FirstOrDefault(g => g.Game != null && (g.Game.AttackerTeam.Id == team.Id || g.Game.DefenderTeam.Id == team.Id));
            if (activeGangwar == null)
            {
                //maybe revive? yeees
                return Task.CompletedTask;
            }

            bool Leader = teamInfo.Rank >= 10;
            bool IsKillerAttacker = team.Id == activeGangwar.Game.AttackerTeam.Id;

            // to avoid team-killing bra
            if (killer.TeamInfo.Team.Id == player.TeamInfo.Team.Id)
                IsKillerAttacker = !IsKillerAttacker;

            int Points = Leader ? 4 : 3;

            if (IsKillerAttacker)
                activeGangwar.Game.AttackerPoints += Points;
            else
                activeGangwar.Game.DefenderPoints += Points;

            foreach (var teamPlayer in _pools.GetAllPlayers().Where(p => p.TeamInfo.Team.Id == team.Id))
            {
                if (killer.TeamInfo.Team.Id == player.TeamInfo.Team.Id)
                {
                    if (Leader)
                        teamPlayer.SendNotification($"+4 Punkte an das Gegnerteam für den Tot von Leader {player.Name}.", team.HexColor, "Gangwar Team-Kill");
                    else
                        teamPlayer.SendNotification($"+3 Punkte an das Gegnerteam für den Tot von {player.Name}.", team.HexColor, "Gangwar Team-Kill");

                    continue;
                }

                if (Leader)
                    teamPlayer.SendNotification($"+4 Punkte für den Tot von Leader {player.Name}.", team.HexColor, "Gangwar Kill");
                else
                    teamPlayer.SendNotification($"+3 Punkte für den Tot von {player.Name}.", team.HexColor, "Gangwar Kill");
            }

            UpdateGangwarHud(activeGangwar);

            return Task.CompletedTask;
        }

        /*private async Task ConfirmParkOut(SPlayer player)
        {
            GangwarInfoModel gangwarInfoModel = Gangwars.Values.FirstOrDefault(g => g.Gangwar.AttackPosition.DistanceTo(player.Position) <= 3);
            if (gangwarInfoModel == null) return;
            if (player.Position.DistanceTo(gangwarInfoModel.Gangwar.ParkOutPedPosition) >= 3f) return;

            //GET FREE PARKOUT SPOT

            SVehicle VehicleToSpawn = (SVehicle)NAPI.Vehicle.CreateVehicle(VehicleHash.Kuruma, new Vector3(), 0f, new Color(), new Color(), player.TeamInfo.Team.ShortName);
            VehicleToSpawn.Id = player.TeamInfo.Team.Id;

            player.SendNotification("Fahrzeug ausgeparkt.", NotificationType.SUCCESS, "Gangwar");
        }*/

        public void GetAvailableGarageVehicles(ref List<GarageVehicleDataModel> vehicles) => vehicles.Add(new GarageVehicleDataModel(1, "Kuruma"));

        // this is just every subscribed minute:
        // gangwar ends exactly x minutes after it started
        private Task OnEveryMinute()
        {
            foreach (var gangwar in Gangwars.Values)
            {
                // just handle all running gangwars
                if (gangwar.Game == null) continue;

                //TODO: add production gangwar time
                if (gangwar.Game.StartedAt.AddMinutes(GANGWAR_TIME_MINUTES) < DateTime.Now)
                {
                    // WIN
                    using var scope = _serviceScopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SContext>();

                    Database.Entities.Team winningTeam = gangwar.Game.AttackerPoints >= gangwar.Game.DefenderPoints ? gangwar.Game.AttackerTeam : gangwar.Game.DefenderTeam;

                    gangwar.Territory.Shape.Players.Values.ForEach(async (p) =>
                    {
                        p.SendNotification(winningTeam.Id == p.TeamInfo.Team.Id ? "Sie haben den Kampf um die Zone gewonnen." : "Sie haben den Kampf um die Zone verloren.", NotificationType.SUCCESS);

                        p.Injured = false;
                        p.InjuryTimeLeft = 0;

                        NAPI.Task.Run(() =>
                        {
                            p.StopAnimation();
                            p.TriggerEvent("SetInjured", false);
                            p.TriggerEvent("StopEffect", "DeathFailMPDark");
                            p.TriggerEvent("UpdateView", "StopGangwarHud");
                        });

                        await p.SetPosition(p.TeamInfo.Team.SpawnPosition);
                        await p.SetDimension(0);
                        p.DimensionType = DimensionTypes.WORLD;
                    });

                    NAPI.Task.Run(() => gangwar.Blip.Color = winningTeam.ColorBlip);

                    gangwar.Gangwar.Team = winningTeam;
                    gangwar.Gangwar.TeamId = winningTeam.Id;
                    gangwar.Gangwar.LastAttack = DateTime.Now;

                    var dbGangwar = db.Gangwars.Find(gangwar.Gangwar.Id);
                    if (dbGangwar == null)
                    {
                        gangwar.Game = null;
                        continue;
                    }

                    dbGangwar.Team = winningTeam;
                    dbGangwar.TeamId = winningTeam.Id;
                    dbGangwar.LastAttack = DateTime.Now;

                    db.SaveChanges();

                    _eventHub.OnEverySubscribedMinute -= OnEveryMinute;
                    gangwar.Game = null;

                    continue;
                }

                if (gangwar.Game.AttackerFlags > 0 || gangwar.Game.DefenderFlags > 0)
                {
                    gangwar.Game.AttackerPoints += gangwar.Game.AttackerFlags * 2;
                    gangwar.Game.DefenderPoints += gangwar.Game.DefenderFlags * 2;

                    UpdateGangwarHud(gangwar);
                }
            }

            return Task.CompletedTask;
        }

        private Task OnKeyPress(SPlayer player, Keys key)
        {
            if (key != Keys.KEY_E) return Task.CompletedTask;

            GangwarInfoModel gangwarInfoModel = Gangwars.Values.FirstOrDefault(g => g.Gangwar.AttackPosition.DistanceTo(player.Position) <= 3);
            if (gangwarInfoModel == null) return Task.CompletedTask;

            if (gangwarInfoModel.Gangwar.LastAttack.AddHours(6) > DateTime.Now)
            {
                player.SendNotification($"Das Gangwargebiet kann erst um {gangwarInfoModel.Gangwar.LastAttack.AddHours(6)} Uhr angegriffen werden!", NotificationType.ERROR, "GANGWAR", 5000);
                return Task.CompletedTask;
            }

            player.TriggerEvent("ShowIF", "Confirm", new ConfirmDataModel($"Möchtest du Gangwargebiet: {gangwarInfoModel.Gangwar.Name} angreifen?", "Angreifen", "Abbrechen", "AttackGangwar").ToJson());

            return Task.CompletedTask;
        }

        private void AttackGangwar(SPlayer player)
        {
            GangwarInfoModel gangwarInfoModel = Gangwars.Values.FirstOrDefault(g => g.Gangwar.AttackPosition.DistanceTo(player.Position) <= 3);
            if (gangwarInfoModel == null) return;

            gangwarInfoModel.Game = new GangwarGameModel()
            {
                AttackerTeam = player.TeamInfo.Team,
                AttackerFlags = 0,
                AttackerPoints = 0,

                DefenderTeam = gangwarInfoModel.Gangwar.Team,
                DefenderFlags = 0,
                DefenderPoints = 0,
                StartedAt = DateTime.Now
            };

            NAPI.Task.Run(() =>
            {
                player.Spawn(player.TeamInfo.Team.SpawnPosition);
                player.Dimension = 0;
                player.DimensionType = DimensionTypes.WORLD;

                player.SendNotification("Du hast nun den Kampf begonnen. Trete dem Gangwar über dem Fraktionsschrank bei.", NotificationType.SUCCESS, "Gangwar");
            });

            _eventHub.OnEverySubscribedMinute += OnEveryMinute;
        }

        private void Load(Database.Entities.Gangwar gangwar)
        {
            var gangwarInfo = new GangwarInfoModel()
            {
                AvailableRewards = 0, /* maybe load or save them */
                Gangwar = gangwar
            };

            // not beautiful code, but works :_:
            var markerShape = new SMarkerShape(NAPI.Entity.CreateMarkerColshape(MarkerType.DebugSphere, gangwarInfo.Gangwar.MiddlePosition, gangwarInfo.Gangwar.Size, (uint)Name.GetHashCode()));
            markerShape.Marker.CreateMessage($"Du betrittst nun: {gangwarInfo.Gangwar.Name}.", 5000, NotificationType.SUCCESS);
            markerShape.Shape.GangwarInfo = gangwarInfo;
            gangwarInfo.Territory = markerShape;

            var flag1MarkerShape = new SMarkerShape(NAPI.Entity.CreateMarkerColshape(MarkerType.CheckeredFlagRect, gangwarInfo.Gangwar.Flag1Position, 1, (uint)Name.GetHashCode()));
            flag1MarkerShape.Shape.GangwarFlag = new GangwarFlagModel(gangwar.Id);
            gangwarInfo.Flag1 = flag1MarkerShape;

            var flag2MarkerShape = new SMarkerShape(NAPI.Entity.CreateMarkerColshape(MarkerType.CheckeredFlagRect, gangwarInfo.Gangwar.Flag2Position, 1, (uint)Name.GetHashCode()));
            flag2MarkerShape.Shape.GangwarFlag = new GangwarFlagModel(gangwar.Id);
            gangwarInfo.Flag2 = flag2MarkerShape;

            var flag3MarkerShape = new SMarkerShape(NAPI.Entity.CreateMarkerColshape(MarkerType.CheckeredFlagRect, gangwarInfo.Gangwar.Flag3Position, 1, (uint)Name.GetHashCode()));
            flag3MarkerShape.Shape.GangwarFlag = new GangwarFlagModel(gangwar.Id);
            gangwarInfo.Flag3 = flag3MarkerShape;

            Gangwars.Add(gangwar.Id, gangwarInfo);

            gangwarInfo.Blip = NAPI.Blip.CreateBlip(543, gangwar.MiddlePosition, 1, (byte)gangwarInfo.Gangwar.Team.ColorBlip, $"Gangwar {gangwarInfo.Gangwar.Name}", 255, 0, true);
        }


        public Task StopAsync()
        {
            //NAPI.ClientEvent.Unregister("FinishChar");

            return Task.CompletedTask;
        }

        private Task OnPlayerEnterColshape(SColShape colShape, SPlayer player)
        {
            if (colShape.GangwarFlag != null)
            {
                if (colShape.Players.Count > 1)
                    return Task.CompletedTask;

                if (colShape.Players.Count == 1)
                {
                    if (Gangwars.TryGetValue(colShape.GangwarFlag.GameId, out GangwarInfoModel flagGangwarInfo))
                    {
                        if (flagGangwarInfo.Game.AttackerFlags + flagGangwarInfo.Game.DefenderFlags >= 3) return Task.CompletedTask;

                        if (player.TeamInfo.TeamId == flagGangwarInfo.Game.AttackerTeam.Id)
                            flagGangwarInfo.Game.AttackerFlags++;

                        if (player.TeamInfo.TeamId == flagGangwarInfo.Game.DefenderTeam.Id)
                            flagGangwarInfo.Game.DefenderFlags++;

                        UpdateGangwarHud(flagGangwarInfo);
                    }
                }

                return Task.CompletedTask;
            }

            var gangwarInfo = colShape.GangwarInfo;
            if (gangwarInfo == null) return Task.CompletedTask;

            var gangwar = gangwarInfo.Gangwar;
            if (gangwar == null) return Task.CompletedTask;

            var gangwarGame = gangwarInfo.Game;
            if (gangwarGame == null)
            {
                NAPI.Task.Run(() =>
                {
                    player.Spawn(player.TeamInfo.Team.SpawnPosition);
                    player.Dimension = 0;
                    player.DimensionType = DimensionTypes.WORLD;
                });
                return Task.CompletedTask;
            }

            //TODO: add production gangwar time
            player.TriggerEventSafe("UpdateView", "ShowGangwarHud", gangwarGame.AttackerTeam.Id, gangwarGame.DefenderTeam.Id,
                                                                gangwarGame.AttackerPoints, gangwarGame.DefenderPoints, 3, Convert.ToInt32(gangwarGame.StartedAt.AddMinutes(GANGWAR_TIME_MINUTES).Subtract(DateTime.Now).TotalSeconds));

            return Task.CompletedTask;
        }

        private Task OnPlayerLeaveColshape(SColShape colShape, SPlayer player)
        {
            if (colShape.GangwarFlag != null)
            {
                if (colShape.Players.Count >= 1)
                    return Task.CompletedTask;

                if (colShape.Players.Count == 0)
                {
                    if (Gangwars.TryGetValue(colShape.GangwarFlag.GameId, out GangwarInfoModel flagGangwarInfo))
                    {
                        if (flagGangwarInfo.Game == null) return Task.CompletedTask;

                        if (player.TeamInfo.TeamId == flagGangwarInfo.Game.AttackerTeam.Id)
                            if (flagGangwarInfo.Game.AttackerFlags > 0)
                                flagGangwarInfo.Game.AttackerFlags--;

                        if (player.TeamInfo.TeamId == flagGangwarInfo.Game.DefenderTeam.Id)
                            if (flagGangwarInfo.Game.DefenderFlags > 0)
                                flagGangwarInfo.Game.DefenderFlags--;

                        UpdateGangwarHud(flagGangwarInfo);
                    }
                }

                return Task.CompletedTask;
            }

            var gangwarInfo = colShape.GangwarInfo;
            if (gangwarInfo == null) return Task.CompletedTask;

            var gangwar = gangwarInfo.Gangwar;
            if (gangwar == null) return Task.CompletedTask;

            player.TriggerEventSafe("UpdateView", "StopGangwarHud");

            var gangwarGame = gangwarInfo.Game;
            if (gangwarGame == null)
            {
                NAPI.Task.Run(() =>
                {
                    player.Spawn(player.TeamInfo.Team.SpawnPosition);
                    player.Dimension = 0;
                    player.DimensionType = DimensionTypes.WORLD;
                });
            }

            return Task.CompletedTask;
        }

        private void UpdateGangwarHud(GangwarInfoModel gangwarInfoModel) =>
            gangwarInfoModel.Territory.Shape.Players.Values.ForEach((p) =>
                    p.TriggerEventSafe("UpdateView", "UpdateGangwarHud", gangwarInfoModel.Game.AttackerPoints, gangwarInfoModel.Game.DefenderPoints,
                                                            gangwarInfoModel.Game.AttackerFlags, gangwarInfoModel.Game.DefenderFlags));

        public void OnPlayerRevive(SPlayer player)
        {
            player.Spawn(player.TeamInfo.Team.SpawnPosition);
            player.Dimension = (uint)Name.GetHashCode();
            player.DimensionType = DimensionTypes.GANGWAR;
        }
    }
}
