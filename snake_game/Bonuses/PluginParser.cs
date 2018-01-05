using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace snake_game.Bonuses
{
    /// <summary>
    ///     Содержит всё необходимое для работы с настройками плагина
    /// </summary>
    public interface IPluginContainer
    {
        /// <summary>
        ///     Сам плагин
        /// </summary>
        IPlugin Plugin { get; }

        /// <summary>
        ///     Загружает параметры плагина из JSON
        /// </summary>
        /// <param name="config">Объект в JSON, соответсвующий параметрам этого плагина</param>
        /// <param name="serializer"></param>
        /// <returns>Десериализованные параметры плагина</returns>
        IPluginConfig LoadConfig(JObject config, JsonSerializer serializer);

        /// <summary>
        ///     Сериализует параметры плагина в объект JSON
        /// </summary>
        /// <param name="config">Текущие параметры плагина</param>
        /// <param name="serializer"></param>
        /// <returns>Сериализованные параметры плагина</returns>
        JObject SaveConfig(IPluginConfig config, JsonSerializer serializer);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Предназначен для плагинов на .NET
    /// </summary>
    public class NetPluginContainer : IPluginContainer
    {
        private readonly Assembly _assembly;

        /// <inheritdoc />
        /// <param name="plugin">Экземпляр плагина</param>
        /// <param name="assembly">Откуда этот плагин был загружен</param>
        public NetPluginContainer(IPlugin plugin, Assembly assembly)
        {
            Plugin = plugin;
            _assembly = assembly;
        }

        /// <inheritdoc />
        public IPlugin Plugin { get; }

        /// <inheritdoc />
        public IPluginConfig LoadConfig(JObject config, JsonSerializer serializer)
        {
            var typeName = (string) config["_type"];
            IPluginConfig pluginConfig = null;
            var type = SearchType(typeName);
            if (type != null && typeof(IPluginConfig).IsAssignableFrom(type))
                pluginConfig = (IPluginConfig) config.ToObject(type, serializer);
            return pluginConfig;
        }

        /// <inheritdoc />
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

    /// <inheritdoc />
    /// <summary>
    ///     Предназначен для плагинов на IronPython
    /// </summary>
    public class PythonPluginContainer : IPluginContainer
    {
        /// <inheritdoc />
        /// <param name="plugin">Экземпляр плагина</param>
        public PythonPluginContainer(IPlugin plugin)
        {
            Plugin = plugin;
        }

        /// <inheritdoc />
        public IPlugin Plugin { get; }

        /// <inheritdoc />
        public IPluginConfig LoadConfig(JObject config, JsonSerializer serializer)
        {
            var conf = (IPythonPluginConfig) Plugin.Config;
            conf.Deserialize(config.ToObject<Dictionary<string, string>>());
            return conf;
        }

        /// <inheritdoc />
        public JObject SaveConfig(IPluginConfig config, JsonSerializer serializer)
        {
            var val = ((IPythonPluginConfig) config).Serialize();
            var newValue = JObject.FromObject(val, serializer);
            return newValue;
        }
    }
}