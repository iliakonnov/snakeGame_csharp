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
            try
            {
                var plugins = BonusLoader.LoadPlugins(".");
                var config = File.Exists("config.json")
                    ? ConfigLoad.Parse(File.ReadAllText("config.json"), plugins.Values.Select(p=>p.GetType().Assembly).ToArray())
                    : new Config
                    {
                        BonusConfig = plugins.ToDictionary(kv => kv.Key, kv => kv.Value.Config)
                    };
                new MainGame.MainGame(config, plugins).Run(GameRunBehavior.Synchronous);
            }
            catch (Exception e)
            {
                using (var writer = new StreamWriter(@"log.log", true))
                {
                    writer.WriteLine(e.Message);
                    writer.WriteLine(Environment.StackTrace);
                }
                throw;
            }
        }
    }
}