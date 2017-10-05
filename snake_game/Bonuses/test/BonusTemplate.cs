using System.Net;
using Eto.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace snake_game.Bonuses.test
{
    public interface IBonus
    {
        void LoadContent(GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime, int fullTime, IBonus[] bonuses, CircleF[] snakePoints, Rectangle size);
        void Draw(SpriteBatch sb);
    }
    
    public interface IPlugin
    {
        string Name { get; }
        object GetConfig();
        IBonus GetBonus(object config);
        TabPage GetPage(object config);
    }
    
    public class Config
    {
        // Тут разные переменные
    }

    public class Bonus : IBonus
    {
        public Bonus(Config config)
        {
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
        }

        public void Update(GameTime gameTime, int fullTime, IBonus[] bonuses, CircleF[] snakePoints, Rectangle size)
        {
        }

        public void Draw(SpriteBatch sb)
        {
        }
    }

    public class Plugin : IPlugin
    {   
        public string Name => "test bonus";

        public object GetConfig()
        {
            return new Config();
        }

        public IBonus GetBonus(object config)
        {
            return new Bonus((Config)config);
        }

        public TabPage GetPage(object config)
        {
            return new TabPage();
        }
    }
}