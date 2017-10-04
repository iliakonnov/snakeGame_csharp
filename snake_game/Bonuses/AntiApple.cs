using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using Newtonsoft.Json;

namespace snake_game.Bonuses
{
    public class AntiAppleManager : IBonusManager
    {   
        readonly MainGame.Config.BonusConfigClass.AntiAppleConfigClass _config;
        Polygon _hex;
        public string Name => "antiapple";

        public void Draw(SpriteBatch sb)
        {
            sb.DrawPolygon(Vector2.Zero, _hex.ToPolygonF(), Color.AliceBlue);
            sb.DrawPoint(100, 100, Color.Aqua, 5);
        }

        public AntiAppleManager(MainGame.Config.BonusConfigClass.AntiAppleConfigClass config, Random rnd, Game game)
        {
            _config = config;
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _hex = new Polygon(6, _config.Size, new Vector2(100, 100));
        }

        public void Update(GameTime gameTime, int fullTime, IBonusManager[] bonuses, CircleF[] snakePoints, Rectangle size)
        {
            foreach (var vector in _hex.Vertices)
            {
            }
            // throw new NotImplementedException();
        }
    }
}
