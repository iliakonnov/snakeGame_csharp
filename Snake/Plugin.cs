using System;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace Snake
{
    /// <inheritdoc />
    // ReSharper disable once UnusedMember.Global
    public class Plugin : IPlugin
    {
        /// <inheritdoc />
        public string Name { get; } = "Snake";

        /// <inheritdoc />
        public IPluginConfig Config { get; } = new Config();

        /// <inheritdoc />
        public BonusBase GetBonus(IPluginConfig config, Random random, MainGame game)
        {
            return new Bonus((Config) config, game);
        }

        /// <inheritdoc />
        public IConfigPage GetPage(IPluginConfig config)
        {
            return new ConfigPage((Config) config);
        }
    }
}