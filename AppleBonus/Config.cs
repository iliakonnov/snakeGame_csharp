using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace AppleBonus
{
    public class Config : IPluginConfig
    {
        public bool IsEnabled { get; set; } = true;
        public float BounceTimeout = 150f;
        public int AppleCount = 2;
        public float Thickness = 10f;
        public int Radius = 25;
        public int Sides = 30;
        public int Speed = 300;
        [JsonConverter(typeof(HexColorConverter))] public Color AppleColor = Color.SpringGreen;
    }
}