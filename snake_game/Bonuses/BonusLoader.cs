using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Eto.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses
{
    public abstract class Gettable
    {
        public virtual TResult GetMethodResult<TResult>(string methodName)
        {
            throw new ArgumentException($"Cannot execute method '{methodName}'");
        }
        
        public virtual TResult GetProperty<TResult>(string propertyName)
        {
            throw new ArgumentException($"Cannot get property '{propertyName}'");
        }
        
        public virtual List<TResult> GetListProperty<TResult>(string propertyName)
        {
            throw new ArgumentException($"Cannot get property '{propertyName}'");
        }

        public virtual void SetProperty(string propertyName, object newValue)
        {
            throw new ArgumentException($"Cannot set property '{propertyName}'");
        }
        
        public virtual void SetListProperty(string propertyName, IEnumerable<object> newValue)
        {
            throw new ArgumentException($"Cannot set property '{propertyName}'");
        }
    }

    public abstract class IBonus : Gettable
    {
        public abstract void LoadContent(GraphicsDevice graphicsDevice);

        public abstract void Update(GameTime gameTime, int fullTime, Dictionary<string, IBonus> plugins,
            CircleF[] snakePoints, Rectangle size);

        public abstract void Draw(SpriteBatch sb);
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
        IConfigPage GetPage(object config);
    }

    public interface IConfigPage
    {
        IPluginConfig GetConfig();
        TabPage GetPage();
    }

    public static class BonusLoader
    {
        public static Dictionary<string, IPlugin> LoadPlugins(string path)
        {
            var result = new Dictionary<string, IPlugin>();
            foreach (var file in Directory.EnumerateFiles(path).Where(f => f.EndsWith(".dll")))
            {
                var dll = Assembly.LoadFile(Path.GetFullPath(file));
                foreach (var type in dll.GetExportedTypes().Where(t => t.GetInterfaces().Contains(typeof(IPlugin))))
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
            return result;
        }
    }
}