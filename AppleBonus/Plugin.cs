﻿using System;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace AppleBonus
{
    /// <inheritdoc />
    // ReSharper disable once UnusedMember.Global
    public class Plugin : IPlugin
    {
        /// <inheritdoc />
        public string Name => "Apple";

        /// <inheritdoc />
        public IPluginConfig Config { get; set; } = new Config();

        /// <inheritdoc />
        public BonusBase GetBonus(IPluginConfig config, Random random, MainGame game)
        {
            return new Bonus((Config) config, random, game);
        }

        /// <inheritdoc />
        public IConfigPage GetPage(IPluginConfig config)
        {
            return new ConfigPage((Config) config);
        }
    }
}