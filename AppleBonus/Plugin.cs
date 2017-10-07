using System;
using Eto.Forms;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace snake_plugins.AppleBonus
{
    public class Plugin : IPlugin
    {
        public string Name => "Apple";
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