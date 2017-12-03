using System;
using System.Collections.Generic;
using Eto.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace snake_game.Bonuses
{
    public abstract class Accessable
    {
        public virtual TResult GetMethodResult<TResult>(string methodName, object[] arguments = null)
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

    public abstract class BonusBase : Accessable
    {
        public abstract IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins);

        public abstract void LoadContent(GraphicsDevice graphicsDevice);

        public abstract Accessable Update(GameTime gameTime, int fullTime, KeyboardState keyboardState,
            IReadOnlyDictionary<string, BonusBase> plugins,
            Rectangle size, IReadOnlyDictionary<string, Accessable> events);

        public abstract void Draw(SpriteBatch sb);
    }

    public interface IPluginConfig
    {
        bool IsEnabled { get; set; }
    }

    public interface IPlugin
    {
        string Name { get; }
        IPluginConfig Config { get; }
        BonusBase GetBonus(object config, Random random, MainGame.MainGame game);
        IConfigPage GetPage(object config);
    }

    public interface IConfigPage
    {
        IPluginConfig GetConfig();
        TabPage GetPage();
    }

    public class PluginNotFoundException : Exception
    {
        public readonly string NeededName;
        public string SelfName;

        public PluginNotFoundException(string neededName, string selfName)
        {
            NeededName = neededName;
            SelfName = selfName;
        }

        public PluginNotFoundException(string neededName)
        {
            NeededName = neededName;
            SelfName = null;
        }

        public override string ToString()
        {
            return SelfName != null
                ? $"Plugin '{SelfName}' cannot find plugin '{NeededName}'"
                : $"Cannot find plugin '{NeededName}'";
        }
    }
}