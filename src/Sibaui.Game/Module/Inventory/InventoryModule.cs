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
using Sibaui.Game.Client.Data.Inventory;
using Sibaui.Game.Events;
using Sibaui.Game.Factories;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Module.Inventory.Interface;
using Sibaui.Game.Module.Inventory.Models;
using Sibaui.Game.Module.Login.Extensions;
using Sibaui.Game.Singletons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Module.Inventory
{
    public sealed class InventoryModule : IModule
    {
        public string Name => "Inventory";

        private readonly ILogger<InventoryModule> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;
        private readonly SContext _sContext;

        private readonly InventoryFactory _inventoryFactory;

        public Dictionary<int, Item> Items;
        public Dictionary<int, InventoryType> InventoryTypes;
        public IEnumerable<IModuleInventory> ModuleInventories;

        public InventoryModule(ILogger<InventoryModule> logger, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, EventHub eventHub, Pools pools, SContext sContext,
                                InventoryFactory inventoryFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
            _eventHub = eventHub;
            _pools = pools;
            _sContext = sContext;

            _inventoryFactory = inventoryFactory;
        }

        public Task StartAsync()
        {
            NAPI.ClientEvent.Register<SPlayer, int, int, int, int>("MoveItem", this, MoveItem);
            NAPI.ClientEvent.Register<SPlayer, int, int>("UseItem", this, UseItem);

            NAPI.ClientEvent.Register<SPlayer>("Comma", this, Comma);
            NAPI.ClientEvent.Register<SPlayer>("Dot", this, Dot);

            ModuleInventories = _serviceProvider.GetServices<IModuleInventory>();
            Items = _sContext.Items.ToDictionary(item => item.Id);
            InventoryTypes = _sContext.InventoryTypes.ToDictionary(item => item.Id);

            _eventHub.OnKeyPress += OnKeyPress;

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _eventHub.OnKeyPress -= OnKeyPress;

            return Task.CompletedTask;
        }

        private async Task OnKeyPress(SPlayer player, Keys key)
        {
            if (key != Keys.KEY_I) return;
            if (player.Inventory == null) return;

            var inventoryDataModel = new InventoryDataModel(player.Inventory.GetSingleInventoryModel());

            SInventory moduleInventory = await GetModuleInventory(player);
            if (moduleInventory != null) inventoryDataModel.Inventories.Add(moduleInventory.GetSingleInventoryModel());

            player.LocalInventoryData = new LocalInventoryData(player.Inventory, moduleInventory);

            player.TriggerEventSafe("ShowIF", "Inventory", inventoryDataModel.ToJson());
        }

        private void Comma(SPlayer player)
        {
            if (player.Inventory == null) return;
            UseItem(player, player.Inventory.Id, 0);
        }

        private void Dot(SPlayer player)
        {
            if (player.Inventory == null) return;
            UseItem(player, player.Inventory.Id, 1);
        }

        private async void UseItem(SPlayer player, int selectedInventoryId, int selectedItemSlot) =>
            await player.LocalInventoryData.RunTransaction(player, async () =>
            {
                _logger.LogInformation($"UseItem - {await player.GetName()} < selectedInventoryId: {selectedInventoryId} - selectedItemSlot: {selectedItemSlot}");

                var localInventoryData = player.LocalInventoryData;

                SInventory selectedInventory = localInventoryData.Inventories.FirstOrDefault(i => i.Id == selectedInventoryId);
                if (selectedInventory == null) return;

            });

        /// <summary>
        /// InventoryTransaction: MoveItem
        /// </summary>
        /// <param name="player"></param>
        /// <param name="selectedItemSlot"></param>
        /// <param name="targetItemSlot"></param>
        /// <param name="selectedInventoryId"></param>
        /// <param name="targetInventoryId"></param>
        private async void MoveItem(SPlayer player, int selectedItemSlot, int targetItemSlot, int selectedInventoryId, int targetInventoryId) =>
            await player.LocalInventoryData.RunTransaction(player, async () =>
            {
                _logger.LogInformation($"MoveItem - {await player.GetName()} < selectedElement: {selectedItemSlot} - targetElement: {targetItemSlot} - selectedInventory: {selectedInventoryId} - {targetInventoryId}");

                var localInventoryData = player.LocalInventoryData;

                SInventory selectedInventory = localInventoryData.Inventories.FirstOrDefault(i => i.Id == selectedInventoryId);
                if (selectedInventory == null) return;

                SInventory targetInventory = localInventoryData.Inventories.FirstOrDefault(i => i.Id == targetInventoryId);
                if (targetInventory == null) return;

                if (selectedInventory.Items.TryGetValue(selectedItemSlot, out InventoryItem selectedItem))
                {
                    if (targetInventory.Items.TryGetValue(targetItemSlot, out InventoryItem targetItem))
                    {
                        // Spieler verschiebt das Item auf einen Slot, wo ein Item liegt.
                        // -> Items tauschen
                        targetInventory.Items.Remove(targetItemSlot);
                        selectedInventory.Items.Remove(selectedItemSlot);

                        targetItem.Slot = selectedItemSlot;
                        selectedItem.Slot = targetItemSlot;

                        targetItem.InventoryId = selectedInventoryId;
                        selectedItem.InventoryId = targetInventoryId;

                        targetInventory.Items.Add(targetItemSlot, selectedItem);
                        selectedInventory.Items.Add(selectedItemSlot, targetItem);
                    }
                    else
                    {
                        // Spieler verschiebt das Item auf einen leeren Slot.
                        selectedItem.Slot = targetItemSlot;
                        selectedItem.InventoryId = targetInventoryId;

                        selectedInventory.Items.Remove(selectedItemSlot);
                        targetInventory.Items.Add(targetItemSlot, selectedItem);
                    }
                }
            });


        public async Task<SInventory> GetModuleInventory(SPlayer player)
        {
            SInventory inventory = null;
            await ModuleInventories.ForEach(async module =>
            {
                SInventory moduleInventory = await module.GetModuleInventory(player);
                if (moduleInventory != null)
                {
                    inventory = moduleInventory;
                    return true;
                }

                return false;
            });

            return inventory;
        }
    }
}
