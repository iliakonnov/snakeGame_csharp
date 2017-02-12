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

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _snake = new SnakeModel(new Snake.Point(0.0, 0.0), 0);
            _world = new BagelWorld(Window.ClientBounds.Height, Window.ClientBounds.Width);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            var control = Controller.Control(Keyboard.GetState());
            if (control.IsExit)
            {
                Exit();
            }
            if (control.Turn.ToTurn)
            {
                if (control.Turn.ReplaceTurn)
                {
                    _snake.Direction = control.Turn.TurnDegrees * (Math.PI / 180);
                }
                else
                {
                    _snake.Direction += control.Turn.TurnDegrees * (Math.PI / 180);
                }
            }

            _newSnake = _snake.ContinueMove(300*gameTime.ElapsedGameTime.Seconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            var snakeSegments = _world.
            foreach (var pnt in _newSnake.Points)
            {
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}