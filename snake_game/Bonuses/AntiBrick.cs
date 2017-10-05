using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using snake_game.MainGame;
using snake_game.Snake;

namespace snake_game.Bonuses
{
    public class AntiBrickManager : IBonusManager
    {
        readonly Config.BonusConfigClass.AntiBrickConfigClass _config;
        readonly Random _random;
        Texture2D _texture;
        private Polygon _triangle;
        bool _created = false;

        public AntiBrickManager(Config.BonusConfigClass.AntiBrickConfigClass cfg, Random rnd, MainGame.MainGame game)
        {
            _config = cfg;
            _random = rnd;
        }

        public string Name => "antibrick";

        public void LoadContent(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _texture.SetData(new[] {Color.White});
        }

        public void Update(GameTime gameTime, int fullTime, IBonusManager[] bonuses, CircleF[] snakePoints,
            Rectangle size)
        {
            var brickManagers = bonuses.Where(x => x.Name == "brick").ToArray();
            if (brickManagers.Length != 0)
            {
                var brickManager = (BrickManager) brickManagers.First();
                if (
                    brickManager.Bricks.Count >= _config.StartBrickCount &&
                    fullTime % _config.ChanceTime == 0 &&
                    _random.NextDouble() <= _config.NewChance &&
                    !_created
                )
                {
                    var bigHead = snakePoints.First();
                    bigHead.Radius *= 2;
                    do
                    {
                        _triangle = new Polygon(3, _config.Size, new Vector2(
                            _random.Next(_config.Size, size.Width - _config.Size),
                            _random.Next(_config.Size, size.Height - _config.Size)
                        ));
                    } while (_triangle.Intersects(bigHead));
                    _created = true;
                }
                if (_created)
                {
                    if (_triangle.Intersects(snakePoints.First()))
                    {
                        for (var i = 0; i < brickManager.Bricks.Count - 1; i += 2)
                        {
                            brickManager.Bricks.RemoveAt(i);
                        }
                        _created = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (_created)
            {
//                foreach (var seg in new[] {_triangle.AbSeg, _triangle.BcSeg, _triangle.AcSeg})
//                {
//                    sb.DrawLine(new Vector2(seg.A.X, seg.A.Y), new Vector2(seg.B.X, seg.B.Y), _config.Color, _config.Thickness);
//                }
                _triangle.PrettyDraw(sb, _config.Color, _config.Thickness);
                // sb.DrawPolygon(Vector2.Zero, _triangle.ToPolygonF(), _config.Color, _config.Thickness);
            }
        }
    }
}