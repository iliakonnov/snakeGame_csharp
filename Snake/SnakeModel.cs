using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using snake_game.MainGame;
using snake_game.Utils;

namespace Snake
{
    /// <summary>
    ///     Модель змеи, состоящей из отрезков. Голова может двигаться в любом направлении.
    /// </summary>
    public class SnakeModel
    {
        private const float Epsilon = 1e-6f;

        private Point[] _nodes;

        private SnakeModel()
        {
        }

        /// <inheritdoc />
        /// <param name="head">Позиция головы змеи</param>
        /// <param name="direction">Направление движения змеи</param>
        public SnakeModel(Point head, int direction)
        {
            Length = 0;
            _nodes = new[] {head};
            Direction = direction;
        }

        /// <summary>
        ///     Длина змеи
        /// </summary>
        public float Length { get; private set; }

        /// <summary>
        ///     Точки, задающие повороты змеи. Первый - голова
        /// </summary>
        public IEnumerable<Point> Points => new ReadOnlyCollection<Point>(_nodes);

        /// <summary>
        ///     Направление движения головы в градусах
        /// </summary>
        public float Direction { get; private set; }

        /// <summary>
        ///     Сдвигает всю змею на указанное расстояние по направлению движения змеи
        /// </summary>
        /// <param name="s">Расстояние на скоторое змея продвинется</param>
        /// <returns>Уже сдвинутая змея</returns>
        /// <exception cref="Exception">В случае, если змея не из чего не состоит</exception>
        public SnakeModel ContinueMove(float s)
        {
            if (_nodes.Length == 0)
                throw new Exception();

            var head = _nodes[0];
            var pt = Point.FromPolar(Direction, s);
            if (_nodes.Length == 1)
                return new SnakeModel
                {
                    _nodes = new[] {head.Add(pt)},
                    Length = 0,
                    Direction = Direction
                };

            var nodes = _nodes.ToArray();
            nodes[0] = head.Add(pt);
            return SetLength(Length, nodes, Direction);
        }

        /// <summary>
        ///     Поворачивает змею на указанныый угол
        /// </summary>
        /// <param name="alpha">Угол на который надо повернуть змею в градусах</param>
        /// <returns>Уже повернутая змея</returns>
        public SnakeModel Turn(float alpha)
        {
            if (Math.Abs(alpha) <= Epsilon) return this;
            var nodes = _nodes;
            if (_nodes.Length > 1)
            {
                var head = _nodes[0];
                nodes = head.Equals(_nodes[1])
                    ? _nodes
                    : new[] {head}.Concat(_nodes).ToArray();
            }

            var dir = (int) (Direction + alpha) % 360;
            if (dir < 0) dir += 360;
            return new SnakeModel
            {
                _nodes = nodes,
                Length = Length,
                Direction = dir
            };
        }

        /// <summary>
        ///     Устанавливает змее определенный угол движения
        /// </summary>
        /// <param name="degrees">Угол, на котороый должна быть повернута змея в градусах</param>
        /// <returns>Уже повернутая змея</returns>
        public SnakeModel TurnAt(float degrees)
        {
            return Turn(degrees - Direction);
        }

        /// <summary>
        ///     Увеличивает змею на указанную длину
        /// </summary>
        /// <param name="s">На сколько нужно увеличить змею</param>
        /// <returns>Уже удлинённая змея</returns>
        public SnakeModel Increase(float s)
        {
            return SetLength(Length + s, _nodes, Direction);
        }

        /// <summary>
        ///     Уменьшает змею на указанную длину
        /// </summary>
        /// <param name="s">На сколько нужно уменьшить змею</param>
        /// <returns>Уже укороченная змея</returns>
        public SnakeModel Decrease(float s)
        {
            if (Length < s) throw new ArgumentException();
            if (Math.Abs(Length - s) < Epsilon)
                return new SnakeModel
                {
                    _nodes = new[] {_nodes[0]},
                    Length = 0,
                    Direction = Direction
                };

            return SetLength(Length - s, _nodes, Direction);
        }

        /// <summary>
        ///     Разбивает змею на точки, кадая на указанном расстоянии от другой
        /// </summary>
        /// <param name="pointDistance">Расстояние между точками</param>
        /// <returns>Массив точек</returns>
        public Point[] GetSnakeAsPoints(float pointDistance)
        {
            var result = new Point[(int) (Length / pointDistance)];
            var n = 0;
            result[n++] = _nodes[0];
            var skip = 0f;
            for (var i = 0; i < _nodes.Length - 1; i++)
            {
                Point[] points;
                (points, skip) = new Segment(_nodes[i], _nodes[i + 1]).AsSetOfPoints(pointDistance, skip);
                foreach (var item in points)
                    if (n < result.Length)
                        result[n++] = item;
            }

            if (result[result.Length - 1] == null) result[result.Length - 1] = result[result.Length - 2];

            return result;
        }

        /// <summary>
        ///     Нормализует змею, чтобы она переходила из одного края тороидального мира в другой
        /// </summary>
        /// <param name="world">Тороидальный мир</param>
        /// <returns>Отрезки, из которых состоит уже нормализованная змея</returns>
        // ReSharper disable once UnusedMember.Global
        public Segment[] Normalize(BagelWorld world)
        {
            return world.Normalize(Points.ToArray());
        }

        internal static SnakeModel SetLength(float length, Point[] nodes, float headDirection)
        {
            switch (nodes.Length)
            {
                case 0:
                    throw new Exception();
                case 1:
                    var head = nodes[0];
                    var pt = Point.FromPolar(headDirection, -length);
                    return new SnakeModel
                    {
                        _nodes = new[] {head, head.Add(pt)},
                        Length = length,
                        Direction = headDirection
                    };
                default:
                    if (length <= 0) throw new ArgumentException();

                    break;
            }


            int i;
            float c;
            Segment tail = null;
            for (i = 1, c = 0; i < nodes.Length && c < length; i++)
            {
                tail = new Segment(nodes[i], nodes[i - 1]);
                c += tail.Length;
            }

            if (tail == null) throw new NullReferenceException();

            var newNodes = nodes.Take(i).ToArray();

            if (Math.Abs(c - length) < Epsilon)
                return new SnakeModel
                {
                    _nodes = newNodes,
                    Length = c,
                    Direction = headDirection
                };

            {
                // надо укоротить или удлинить
                var pt = tail.MoveFromAtoB(c - length);
                newNodes[newNodes.Length - 1] = pt;
                return new SnakeModel
                {
                    _nodes = newNodes,
                    Length = length,
                    Direction = headDirection
                };
            }
        }
    }
}