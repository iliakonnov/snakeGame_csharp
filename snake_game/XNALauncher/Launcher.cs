using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace snake_game.XNALauncher
{
    class Launcher : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        MainGame.MainGame _game;
        Menu _menu;
        MouseState _lastMouseState;
        bool _gameStarted = false;
        MainGame.Config _config;

        public Launcher(MainGame.Config config)
        {
            _graphics = new GraphicsDeviceManager(this);
            _menu = new Menu(_config);
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            //_menu.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!_gameStarted)
            {
                var currentMouseState = Mouse.GetState();
                if (_lastMouseState == null) _lastMouseState = currentMouseState;

                if (_lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    _game = new MainGame.MainGame(new MainGame.Config(), _graphics, Content);
                    _game.LoadContent(GraphicsDevice);
                    _game.OnExit += (s, e) => Exit();
                    _gameStarted = true;
                }
                _lastMouseState = currentMouseState;

                //_menu.Update(gameTime, Window);
            }
            else _game.Update(gameTime, Window);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_gameStarted)
            {
                _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
                //_menu.Draw(gameTime, GraphicsDevice, _graphics, _spriteBatch, Window);
            }
            else _game.Draw(gameTime, GraphicsDevice, _graphics, _spriteBatch, Window);
            base.Draw(gameTime);
        }
    }
}
