using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Module.Inventory.Interface
{
    public interface IModuleInventory
    {
        Task<SInventory> GetModuleInventory(SPlayer player);
        Task<bool?> CanAccessModuleInventory(SPlayer player, SInventory inventory);
    }
}
