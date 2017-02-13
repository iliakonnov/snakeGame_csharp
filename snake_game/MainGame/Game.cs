using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using snake_game.Snake;

namespace snake_game.MainGame
{
    public class MainGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        SnakeModel _snake;
        SnakeModel _newSnake;
        BagelWorld _world;
        LineDrawer _lineD;
        Controller _ctrl;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _lineD = new LineDrawer(5, Color.Blue, GraphicsDevice);
			_snake = new SnakeModel(new Snake.Point(400, 150), 0).Increase(300);
            _ctrl = new Controller(30);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var control = _ctrl.Control(Keyboard.GetState());

            if (control.IsExit)
            {
                Exit();
            }
            if (control.Turn.ToTurn)
            {
                if (control.Turn.ReplaceTurn)
                {
                    _snake = _snake.TurnAt(control.Turn.TurnDegrees);
                }
                else
                {
					_snake = _snake.Turn(control.Turn.TurnDegrees);
                }
            }

            _newSnake = _snake.ContinueMove(150 * gameTime.ElapsedGameTime.Milliseconds/1000);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
			_world = new BagelWorld(Window.ClientBounds.Height, Window.ClientBounds.Width);
			_graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            var snakeSegments = _world.Normalize(_newSnake);

            /*snakeSegments = new[] {
                new Segment(new Snake.Point(00, 480), new Snake.Point(00, 10) ),
				new Segment(new Snake.Point(200, 480), new Snake.Point(200, 10) ),
			};*/
            foreach (var seg in snakeSegments)
            {
                _lineD.DrawLine(_spriteBatch, seg);
            }
            _snake = _newSnake;

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}