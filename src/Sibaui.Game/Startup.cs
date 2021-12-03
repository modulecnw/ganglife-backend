using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sibaui.Core.Extensions;
using Sibaui.Core.Models;
using Sibaui.Database.Context;
using Sibaui.Game.Controller;
using Sibaui.Game.Module.Char;
using Sibaui.Game.Module.Garage;
using Sibaui.Game.Module.Inventory.Interface;
using Sibaui.Game.Module.Login;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static Sibaui.Core.Extensions.ServiceCollectionExtensions;

// made by Dschahannam.
namespace Sibaui.Game
{
    public class Startup
    {
        private readonly Type[] _types;

        private readonly List<Type> _modules;
        private readonly List<Type> _singletons;

        private IConfiguration Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            _types = Assembly.GetExecutingAssembly().GetTypes();
            _modules = new List<Type>();
            _singletons = new List<Type>();
        }

        /// <summary>
        /// Initializes & sorts all classes, modules, singletons & services
        /// </summary>
        /// <param name="services"></param>
        public void InitializeServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole().AddConfiguration(Configuration.GetSection("Logging")));
            services.AddDbContext<SContext>(options => options.UseMySql(Configuration.GetConnectionString("Database"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.1.37-mariadb")));

            /* Service initialization */
            services.BindAllClassesFrom<ISingleton>(ServiceType.SINGLETON);
            services.BindAllClassesFrom<IModule>(ServiceType.MODULE);

            services.BindAllClassesFrom<IModuleInventory>(ServiceType.SINGLETON);

            services.AddHostedService<PlayerService>();
        }
    }
}
