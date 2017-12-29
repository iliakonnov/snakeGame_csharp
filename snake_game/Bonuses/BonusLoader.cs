﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Eto.Forms;
using IronPython.Hosting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses
{
    public static class BonusLoader
    {
        public static Dictionary<string, IPlugin> LoadPlugins(string path)
        {
            var result = new Dictionary<string, IPlugin>();
            // .NET dlls
            foreach (var file in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".dll")))
            {
                Type[] types;
                try
                {
                    var dll = Assembly.LoadFile(Path.GetFullPath(file));
                    types = dll.GetExportedTypes();
                }
                catch (IOException)
                {
                    continue;
                }
                foreach (var type in types.Where(t => t.GetInterfaces().Contains(typeof(IPlugin))))
                {
                    var plugin = (IPlugin) Activator.CreateInstance(type);
                    if (result.ContainsKey(plugin.Name))
                    {
                        throw new Exception(
                            $"Error occured while loading '{file}': Plugin with name '{plugin.Name}' already loaded!"
                        );
                    }
                    result[plugin.Name] = plugin;
                }
            }
            // return result;
            // IronPython
            var engine = Python.CreateEngine();
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(MainGame.MainGame)));  // snake_game
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(Vector2)));  // Microsoft.Xna.Framework
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(MonoGame.Extended.CircleF)));  // Monogame.Extended
            engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(TabPage)));  // Eto.Forms
            foreach (var folder in Directory.EnumerateDirectories(Path.Combine(path, "python")))
            {
                var scope = engine.CreateScope();
                engine.ExecuteFile(Path.Combine(path, folder, "__init__.py"), scope);
                var plugin = engine.Execute<IPlugin>("getPlugin()", scope);
                if (result.ContainsKey(plugin.Name))
                {
                    throw new Exception(
                        $"Error occured while loading python/'{folder}': Plugin with name '{plugin.Name}' already loaded!"
                    );
                }
                result[plugin.Name] = plugin;
            }
            return result;
        }
    }
}