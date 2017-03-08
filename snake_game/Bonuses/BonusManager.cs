﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;

namespace snake_game.Bonuses
{
    public class BonusManager
    {
        Config.BonusConfigClass _config;
        Random _rnd;
        readonly IBonusManager[] _bonuses;
        public BonusManager(Config.BonusConfigClass config, MainGame.MainGame game)
        {
            if (config.BonusSettings.EnableBonuses)
            {
                _config = config;
                _rnd = new Random();
                var bonuses = new List<IBonusManager>();
                var bonusesEnabled =
                    config.BonusSettings.BonusesEnabled == null ||
                    config.BonusSettings.BonusesEnabled.Length == 0
                        ? new[] {"brick", "apple"}
                        : config.BonusSettings.BonusesEnabled;
                foreach (var bonus in bonusesEnabled)
                {
                    switch (bonus)
                    {
                        case "brick":
                            bonuses.Add(new BrickManager(_config.BrickConfig, _rnd, game));
                            break;
                        case "apple":
                            bonuses.Add(new AppleManager(_config.AppleConfig, _rnd, game));
                            break;
                        default:
                            throw new ArgumentException($"Unknown bonus: {bonus}");
                    }
                }
                _bonuses = bonuses.ToArray();
            }
            else
            {
                _bonuses = new IBonusManager[] { };
            }
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            foreach (var bonus in _bonuses)
            {
                bonus.LoadContent(graphicsDevice);
            }
        }

        public void Update(GameTime gameTime, CircleF snakeHead, Rectangle size)
        {
            foreach (var bonus in _bonuses)
            {
                bonus.Update(gameTime, _bonuses, snakeHead, size);
            }
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (var bonus in _bonuses)
            {
                bonus.Draw(sb);
            }
        }
    }
}