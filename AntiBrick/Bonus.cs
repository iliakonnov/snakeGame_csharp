using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using snake_game;
using snake_game.Bonuses;
using snake_game.Utils;
using SharpDX;
using Color = Microsoft.Xna.Framework.Color;
using Polygon = snake_game.Utils.Polygon;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace AntiBrick
{
    public class Bonus : BonusBase
    {
        readonly Config _config;
        readonly Random _random;
        Texture2D _texture;
        private Polygon _triangle;
        bool _created = false;

        public Bonus(Config cfg, Random rnd, snake_game.MainGame.MainGame game)
        {
            _config = cfg;
            _random = rnd;
        }

        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            var result = new List<string>();
            if (!plugins.ContainsKey("Snake"))
            {
                result.Add("Snake");
            }
            if (!plugins.ContainsKey("Brick"))
            {
                result.Add("Brick");
            }
            return result;
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
        }

        public override Accessable Update(GameTime gameTime, int fullTime, KeyboardState keyboardState,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var snakePoints = plugins["Snake"].GetListProperty<CircleF>("SnakeCircles");

            if (plugins.ContainsKey("Brick"))
            {
                var brickManager = plugins["Brick"];
                var bricks = brickManager.GetListProperty<object>("Bricks");
                if (
                    bricks.Count >= _config.StartBrickCount &&
                    fullTime % _config.ChanceTime == 0 &&
                    _random.NextDouble() <= _config.NewChance &&
                    !_created
                )
                {
                    var bigHead = snakePoints.First();
                    bigHead.Radius *= 2;
                    do
                    {
                        _triangle = new Polygon(3, _config.Size, new Vector2(
                            _random.Next(_config.Size, size.Width - _config.Size),
                            _random.Next(_config.Size, size.Height - _config.Size)
                        ));
                    } while (_triangle.Intersects(bigHead));
                    _created = true;
                }
                if (_created)
                {
                    if (_triangle.Intersects(snakePoints.First()))
                    {
                        for (var i = 0; i < bricks.Count - 1; i += 2)
                        {
                            bricks.RemoveAt(i);
                        }
                        brickManager.SetListProperty("Bricks", bricks);
                        _created = false;
                    }
                }
            }

            return null;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (_created)
            {
                _triangle.PrettyDraw(sb, _config.Color, _config.Thickness);
            }
        }
    }
}