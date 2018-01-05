using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using snake_game.Bonuses;
using snake_game.Utils;
using Point = snake_game.Utils.Point;

namespace AntiBrick
{
    /// <inheritdoc />
    public class Bonus : BonusBase
    {
        private readonly Random _random;

        /// <summary>
        ///     НАстройки бонуса
        /// </summary>
        public readonly Config Config;

        private bool _created;
        private Texture2D _texture;

        private StarFactory _triangleFactory;

        /// <summary>
        ///     Треугольник бонуса
        /// </summary>
        public Star Triangle;

        /// <inheritdoc />
        public Bonus(Config cfg, Random rnd)
        {
            Config = cfg;
            _random = rnd;
        }

        /// <inheritdoc />
        public override IEnumerable<string> CheckDependincies(IReadOnlyDictionary<string, BonusBase> plugins)
        {
            var result = new List<string>();
            if (!plugins.ContainsKey("Snake")) result.Add("Snake");

            if (!plugins.ContainsKey("Brick")) result.Add("Brick");

            return result;
        }

        /// <inheritdoc />
        public override void LoadContent(GraphicsDevice gd)
        {
            _texture = new Texture2D(gd, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
            _triangleFactory = new StarFactory(3, 1, Config.Size, gd);
        }

        /// <inheritdoc />
        public override Accessable Update(GameTime time, int fullTime, KeyboardState keyboard,
            IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size,
            IReadOnlyDictionary<string, Accessable> events)
        {
            var snakePoints = plugins["Snake"].GetListProperty<CircleF>("SnakeCircles");

            if (plugins.ContainsKey("Brick"))
            {
                var brickManager = plugins["Brick"];
                var bricks = brickManager.GetListProperty<object>("Bricks");
                if (
                    bricks.Count >= Config.StartBrickCount &&
                    fullTime % Config.ChanceTime == 0 &&
                    _random.NextDouble() <= Config.NewChance &&
                    !_created
                )
                {
                    var bigHead = snakePoints.First();
                    bigHead.Radius *= 2;
                    do
                    {
                        Triangle = _triangleFactory.GetStar(
                            new Point(
                                _random.Next(Config.Size, size.Width - Config.Size),
                                _random.Next(Config.Size, size.Height - Config.Size)
                            )
                        );
                    } while (Triangle.Intersects(bigHead));

                    _created = true;
                }

                if (_created)
                    if (Triangle.Intersects(snakePoints.First()))
                    {
                        for (var i = 0; i < bricks.Count - 1; i += 2) bricks.RemoveAt(i);

                        brickManager.SetListProperty("Bricks", bricks);
                        _created = false;
                    }
            }

            return null;
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch sb)
        {
            if (_created) Triangle.BorderDraw(sb, Config.Color, Config.Thickness);
        }
    }
}