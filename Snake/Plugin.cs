using System;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace Snake
{
    public class Plugin : IPlugin
    {
        public string Name { get; } = "Snake";
        public IPluginConfig Config { get; } = new Config();
        
        public BonusBase GetBonus(object config, Random random, MainGame game)
        {
            return new Bonus((Config) config, game);
        }

        public IConfigPage GetPage(object config)
        {
            return new ConfigPage((Config) config);
        }
    }
}