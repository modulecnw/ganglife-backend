using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Sibaui.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Core.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>Serializes the object to a JSON string.</summary>
        /// <returns>A JSON string representation of the object.</returns>
        public static string ToJson(this object value)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            return JsonConvert.SerializeObject(value, settings);
        }
    }
}
