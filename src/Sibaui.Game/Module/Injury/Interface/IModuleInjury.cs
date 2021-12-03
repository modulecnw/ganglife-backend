using Sibaui.Game.Client.Data.Garage;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Module.Injury.Interface
{
    public interface IModuleInjury
    {
        string Name { get; }
        void OnPlayerRevive(SPlayer player);
        //void OnPlayerDeath(SPlayer player, SPlayer killer, uint reason);
    }
}
