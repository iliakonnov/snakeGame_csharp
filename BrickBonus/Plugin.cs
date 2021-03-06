﻿using System;
using Eto.Forms;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace BrickBonus
{
    public class Plugin : IPlugin
    {
        public string Name => "Brick";
        public IPluginConfig Config { get; set; } = new Config();

        public BonusBase GetBonus(object config, Random random, MainGame game)
        {
            return new Bonus((Config)config, random, game);
        }

        public IConfigPage GetPage(object config)
        {
            return new ConfigPage((Config) config);
        }
    }
}