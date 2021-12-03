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
namespace Sibaui.Game.Module.Char
{
    public sealed class CharModule : IModule
    {
        public string Name => "Char";

        private readonly ILogger<CharModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;

        public CharModule(ILogger<CharModule> logger, IServiceScopeFactory serviceScopeFactory, EventHub eventHub, Pools pools)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _eventHub = eventHub;
            _pools = pools;
        }

        public Task StartAsync()
        {
            NAPI.ClientEvent.Register<SPlayer, string>("FinishChar", this, FinishChar);

            return Task.CompletedTask;
        }


        public Task StopAsync()
        {
            NAPI.ClientEvent.Unregister("FinishChar");

            return Task.CompletedTask;
        }

        public void SendToCreator(SPlayer player, bool NewChar = false)
        {
            if (!NewChar) return; // Not implemented yet.

            NAPI.Task.Run(() =>
            {
                player.Spawn(new Vector3(402.78842, -996.7156, -99.000305));
                player.Heading = 180;
                player.Transparency = 255;

                player.TriggerEvent("ShowIF", "Char", "");
            });
        }

        public void ApplyChar(SPlayer player, PlayerCharacter character)
        {
            NAPI.Task.Run(() =>
            {
                player.TriggerEvent("SetAppearance", character.Customization);
            });
        }

        private async void FinishChar(SPlayer player, string customizationString)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            PlayerCharacter character = await db.PlayerCharacters.FirstOrDefaultAsync(playerId => playerId.PlayerId == player.Id);
            if (character == null) return;

            bool NewChar = character.Customization == new CustomizationModel().ToJson();
            character.Customization = customizationString;

            await db.SaveChangesAsync();

            if (NewChar)
            {
                player.Login();
                await player.SetPosition(player.TeamInfo.Team.SpawnPosition);
            }
        }
    }
}
