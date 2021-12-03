using Microsoft.Extensions.DependencyInjection;
using Sibaui.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public enum ServiceType
        {
            SINGLETON,
            HOSTEDSERVICE,
            MODULE,
        }

        public static T GetModule<T>(this IServiceProvider provider) where T : IModule
        {
            return (T)provider.GetServices<IModule>().FirstOrDefault(n => n.GetType() == typeof(T));
        }

        public static void BindAllClassesFrom<T>(this IServiceCollection services, ServiceType serviceType) where T : class
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                if (serviceType == ServiceType.MODULE)
                {
                    services.AddSingleton(typeof(T), type);

                    services.AddTransient(type);
                }
                else if (serviceType == ServiceType.SINGLETON)
                {
                    services.AddSingleton(type);
                    services.AddSingleton(typeof(T), type);
                }
                else if (serviceType == ServiceType.HOSTEDSERVICE)
                {
                    // add soon
                    //services.AddHostedService(type);
                }
            }
        }
    }
}
