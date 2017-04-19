using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace snake_game.XNALauncher
{
    public class Menu : IMiniGame
    {
        public enum Focus { mainMenu, gameMenu, screenMenu, snakeMenu, bonusMenu }
        bool _maximized = true;
        Focus _focus = Focus.mainMenu;
        MainGame.Config _config;
        Menus.GameMenu _gameMenu;
        Menus.ScreenMenu _screenMenu;
        Menus.SnakeMenu _snakeMenu;
        Menus.BonusMenu _bonusMenu;
        public event EventHandler FocusChange = delegate { };

        public Menu(MainGame.Config config)
        {
            _config = config;
            _gameMenu = new Menus.GameMenu(config.GameConfig);
            _screenMenu = new Menus.ScreenMenu(config.ScreenConfig);
            _snakeMenu = new Menus.SnakeMenu(config.SnakeConfig);
            _bonusMenu = new Menus.BonusMenu(config.BonusConfig);
        }

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _gameMenu.LoadContent(graphicsDevice);
            _screenMenu.LoadContent(graphicsDevice);
            _snakeMenu.LoadContent(graphicsDevice);
            _bonusMenu.LoadContent(graphicsDevice);
        }

        public void Update(GameTime gameTime, GameWindow window)
        {
            switch (_focus)
            {
                case Focus.mainMenu:
                    break;
                case Focus.gameMenu:
                    _gameMenu.Update(gameTime, window);
                    break;
                case Focus.screenMenu:
                    _screenMenu.Update(gameTime, window);
                    break;
                case Focus.snakeMenu:
                    _snakeMenu.Update(gameTime, window);
                    break;
                case Focus.bonusMenu:
                    _bonusMenu.Update(gameTime, window);
                    break;
            }
        }

        public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, GraphicsDeviceManager graphics, SpriteBatch spriteBatch, GameWindow window)
        {
            switch (_focus)
            {
                case Focus.mainMenu:
                    break;
                case Focus.gameMenu:
                    _gameMenu.Draw(gameTime, graphicsDevice, graphics, spriteBatch, window);
                    break;
                case Focus.screenMenu:
                    _screenMenu.Draw(gameTime, graphicsDevice, graphics, spriteBatch, window);
                    break;
                case Focus.snakeMenu:
                    _snakeMenu.Draw(gameTime, graphicsDevice, graphics, spriteBatch, window);
                    break;
                case Focus.bonusMenu:
                    _bonusMenu.Draw(gameTime, graphicsDevice, graphics, spriteBatch, window);
                    break;
            }
            if (_focus == Focus.mainMenu)
            {
                // TODO: Draw menu
                /* =========================
                 * |Bonuses   | Empty or   |
                 * |----------| Logo or    |
                 * |Screen    | Some debug |
                 * |----------| Information|
                 * |Snake     |            |
                 * |----------| With game  |
                 * |Bonuses   | version    |
                 * |========================
                 */
            }
            else
            {
                // TODO: Draw menu
                /* =========================
                 * |≡|   (<-) Apple (->)   |
                 * |-|---------------------|
                 * |B| Some apple bonus    |
                 * |O| configuration       |
                 * |N| parameters          |
                 * |U|---------------------|
                 * |S| (<) page 1 of 2 (>) |
                 * |========================
                 */
            }
        }
    }
}
