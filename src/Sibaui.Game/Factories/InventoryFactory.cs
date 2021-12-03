using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sibaui.Core.Models;
using Sibaui.Database.Context;
using Sibaui.Database.Entities;
using Sibaui.Game.Events;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Module.Inventory.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Factories
{
    public sealed class InventoryFactory : ISingleton
    {
        public string Name => "InventoryFactory";

        private readonly ILogger<InventoryFactory> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventHub _eventHub;

        public Dictionary<int, SInventory> Inventories;

        public InventoryFactory(ILogger<InventoryFactory> logger, IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory, EventHub eventHub)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceScopeFactory = serviceScopeFactory;
            _eventHub = eventHub;

            Inventories = new Dictionary<int, SInventory>();
        }

        public async Task<SInventory> Create(InventoryTypes type, int targetId)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SContext>();

                await db.Inventories.AddAsync(new Database.Entities.Inventory() { InventoryTypeId = (int)type, TargetId = targetId });
                await db.SaveChangesAsync();

                return await Load(type, targetId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }

        public async Task<SInventory> Load(InventoryTypes type, int targetId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            Inventory inventory = await db.Inventories.Include(items => items.InventoryItems)
                                                                  .Include(type => type.InventoryType)
                                                                  .FirstOrDefaultAsync(inventory => inventory.InventoryTypeId == (int)type &&
                                                                                                    inventory.TargetId == targetId);

            if (inventory == null)
                return await Create(type, targetId);

            if (Inventories.TryGetValue(inventory.Id, out SInventory existingInventory))
                return existingInventory;

            SInventory inventoryEntity = new SInventory()
            {
                Id = inventory.Id,
                InventoryType = inventory.InventoryType,
                TargetId = targetId
            };

            foreach (InventoryItem item in inventory.InventoryItems)
            {
                if (!inventoryEntity.Items.ContainsKey(item.Slot))
                    inventoryEntity.Items.Add(item.Slot, item);
                else
                    _logger.LogInformation($"Inventory #{item.InventoryId} has duplicated Items on Slot {item.Slot}");
            }

            Inventories.Add(inventory.Id, inventoryEntity);

            return inventoryEntity;
        }
        public async Task<SInventory> Load(int inventoryId)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            Inventory inventory = await db.Inventories.Include(items => items.InventoryItems)
                                                                  .Include(type => type.InventoryType)
                                                                  .FirstOrDefaultAsync(inventory => inventory.Id == inventoryId);

            if (inventory == null)
                return null;

            if (Inventories.TryGetValue(inventory.Id, out SInventory existingInventory))
                return existingInventory;

            SInventory inventoryEntity = new SInventory()
            {
                Id = inventory.Id,
                InventoryType = inventory.InventoryType,
                TargetId = inventory.TargetId
            };

            foreach (InventoryItem item in inventory.InventoryItems)
            {
                if (!inventoryEntity.Items.ContainsKey(item.Slot))
                    inventoryEntity.Items.Add(item.Slot, item);
                else
                    _logger.LogInformation($"Inventory #{item.InventoryId} has duplicated Items on Slot {item.Slot}");
            }

            Inventories.Add(inventory.Id, inventoryEntity);

            return inventoryEntity;
        }

        public async Task<bool> AddItem(SInventory inventory, Item item, int Amount)
        {
            if (Amount < 1) return false;

            var localItems = inventory.Items.Where(d => (d.Value.ItemId == item.Id) && (d.Value.Amount < item.MaxStack)).ToDictionary(pair => pair.Key).Values;
            int toBeAdded = Amount;

            foreach (var localItem in localItems)
            {
                int oldAmount = localItem.Value.Amount;
                int newAmount = localItem.Value.Amount += toBeAdded;

                newAmount = newAmount >= item.MaxStack ? item.MaxStack : newAmount;
                localItem.Value.Amount = newAmount;
                toBeAdded -= (newAmount - oldAmount);

                await ChangeAmount(inventory, localItem.Key, newAmount);

                if (toBeAdded <= 0) return true;
            }

            for (int i = 1; i < inventory.InventoryType.MaxSlots; i++)
            {
                if (inventory.Items.Keys.Contains(i)) continue;

                var newItem = new InventoryItem()
                {
                    InventoryId = inventory.Id,
                    ItemId = item.Id,
                    Amount = toBeAdded >= item.MaxStack ? item.MaxStack : toBeAdded,
                    Slot = i
                };

                toBeAdded -= item.MaxStack;
                inventory.Items.Add(i, newItem);

                using var scope = _serviceScopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SContext>();

                await db.InventoryItems.AddAsync(newItem);
                await db.SaveChangesAsync();

                if (toBeAdded <= 0)
                    return true;
            }
            return false;
        }

        public async Task ChangeAmount(SInventory inventory, int Slot, int Amount)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            InventoryItem item = await db.InventoryItems.FirstOrDefaultAsync(i => i.InventoryId == inventory.Id && i.Slot == Slot);
            if (item == null) return;

            item.Amount = Amount;

            db.InventoryItems.Update(item);

            await db.SaveChangesAsync();
        }
    }
}
