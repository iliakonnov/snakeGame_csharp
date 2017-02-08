using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace snake_game
{
	public class SnakeClass
	{
		int _length;
		public int length {
			get { return _length; }
			set {
				_length = value;
				snakeParts.Add(new TailPart());
			}
		}

		// Textures
		Texture2D head;
		Texture2D snake;
		Texture2D brick;

		public List<TailPart> snakeParts = new List<TailPart>();
		public Rectangle headRect { get { return snakeParts[0].collisionBox; } }
#if DEBUG
		Texture2D collisionTexture;
#endif

		public SnakeClass(int length)
		{
			this._length = length;
		}

		public void Initialize()
		{
			var startPos = (length + 1)*64;
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
					collisionBox = new Rectangle((startPos - i*64)-32, -32, 64, 64),
					position = new Vector2(startPos - i*64, 0),
					direction = new Vector2(0, 1)  // Down
				});
			}
		}

		public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
		{
			head = Content.Load<Texture2D>("images/head");
			snake = Content.Load<Texture2D>("images/snake");
			brick = Content.Load<Texture2D>("images/brick");
		}

		public void Update(GameTime gameTime, KeyboardState state, Rectangle bounds, float speed)
		{
			Control(state);

			var newParts = new List<TailPart>();
			for (int i = 0; i < snakeParts.Count; i++)
			{
				var item = snakeParts[i];
				if (item.type == TailType.head)  // If head
				{
					var newPos = item.position + item.direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
					newParts.Add(new TailPart {
						rotateRadians = item.rotateRadians,
						type = TailType.head,
						collisionBox = new Rectangle((int)newPos.X-32, (int)newPos.Y-32, 64, 64),
						position = newPos,
						direction = item.direction
					});
				}
				else
				{
					var prev = snakeParts[i - 1];
					newParts.Add(new TailPart
					{
						rotateRadians = prev.rotateRadians,
						type = TailType.tail,
						collisionBox = prev.collisionBox,
						position = prev.position,
						direction = prev.direction
					}
					);
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
					new Vector2(32, 32), 1.0f, SpriteEffects.None, i / 10000
				);
			}
		}
	}
}
