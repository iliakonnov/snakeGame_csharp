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
        EquilateralTriangle _triangle;
        bool created = false;

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

        public class EquilateralTriangle
        {
            public readonly Segment AbSeg;
            public readonly Segment BcSeg;
            public readonly Segment AcSeg;

            readonly Line _abLn;
            readonly Line _bcLn;
            readonly Line _acLn;

            readonly Snake.Point _aPoint;
            readonly Snake.Point _bPoint;
            readonly Snake.Point _cPoint;
            public EquilateralTriangle(float size, Vector2 a)
            {
                /*
                      C
                      ^
                     /|\
                    / | \
                   /  |  \
                  /   |   \
                 /____|____\
                A     K     B
                AB = BC = AC = size
                CK = height
                AK = KB = halfSize
                */

                var halfSize = size / 2;
                var height = Math.Sqrt(size * size - halfSize * halfSize);

                _aPoint = new Snake.Point(a.X, a.Y);
                _bPoint = new Snake.Point(a.X + size, a.Y);
                _cPoint = new Snake.Point(a.X + halfSize, (float)(a.Y + height));

                AbSeg = new Segment(_aPoint, _bPoint);
                BcSeg = new Segment(_bPoint, _cPoint);
                AcSeg = new Segment(_aPoint, _cPoint);

                _abLn = _Normalize(MathUtils.StandardLine(_aPoint, _bPoint), _cPoint);
                _bcLn = _Normalize(MathUtils.StandardLine(_bPoint, _cPoint), _aPoint);
                _acLn = _Normalize(MathUtils.StandardLine(_cPoint, _aPoint), _bPoint);
            }

            static Line _Normalize(Line ln, Snake.Point p)
            {
                return ln.Apply(p) < 0
                    ? new Line(ln.A * -1, ln.B * -1, ln.C * -1)
                    : ln;
            }

            public RectangleF GetBoundingRectangle()
            {
                var height = MathUtils.Distance(AbSeg, _cPoint);
                var width = _bPoint.Y - _aPoint.Y;
                return new RectangleF(_aPoint.X, _aPoint.Y, width, height);
            }

            public bool Intersects(float x, float y, float dist)
            {
                var p = new Snake.Point(x, y);
                return MathUtils.Distance(AbSeg, p) <= dist ||
                       MathUtils.Distance(BcSeg, p) <= dist ||
                       MathUtils.Distance(AcSeg, p) <= dist;
            }

            public bool Intersects(float x, float y)
            {
                return Intersects(x, y, 0);
            }

            public bool Intersects(Vector2 point)
            {
                return Intersects(point.X, point.Y, 0);
            }

            public bool Intersects(CircleF circle)
            {
                return Intersects(circle.Center.X, circle.Center.Y, circle.Radius);
            }

            public bool Intersects(RectangleF rect)
            {
                var sides = new[]
                {
                    new Segment(
                        new Snake.Point(rect.X, rect.Y),
                        new Snake.Point(rect.X + rect.Height, rect.Y)
                    ),
                    new Segment(
                        new Snake.Point(rect.X, rect.Y),
                        new Snake.Point(rect.X, rect.Y + rect.Height)
                    ),
                    new Segment(
                        new Snake.Point(rect.X + rect.Width, rect.Y),
                        new Snake.Point(rect.X + rect.Width, rect.Y + rect.Height)
                    ),
                    new Segment(
                        new Snake.Point(rect.X, rect.Y + rect.Height),
                        new Snake.Point(rect.X + rect.Width, rect.Y + rect.Height)
                    )
                };
                foreach (var side in sides)
                {
                    foreach (var segment in new[] {AbSeg, BcSeg, AcSeg})
                    {
                        if (MathUtils.Intersect(side, segment) != null) return true;
                    }
                }
                return false;
            }

            public bool Contains(float x, float y)
            {
                var p = new Snake.Point(x, y);
                return _abLn.Apply(p) > 0 && _bcLn.Apply(p) > 0 && _acLn.Apply(p) > 0;
            }

            public bool Contains(Vector2 point)
            {
                return Contains(point.X, point.Y);
            }
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
                    !created
                )
                {
                    var bigHead = snakePoints.First();
                    bigHead.Radius *= 2;
                    do
                    {
                        _triangle = new EquilateralTriangle(_config.Size, new Vector2(
                            _random.Next(_config.Size, size.Width - _config.Size),
                            _random.Next(_config.Size, size.Height - _config.Size)
                        ));
                    } while (_triangle.Intersects(bigHead));
                    created = true;
                }
                if (created)
                {
                    if (_triangle.Intersects(snakePoints.First()))
                    {
                        for (var i = 0; i < brickManager.Bricks.Count - 1; i += 2)
                        {
                            brickManager.Bricks.RemoveAt(i);
                        }
                        created = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if (created)
            {
                foreach (var seg in new[] {_triangle.AbSeg, _triangle.BcSeg, _triangle.AcSeg})
                {
                    sb.DrawLine(new Vector2(seg.A.X, seg.A.Y), new Vector2(seg.B.X, seg.B.Y), _config.Color, _config.Thickness);
                }
                // sb.DrawCircle(_snakeHead, 10, _config.Color, 10);
            }
        }
    }
}