﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using snake_game.Bonuses;
using Color = Microsoft.Xna.Framework.Color;
using Polygon = snake_game.Utils.Polygon;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace AntiBrick
{
    public class Bonus : BonusBase
    {
        public readonly Config Config;
        public Polygon Triangle;
        
        readonly Random _random;
        Texture2D _texture;
        bool _created = false;

        public Bonus(Config cfg, Random rnd, snake_game.MainGame.MainGame game)
        {
            Config = cfg;
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

        public override void LoadContent(GraphicsDevice gd)
        {
            _texture = new Texture2D(gd, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
        }

        public override Accessable Update(GameTime time, int fullTime, KeyboardState keyboard, IReadOnlyDictionary<string, BonusBase> plugins, Rectangle size, IReadOnlyDictionary<string, Accessable> events)
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
                        Triangle = new Polygon(3, Config.Size, new Vector2(
                            _random.Next(Config.Size, size.Width - Config.Size),
                            _random.Next(Config.Size, size.Height - Config.Size)
                        ));
                    } while (Triangle.Intersects(bigHead));
                    _created = true;
                }
                if (_created)
                {
                    if (Triangle.Intersects(snakePoints.First()))
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
                Triangle.PrettyDraw(sb, Config.Color, Config.Thickness);
            }
        }
    }
}