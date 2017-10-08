using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game;
using snake_game.Bonuses;

namespace AntiBrick
{
    public class Bonus : IBonus
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

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
        }

        public override void Update(GameTime gameTime, int fullTime, Dictionary<string, IBonus> plugins, CircleF[] snakePoints,
            Rectangle size)
        {
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