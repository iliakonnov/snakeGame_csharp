using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace snake_game
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		#region region variables
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SnakeClass snake;

		// Config
		int length = 3;
		int score = 0;
		float speed = 0.0f;

		// Other
#if DEBUG
		//FPS
		int total_frames = 0;
		float elapsed_time = 0.0f;
		int fps = 0;
		int showTail = 0;
		SpriteFont font;
#endif

		Random rnd = new Random();

		bool speedChanged = false;

		Rectangle appleRect;
		#endregion

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			Window.AllowUserResizing = true;
			snake = new SnakeClass(length);
			snake.Initialize();
			CreateApple();
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);
#if DEBUG
			font = Content.Load<SpriteFont>("DejaVu Sans Mono");
#endif

			snake.LoadContent(Content);
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
#if DEBUG
			//FPS
			elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (elapsed_time >= 1000.0f)
			{
				fps = total_frames;
				total_frames = 0;
				elapsed_time = 0;
			}
#endif

			KeyboardState state = Keyboard.GetState();

			if (state.IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			snake.Update(gameTime, state, Window.ClientBounds, speed);

			if (snake.headRect.Intersects(appleRect))
			{
				score++;
				length++;
				CreateApple();
			}

			#region speed
			// Control speed
			if (state.IsKeyDown(Keys.Q))
			{
				if (!speedChanged || state.IsKeyDown(Keys.LeftControl)) speed -= 10;
				speedChanged = true;
			}
			else if (state.IsKeyDown(Keys.W))
			{
				if (!speedChanged || state.IsKeyDown(Keys.LeftControl)) speed += 10;
				speedChanged = true;
			}
			else if (state.IsKeyDown(Keys.A))
			{
				if (!speedChanged || state.IsKeyDown(Keys.LeftControl)) speed -= 30;
				speedChanged = true;
			}
			else if (state.IsKeyDown(Keys.S))
			{
				if (!speedChanged || state.IsKeyDown(Keys.LeftControl)) speed += 30;
				speedChanged = true;
			}
			else if (state.IsKeyDown(Keys.Z))
			{
				if (!speedChanged || state.IsKeyDown(Keys.LeftControl)) speed -= 50;
				speedChanged = true;
			}
			else if (state.IsKeyDown(Keys.X))
			{
				if (!speedChanged || state.IsKeyDown(Keys.LeftControl)) speed += 50;
				speedChanged = true;
			}
			else speedChanged = false;

			if (state.IsKeyDown(Keys.OemTilde)) speed = 300;
			#endregion

			#region tail
#if DEBUG
			if (state.IsKeyDown(Keys.D1)) showTail = 0;
			else if (state.IsKeyDown(Keys.D2)) showTail = 1;
			else if (state.IsKeyDown(Keys.D3)) showTail = 2;
			else if (state.IsKeyDown(Keys.D4)) showTail = 3;
			else if (state.IsKeyDown(Keys.D5)) showTail = 4;
			else if (state.IsKeyDown(Keys.D6)) showTail = 5;
			else if (state.IsKeyDown(Keys.D7)) showTail = 6;
			else if (state.IsKeyDown(Keys.D8)) showTail = 7;
			else if (state.IsKeyDown(Keys.D9)) showTail = 8;
			else if (state.IsKeyDown(Keys.D0)) showTail = -1;
#endif
			#endregion
			base.Update(gameTime);
		}

		void CreateApple()
		{
			// TODO: Create apple
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

#if DEBUG
			//FPS
			total_frames++;
			// Debug
			var debugInfo = string.Format(
				"FPS:{0}\n" +
				"Speed: {1}\n" +
				"Score: {2}\n" +
				"Size: {3}x{4}\n",
				new object[] { fps, speed, score, Window.ClientBounds.Width, Window.ClientBounds.Height }
			);
			if (showTail < snake.snakeParts.Count && showTail >= 0)  // Info about part of tail
			{
				var part = snake.snakeParts[showTail];
				char arrow = '+';
				int degrees = (int)MathHelper.ToDegrees((float)snake.snakeParts[showTail].rotateRadians);
				switch (degrees)
				{
					case 0: arrow = '←'; break;
					case 90: arrow = '↑'; break;
					case 180: arrow = '→'; break;
					case 270: arrow = '↓'; break;
					default: arrow = '+'; break;
				}
				debugInfo += string.Format(
					"\n#{0}:{1}\n" +
					"position: ({2:D5} {3:D5})\n" +
					"collision box: ({4:D5} {5:D5}); w:{6:D2} h:{7:D2})\n" +
					"rotate: {8:D3}° ({9})\n",
					new object[] {
					showTail, part.type == TailType.head? "head": "tail",

					(int)part.position.X, (int)part.position.Y,

					part.collisionBox.X, part.collisionBox.Y, part.collisionBox.Width, part.collisionBox.Height,

					(int)MathHelper.ToDegrees((float)part.rotateRadians), arrow
					}
				);
			}
			spriteBatch.DrawString(font, debugInfo, new Vector2(0), Color.Black,
								   0.0f, new Vector2(), 1.0f, SpriteEffects.None, 1.0f);
#endif
			// Draw snake
			snake.Draw(gameTime, spriteBatch);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}