﻿using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace snake_game.MainGame
{
	public class Config
	{
		public class SnakeConfigClass
		{
			public int Speed = 150;  // Pixels per second
			public int CircleSize = 40;  // Radius
			public int CircleOffset = 5;
			public int InitLen = 30;
		    public Color DamageColor = Color.WhiteSmoke;
			public Color? HeadColor = null;
			public Color[] Colors = null;
		}

		public class ScreenConfigClass
		{
			public bool IsMouseVisible = true;
			public bool IsFullScreen = false;
			public int ScreenWidth = 800;
			public int ScreenHeight = 600;
		}

		public class GameConfigClass
		{
		    public int Lives = 3;
		    public int DamageTimeout = 1500;
		    public int FoodToLive = 10;
		    public Color TextColor = Color.Black;
			public bool DebugShow = false;
			public Color DebugColor = Color.LightGray;
			public bool FogEnabled = true;
			public Tuple<Color,Color> FogColor = new Tuple<Color, Color>(Color.DarkSlateBlue, Color.Transparent);
			public double FogSizeMultiplier = 1.5;
			public Color BackgroundColor = Color.CornflowerBlue;
		}

		public class BonusConfigClass
		{
			public class BonusSettingsClass
			{
				public bool EnableBonuses = true;
				public string[] BonusesEnabled = null;
			}
			public class BrickConfigClass
			{
			    public int ChanceTime = 1500;
			    public double MoveChance = 0.25;
			    public double NewChance = 0.1;
			    public int Step = 50;
				public Color BrickColor = Color.OrangeRed;
				public int Size = 25;
			}
		    public class AppleConfigClass
		    {
		        public int AppleCount = 1;
		        public int Thickness = 10;
		        public int Radius = 25;
		        public int Sides = 30;
		        public int Speed = 100;
		        public Color AppleColor = Color.SpringGreen;
		    }
			public BonusSettingsClass BonusSettings = new BonusSettingsClass();
			public BrickConfigClass BrickConfig = new BrickConfigClass();
		    public AppleConfigClass AppleConfig = new AppleConfigClass();
		}
		public SnakeConfigClass SnakeConfig = new SnakeConfigClass();
		public ScreenConfigClass ScreenConfig = new ScreenConfigClass();
		public GameConfigClass GameConfig = new GameConfigClass();
		public BonusConfigClass BonusConfig = new BonusConfigClass();
	}

	public static class HexColorConverter
	{
		// TODO: Разобраться с json и конвертировать цвета с помощью этого класса.
		public static string ToString(Color color)
		{
			var r = color.R.ToString("X");
			var g = color.G.ToString("X");
			var b = color.B.ToString("X");
			var a = color.A.ToString("X");

			if (a == "FF") a = "";

			if (
				r[0] == r[1] &&
				g[0] == g[1] &&
				b[0] == b[1] &&
				(a[0] == a[1] || a == "")
			)
			{
				r = r[0].ToString();
				g = g[0].ToString();
				b = b[0].ToString();
				if (a != "") a = a[0].ToString();
			}

			return $"#{r}{g}{b}{a}";
		}

		public static Color FromString(string color)
		{
			color = color.TrimStart('#');
			int r, g, b, a;
			switch (color.Length)
			{
				case 3: // rgb
					r = int.Parse(color.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
					g = int.Parse(color.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
					b = int.Parse(color.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);
					a = 255;
					break;
				case 4: // rgba
					r = int.Parse(color.Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
					g = int.Parse(color.Substring(1, 1), System.Globalization.NumberStyles.HexNumber);
					b = int.Parse(color.Substring(2, 1), System.Globalization.NumberStyles.HexNumber);
					a = int.Parse(color.Substring(3, 1), System.Globalization.NumberStyles.HexNumber);
					break;
				case 6: // rrggbb
					r = int.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					g = int.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
					b = int.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
					a = 255;
					break;
				case 8: // rrggbbaa
					r = int.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					g = int.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
					b = int.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
					a = int.Parse(color.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
					break;
				default:
					throw new ArgumentException("Unknown color format");
			}
			return new Color(r, g, b, a);
		}
	}

	public class ConfigLoad
	{
		public static Config Parse(string json)
		{
			return JsonConvert.DeserializeObject<Config>(json);
		}

		public static string Save(Config config)
		{
			return JsonConvert.SerializeObject(config, Formatting.Indented);
		}
	}
}