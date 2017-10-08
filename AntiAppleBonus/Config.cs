using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace AntiAppleBonus
{
    public class Config : IPluginConfig
    {
        public bool IsEnabled { get; set; }
        public int StartSnakeLength = 10;
        public int ChanceTime = 7000;
        public double NewChance = 0.95;
        [JsonConverter(typeof(HexColorConverter))] public Color Color = Color.LightYellow;
        public int Size = 25;
        public float Thickness = 10f;
    }
}