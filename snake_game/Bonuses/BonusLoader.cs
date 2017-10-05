using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eto.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses
{
    public interface IBonus
    {
        void LoadContent(GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime, int fullTime, Dictionary<string, IBonus> plugins, CircleF[] snakePoints, Rectangle size);
        void Draw(SpriteBatch sb);
    }
    
    public interface IPluginConfig
    {
        bool IsEnabled { get; set; }
    }

    public interface IPlugin
    {
        string Name { get; }
        IPluginConfig Config { get; set; }
        IBonus GetBonus(object config, Random random, MainGame.MainGame game);
        TabPage GetPage(object config);
    }

    public interface IConfigPage
    {
        object GetConfig();
        TabPage GetPage();
    }

    public static class BonusLoader
    {
        public static Dictionary<string, IPlugin> LoadPlugins(string path)
        {
            var result = new Dictionary<string, IPlugin>();
            foreach (var file in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".dll")))
            {
                var dll = Assembly.LoadFile(Path.Combine(path, file));
                foreach (var type in dll.GetExportedTypes().Where(t => t.GetInterfaces().Contains(typeof(IPlugin))))
                {
                    var plugin = (IPlugin) Activator.CreateInstance(type);
                    if (result.ContainsKey(plugin.Name))
                    {
                        throw new Exception($"Plugin with name '{plugin.Name}' already loaded!");
                    }
                    result[plugin.Name] = plugin;
                }
            }
            return result;
        }
    }
}