using System;
using AntiAppleBonus;
using Eto.Forms;
using snake_game.Bonuses;
using snake_game.MainGame;
using Config = AntiAppleBonus.Config;

namespace BrickBonus
{
    public class Plugin : IPlugin
    {
        public string Name => "AntiApple";
        public IPluginConfig Config { get; set; } = new Config();

        public IBonus GetBonus(object config, Random random, MainGame game)
        {
            return new Bonus((Config)config, random, game);
        }

        public TabPage GetPage(object config)
        {
            return new ConfigPage((Config) config).GetPage();
        }
    }
}