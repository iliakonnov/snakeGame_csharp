using System;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace snake_plugins.AntiAppleBonus
{
    public class Plugin : IPlugin
    {
        public string Name => "AntiApple";
        public IPluginConfig Config { get; set; } = new Config();

        public IBonus GetBonus(object config, Random random, MainGame game)
        {
            return new Bonus((Config)config, random, game);
        }

        public IConfigPage GetPage(object config)
        {
            return new ConfigPage((Config) config);
        }
    }
}