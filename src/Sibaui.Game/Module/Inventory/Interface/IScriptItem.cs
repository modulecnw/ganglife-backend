using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Module.Inventory.Interface
{
    public interface IScriptItem
    {
        int[] ItemId { get; }
        Task<bool> UseItem(SPlayer player);
    }
}
