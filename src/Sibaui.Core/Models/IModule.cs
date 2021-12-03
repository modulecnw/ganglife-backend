using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Core.Models
{
    public interface IModule
    {
        string Name { get; }

        Task StartAsync();
        Task StopAsync();
    }
}
