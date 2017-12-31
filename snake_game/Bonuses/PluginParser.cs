using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using snake_game.MainGame;

namespace snake_game.Bonuses
{
    public interface IPluginContainer
    {    
        IPlugin Plugin { get; }
        IPluginConfig LoadConfig(JObject config, JsonSerializer serializer);
        JObject SaveConfig(IPluginConfig config, JsonSerializer serializer);
    }

    public class NetPluginContainer : IPluginContainer
    {
        public NetPluginContainer(IPlugin plugin, Assembly assembly)
        {
            Plugin = plugin;
            _assembly = assembly;
        }

        private readonly Assembly _assembly;
        public IPlugin Plugin { get; }

        public IPluginConfig LoadConfig(JObject config, JsonSerializer serializer)
        {
            var typeName = (string) config["_type"];
            IPluginConfig pluginConfig = null;
            var type = SearchType(typeName);
            if (type != null && typeof(IPluginConfig).IsAssignableFrom(type))
            {
                pluginConfig = (IPluginConfig) config.ToObject(type, serializer);
            }
            return pluginConfig;
        }

        public JObject SaveConfig(IPluginConfig config, JsonSerializer serializer)
        {
            var type = config.GetType();
            var name = type.FullName;
            var assemblyName = type.Assembly.GetName().Name;
            var typeName = $"{name}, {assemblyName}";
            var newValue = JObject.FromObject(config, serializer);
            newValue["_type"] = typeName;
            return newValue;
        }

        private Type SearchType(string typeName)
        {
            var type = _assembly.GetExportedTypes()
                .FirstOrDefault(t => t.AssemblyQualifiedName != null &&
                                     t.AssemblyQualifiedName.StartsWith(typeName));
            return type;
        }
    }

    public class PythonPluginContainer : IPluginContainer
    {
        private ScriptScope _scope;

        public PythonPluginContainer(IPlugin plugin, ScriptScope scope)
        {
            Plugin = plugin;
            _scope = scope;
        }
        
        public IPlugin Plugin { get; }
        
        public IPluginConfig LoadConfig(JObject config, JsonSerializer serializer)
        {
            var conf = (IPythonPluginConfig) Plugin.Config;
            conf.Deserialize(config.ToObject<Dictionary<string, string>>());
            return conf;
        }

        public JObject SaveConfig(IPluginConfig config, JsonSerializer serializer)
        {
            var val = ((IPythonPluginConfig) config).Serialize();
            var newValue = JObject.FromObject(val, serializer);
            return newValue;
        }
    }
}