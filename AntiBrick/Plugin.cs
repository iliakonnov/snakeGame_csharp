using System;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace AntiBrick
{
    public class Plugin : IPlugin
    {
        public string Name => "AntiBrick";
        
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