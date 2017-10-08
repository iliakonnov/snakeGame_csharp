using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace BrickBonus
{
    public class Config : IPluginConfig
    {
        public bool IsEnabled { get; set; }
        public int ChanceTime = 1500;
        public double MoveChance = 0.25;
        public double NewChance = 0.1;
        public int Step = 50;
        [JsonConverter(typeof(HexColorConverter))] public Color BrickColor = Color.OrangeRed;
        public int Size = 25;
    }
}