using snake_game.MainGame;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;
using snake_game.Snake;

namespace snake_game.Bonuses
{
	class BrickManager : IBonusManager
	{
	    readonly Random _random;
	    readonly Config.BonusConfigClass.BrickConfigClass _config;
	    int _time;
	    MainGame.MainGame _game;
	    Texture2D _texture;
	    List<BrickBonus> _bricks = new List<BrickBonus>();

	    public string Name => "brick";

	    public BrickManager(Config.BonusConfigClass.BrickConfigClass cfg, Random rnd, MainGame.MainGame game)
	    {
	        _config = cfg;
	        _random = rnd;
	        _game = game;
	    }

	    public void LoadContent(GraphicsDevice graphicsDevice)
	    {
	        _texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
	        _texture.SetData(new[] { Color.White });
	    }

	    public void Update(GameTime gameTime, IBonusManager[] bonuses, CircleF[] snakePoints, Rectangle size)
	    {
	        _time += gameTime.ElapsedGameTime.Milliseconds;
	        if (_time >= _config.ChanceTime)
	        {
	            _time = 0;
	            foreach (var brick in _bricks)
	            {
	                brick.Move(
	                    _random.NextDouble() > _config.MoveChance
	                        ? new Vector2()
	                        : new Vector2(
	                            (float) (_config.Step / 2 - _random.NextDouble() * _config.Step),
	                            (float) (_config.Step / 2 - _random.NextDouble() * _config.Step)
	                        ),
	                    new BagelWorld(size.Height, size.Width)
	                );
	            }
	            if (_random.NextDouble() <= _config.NewChance)
	            {
	                var bigHead = snakePoints.First();
	                bigHead.Radius *= 2;
	                BrickBonus newBrick;
	                do
	                {
	                    newBrick = new BrickBonus(new Vector2(
	                        _random.Next(size.Width), _random.Next(size.Height)
	                    ));
	                } while (bigHead.Intersects(newBrick.GetRectangle(_config.Size)));
	                _bricks.Add(newBrick);
	            }
	        }

	        foreach (var brick in _bricks)
	        {
	            var rect = brick.GetRectangle(_config.Size);
	            rect.X += rect.Width / 2;
	            rect.Y += rect.Height / 2;
	            if (snakePoints.First().Intersects(rect))
	            {
                    _game.Die(1);
	            }
	        }
	    }

	    public void Draw(SpriteBatch sb)
	    {
	        foreach (var brick in _bricks)
	        {
	            sb.Draw(_texture, brick.GetRectangle(_config.Size), _config.BrickColor);
	        }
	    }
	}

    class BrickBonus
    {
        public Vector2 position;  // Left upper corner

        public BrickBonus(Vector2 position)
        {
            this.position = position;
        }

        public Rectangle GetRectangle(int size)
        {
            return new Rectangle(
                (int) position.X, (int) position.Y, size, size
            );
        }

        public void Move(Vector2 move, BagelWorld world)
        {
            position += move;
            var newPos = world.Normalize(new Snake.Point(position.X, position.Y));
            position = new Vector2(newPos.X, newPos.Y);
        }
    }
}
