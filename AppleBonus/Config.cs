using Microsoft.Xna.Framework;
using snake_game.Bonuses;

namespace AppleBonus
{
    public class Config : IPluginConfig
    {
        public bool IsEnabled { get; set; }
        public float BounceTimeout = 150f;
        public int AppleCount = 1;
        public float Thickness = 10f;
        public int Radius = 25;
        public int Sides = 30;
        public int Speed = 100;
        public Color AppleColor = Color.SpringGreen;
    }
}