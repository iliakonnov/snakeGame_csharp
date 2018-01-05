using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eto.Forms;
using IronPython.Hosting;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace snake_game.Bonuses
{
    /// <summary>
    ///     Загружает все доступные плагины
    /// </summary>
    public static class BonusLoader
    {
        /// <summary>
        ///     Загружает плагины
        /// </summary>
        /// <param name="path">Путь, где находятся плагины</param>
        /// <returns>Словарь загруженных плагинов с именем плагина в виде ключа</returns>
        /// <exception cref="PluginNameConflictException">В случе, если были загружены плагины с одинаковыми именами.</exception>
        public static Dictionary<string, IPluginContainer> LoadPlugins(string path)
        {
            var result = new Dictionary<string, IPluginContainer>();

            // .NET dlls
            foreach (var file in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".dll")))
            {
                Type[] types;
                Assembly dll;
                try
                {
                    dll = Assembly.LoadFile(Path.GetFullPath(file));
                    types = dll.GetExportedTypes();
                }
                catch (IOException)
                {
                    continue;
                }

                foreach (var type in types.Where(t => t.GetInterfaces().Contains(typeof(IPlugin))))
                {
                    var plugin = (IPlugin) Activator.CreateInstance(type);
                    if (result.ContainsKey(plugin.Name)) throw new PluginNameConflictException(file, plugin);

                    result[plugin.Name] = new NetPluginContainer(plugin, dll);
                }
            }

            // IronPython
            var engine = Python.CreateEngine();
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(MainGame.MainGame))); // snake_game
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(Vector2))); // Microsoft.Xna.Framework
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(CircleF))); // Monogame.Extended
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(TabPage))); // Eto.Forms
            foreach (var folder in Directory.EnumerateDirectories(Path.Combine(path, "python")))
            {
                var scope = engine.CreateScope();
                engine.ExecuteFile(Path.Combine(path, folder, "__init__.py"), scope);
                var neededFiles = engine.Execute<List<string>>("REQUIRED_IMPORTS", scope);
                foreach (var neededFile in neededFiles)
                    engine.ExecuteFile(Path.Combine(path, folder, neededFile), scope);

                var plugin = engine.Execute<IPlugin>("GET_PLUGIN()", scope);
                if (result.ContainsKey(plugin.Name))
                    throw new PluginNameConflictException($"python/'{folder}'", plugin);

                result[plugin.Name] = new PythonPluginContainer(plugin);
            }

            return result;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     В случе, если были загружены плагины с одинаковыми именами.
    /// </summary>
    public class PluginNameConflictException : Exception
    {
        private readonly string _fileName;
        private readonly IPlugin _plugin;

        /// <inheritdoc />
        /// <param name="fileName">Путь к файлу плагина</param>
        /// <param name="plugin">Сам плагин</param>
        public PluginNameConflictException(string fileName, IPlugin plugin)
        {
            _fileName = fileName;
            _plugin = plugin;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Error occured while loading '{_fileName}': Plugin with name '{_plugin.Name}' already loaded!";
        }
    }
}