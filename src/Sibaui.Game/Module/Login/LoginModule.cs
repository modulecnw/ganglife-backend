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
using Sibaui.Game.Module.Login.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Sibaui.Database.Context;
using System.Linq;
using Crypt = BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Sibaui.Database.Entities;
using Sibaui.Game.Client.Data.Char;
using Sibaui.Game.Module.Char;
using System.Threading;
using Sibaui.Game.Factories;
using Sibaui.Game.Module.Inventory.Enumerations;

// made by Dschahannam.
namespace Sibaui.Game.Module.Login
{
    public sealed class LoginModule : IModule
    {
        public string Name => "Login";

        private readonly ILogger<LoginModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventHub _eventHub;

        private readonly CharModule _charModule;
        private readonly InventoryFactory _inventoryFactory;

        public LoginModule(ILogger<LoginModule> logger, IServiceScopeFactory serviceScopeFactory, EventHub eventHub,
            InventoryFactory inventoryFactory, CharModule charModule)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _eventHub = eventHub;

            _inventoryFactory = inventoryFactory;
            _charModule = charModule;
        }

        public Task StartAsync()
        {
            _eventHub.OnPlayerConnect += OnPlayerConnect;

            NAPI.ClientEvent.Register<SPlayer, string>("EnterUsername", this, OnEnterUsername);
            NAPI.ClientEvent.Register<SPlayer, string>("TryLogin", this, OnTryLogin);

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _eventHub.OnPlayerConnect -= OnPlayerConnect;

            return Task.CompletedTask;
        }

        private Task OnPlayerConnect(SPlayer player)
        {
            if (player.Id != -1) return Task.CompletedTask;

            player.Position = new Vector3(4273.950, 2975.714, -170.746);
            player.Transparency = 0;
            player.SetInvincible(true);

            player.TriggerEvent("ShowIF", "Input", new InputDataModel("EnterUsername", "Bitte geben Sie Ihren Nutzername ein. (Format: Vorname_Nachname).").ToJson());

            return Task.CompletedTask;
        }

        private void OnEnterUsername(SPlayer player, string username)
        {
            if (player.Id != -1) return;

            player.Name = username;

            player.TriggerEvent("ShowIF", "Login", player.Name);
        }

        private async void OnTryLogin(SPlayer player, string password)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            // shit code, for test serverino
            string Name = await player.GetName();
            bool Exists = await db.Players.FirstOrDefaultAsync((p) => p.CharacterName == Name) != null;

            Database.Entities.Player dbPlayer = null;
            if (Exists) dbPlayer = await db.Players.Include(teamInfo => teamInfo.PlayerTeamInfos).ThenInclude(team => team.Team)
                                            .Include(character => character.PlayerCharacters)
                                            .Include(weapon => weapon.PlayerWeapons)
                                            .Include(inventory => inventory.PlayerInventories)
                                            .OrderBy(u => u.Id)
                                            .AsSplitQuery()
                                            .FirstOrDefaultAsync((p) => p.CharacterName == Name);

            if (dbPlayer == null)
            {
                player.SendNotification("Account erfolgreich registriert!", NotificationType.SUCCESS);

                var toInsert = new Database.Entities.Player()
                {
                    CharacterName = await player.GetName(),
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    SocialclubName = await player.GetSocialClubName(),
                    Serial = await player.GetSerial(),
                };

                await db.Players.AddAsync(toInsert);
                await db.SaveChangesAsync();

                dbPlayer = toInsert;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, dbPlayer.Password))
            {
                player.SendNotification("Bitte überprüfe dein Passwort.", NotificationType.ERROR);
                return;
            }

            player.Id = dbPlayer.Id;
            player.Money = dbPlayer.Money;
            player.Weapons = dbPlayer.PlayerWeapons;
            //player.Inventory = _inventoryFactory.Load(InventoryTypes.PLAYER, )

            /* Load Data */
            // bad code >:(
            if (dbPlayer.PlayerTeamInfos.FirstOrDefault() == null)
            {
                var toInsert = new PlayerTeamInfo()
                {
                    PlayerId = dbPlayer.Id,
                    TeamId = 1,
                    Rank = 0,
                    Salary = 0
                };

                await db.PlayerTeamInfos.AddAsync(toInsert);
                await db.SaveChangesAsync();

                player.TeamInfo = await db.PlayerTeamInfos.Include(p => p.Team).FirstAsync(p => p.Id == toInsert.Id);
            }
            else
            {
                player.TeamInfo = dbPlayer.PlayerTeamInfos.FirstOrDefault();
            }

            if (dbPlayer.PlayerInventories.FirstOrDefault() == null)
            {
                player.Inventory = await _inventoryFactory.Create(InventoryTypes.PLAYER, player.Id);

                var toInsert = new PlayerInventory()
                {
                    PlayerId = dbPlayer.Id,
                    InventoryId = player.Inventory.Id
                };

                await db.PlayerInventories.AddAsync(toInsert);
                await db.SaveChangesAsync();
            }
            else
            {
                player.Inventory = await _inventoryFactory.Load(dbPlayer.PlayerInventories.FirstOrDefault().InventoryId);
            }


            if (dbPlayer.PlayerCharacters.FirstOrDefault() == null)
            {
                var toInsert = new PlayerCharacter()
                {
                    PlayerId = dbPlayer.Id,
                    Customization = new CustomizationModel().ToJson(),
                };

                await db.PlayerCharacters.AddAsync(toInsert);
                await db.SaveChangesAsync();

                player.Character = toInsert;

                _charModule.SendToCreator(player, true);

                return;
            }
            else
            {
                player.Character = dbPlayer.PlayerCharacters.FirstOrDefault();
                if (player.Character != null)
                    _charModule.ApplyChar(player, player.Character);
            }

            player.Login();
            await player.SetPosition(dbPlayer.Position);
        }
    }
}
