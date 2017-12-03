using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using snake_game.Bonuses;
using snake_game.MainGame;

namespace Snake
{
    public class Config : IPluginConfig
    {
        public bool IsEnabled { get; set; }
        public int DamageTimeout = 1000;
        public string ControlType = "small";
        public int? TurnSize = 30;
        public int Speed = 275; // Pixels per second
        public int CircleSize = 40; // Radius
        public int CircleOffset = 5;
        public int InitLen = 30;
        [JsonConverter(typeof(HexColorConverter))] public Color DamageColor = Color.WhiteSmoke;
        [JsonConverter(typeof(HexColorConverter))] public Color? HeadColor = null;
        [JsonConverter(typeof(HexColorConverter))] public Color[] Colors = null;
    }
}