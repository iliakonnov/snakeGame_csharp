using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace snake_game
{
	public class SnakeClass
	{
		int length;

		// Textures
		Texture2D head;
		Texture2D snake;
		Texture2D brick;
		Texture2D apple;

		public List<TailPart> snakeParts = new List<TailPart>();
		public Rectangle headRect { get { return snakeParts[0].collisionBox; } }

		public SnakeClass(int length)
		{
			this.length = length;
		}

		public void Initialize()
		{
			var startPos = (length + 1) * 64;
			// Add head
			snakeParts.Add(new TailPart
			{
				rotateRadians = MathHelper.ToRadians(270),
				type = TailType.head,
				collisionBox = new Rectangle(0, 0, 64, 64),
				position = new Vector2(startPos, 0),
				direction = new Vector2(0, 1)  // Down
			});
			// Add tail
			for (int i = 1; i <= length; i++)
			{
				snakeParts.Add(new TailPart
				{
					rotateRadians = MathHelper.ToRadians(270),
					type = TailType.tail,
					collisionBox = new Rectangle(startPos - 64 * i, 0, 64, 64),
					position = new Vector2(startPos - 64 * i, 0),
					direction = new Vector2(0, 1)  // Down
				});
			}
		}

		public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
		{
			head = Content.Load<Texture2D>("images/head");
			snake = Content.Load<Texture2D>("images/snake");
			brick = Content.Load<Texture2D>("images/brick");
			apple = Content.Load<Texture2D>("images/apple");
		}

		#region update
		public void Update(GameTime gameTime, KeyboardState state, Rectangle bounds, float speed)
		{
			Control(state);

			// Move the sprite by speed, scaled by elapsed time.
			var newParts = new List<TailPart>();
			for (int i = 0; i < snakeParts.Count; i++)
			{
				var item = snakeParts[i];
				if (item.type == TailType.head)  // If head
				{
					newParts.Add(item);
					newParts[newParts.Count - 1]
						.position += item.direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					newParts[newParts.Count - 1].collisionBox = new Rectangle(
						(int)newParts[newParts.Count - 1].position.X, (int)newParts[newParts.Count - 1].position.Y, 64, 64
					);
				}
				else
				{
					newParts.Add(item);
					newParts[newParts.Count - 1].position = snakeParts[i - 1].position;
					newParts[newParts.Count - 1].rotateRadians = snakeParts[i - 1].rotateRadians;
					newParts[newParts.Count - 1].collisionBox = snakeParts[i - 1].collisionBox;
				}
			}
			snakeParts = newParts;

			Toroidate(bounds);
		}
		void Control(KeyboardState state)
		{
			int RotationAngle;
			Vector2 snakeSpeed;
			if (state.IsKeyDown(Keys.Left))
			{
				RotationAngle = 0;
				snakeSpeed = new Vector2(-1, 0);
			}
			else if (state.IsKeyDown(Keys.Right))
			{
				RotationAngle = 180;
				snakeSpeed = new Vector2(1, 0);
			}
			else if (state.IsKeyDown(Keys.Up))
			{
				RotationAngle = 90;
				snakeSpeed = new Vector2(0, -1);
			}
			else if (state.IsKeyDown(Keys.Down))
			{
				RotationAngle = 270;
				snakeSpeed = new Vector2(0, 1);
			}
			else
			{
				return;
			}
			snakeParts[0].rotateRadians = MathHelper.ToRadians(RotationAngle);
			snakeParts[0].direction = snakeSpeed;
		}
		void Toroidate(Rectangle bounds)
		{
			foreach (var item in snakeParts)
			{
				int width = bounds.Width;
				int height = bounds.Height;
				if (item.position.X > width) item.position.X -= width;
				if (item.position.X < 0) item.position.X = width - item.position.X;
				if (item.position.Y > height) item.position.Y -= height;
				if (item.position.Y < 0) item.position.Y = height - item.position.Y;
			}
		}
		#endregion

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			Texture2D texture;
			TailPart item;
			for (int i = snakeParts.Count - 1; i >= 0; i--)
			{
				item = snakeParts[i];
				texture = item.type == TailType.head ? head : snake;
				spriteBatch.Draw(
					texture, item.position, null, Color.White, (float)item.rotateRadians,
					new Vector2(32, 32), 1.0f, SpriteEffects.None, i / 10
				);
			}
		}
	}
}
