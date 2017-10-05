using Microsoft.Xna.Framework;
using snake_game.Bonuses;

namespace AntiAppleBonus
{
    public class Config : IPluginConfig
    {
        public bool IsEnabled { get; set; }
        public int StartSnakeLength = 10;
        public int ChanceTime = 7000;
        public double NewChance = 0.95;
        public Color Color = Color.LightYellow;
        public int Size = 25;
        public float Thickness = 10f;
    }
}