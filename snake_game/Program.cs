using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace snake_game
{
    public static class Program
    {
        public static void Main()
        {
            #if RELEASE
            try
            {
            #endif
                var plugins = BonusLoader.LoadPlugins(".");
                var config = File.Exists("config.json")
                    ? ConfigLoad.Parse(File.ReadAllText("config.json"), plugins)
                    : new Config
                    {
                        BonusConfig = plugins.ToDictionary(kv => kv.Key, kv => kv.Value.Plugin.Config)
                    };
                new MainGame.MainGame(config, plugins.ToDictionary(kv => kv.Key, kv => kv.Value.Plugin)).Run(GameRunBehavior.Synchronous);
            #if RELEASE
            }
            catch (Exception e)
            {
                using (var writer = new StreamWriter(@"log.log", true))
                {
                    writer.WriteLine(e);
                    writer.WriteLine(e.Message);
                    writer.WriteLine(Environment.StackTrace);
                }
                throw;
            }
            #endif
        }
    }
}