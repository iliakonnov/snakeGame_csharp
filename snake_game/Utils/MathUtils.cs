using System;
using System.Collections.Generic;
using System.Linq;

namespace snake_game.Utils
{
    /// <summary>
    ///     Точка/вектор внутри плоскости
    /// </summary>
    public class Point
    {
        private const float Epsilon = 1e-6f;

        /// <inheritdoc />
        /// <param name="x">Координата X</param>
        /// <param name="y">Коородината Y</param>
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Координата X
        /// </summary>
        public float X { get; }

        /// <summary>
        ///     Координата Y
        /// </summary>
        public float Y { get; }

        /// <summary>
        ///     Возвращает точку на основе полярных координат
        /// </summary>
        /// <param name="degree">Угол (в градусах)</param>
        /// <param name="r">Радиус</param>
        /// <returns>Точка на плоскости</returns>
        public static Point FromPolar(float degree, float r)
        {
            degree = (float) (degree / 180 * Math.PI);
            return FromRadians(degree, r);
        }

        /// <summary>
        ///     Возвращает точку на основе полярных координат
        /// </summary>
        /// <param name="degree">Угол (в радианах)</param>
        /// <param name="r">Радиус</param>
        /// <returns>Точка на плоскости</returns>
        public static Point FromRadians(float degree, float r)
        {
            return new Point(r * (float) Math.Cos(degree), r * (float) Math.Sin(degree));
        }

        /// <summary>
        ///     Прибавляет переданный вектор
        /// </summary>
        /// <param name="pt">Вектор</param>
        /// <returns>Результат сложения векторов</returns>
        public Point Add(Point pt)
        {
            return new Point(X + pt.X, Y + pt.Y);
        }

        /// <summary>
        ///     Скалярно умножает вектор на число
        /// </summary>
        /// <param name="k">Число</param>
        /// <returns>Результат скалярного умножения</returns>
        public Point Multiply(float k)
        {
            return new Point(X * k, Y * k);
        }

        /// <summary>
        ///     Умножает на переданный вектор
        /// </summary>
        /// <param name="pt">Вектор</param>
        /// <returns>Результат умножения</returns>
        public float Multiply(Point pt)
        {
            return X * pt.X + Y * pt.Y;
        }

        /// <summary>
        ///     Склдывает два вектора
        /// </summary>
        /// <param name="a">Первый вектор</param>
        /// <param name="b">Второй вектор</param>
        /// <returns>Результат сложения</returns>
        public static Point operator +(Point a, Point b)
        {
            return a.Add(b);
        }

        /// <summary>
        ///     Выитает два вектора
        /// </summary>
        /// <param name="a">Первый вектор</param>
        /// <param name="b">Второй вектор</param>
        /// <returns>Результат вычитания</returns>
        public static Point operator -(Point a, Point b)
        {
            return a + -b;
        }

        /// <summary>
        ///     Находит противоположный вектор
        /// </summary>
        /// <param name="a">Данный вектор</param>
        /// <returns>Противоположный вектор</returns>
        public static Point operator -(Point a)
        {
            return -1f * a;
        }

        /// <summary>
        ///     Скалярно умножает вектор на число
        /// </summary>
        /// <param name="a">Вектор</param>
        /// <param name="k">Число</param>
        /// <returns>Результат скалярного произведения</returns>
        public static Point operator *(Point a, float k)
        {
            return a.Multiply(k);
        }

        /// <summary>
        ///     Скалярно умножает число на вектор
        /// </summary>
        /// <param name="k">Число</param>
        /// <param name="a">Вектор</param>
        /// <returns>Результат скалярного произведения</returns>
        public static Point operator *(float k, Point a)
        {
            return a * k;
        }

        /// <summary>
        ///     Перемножает два вектора между собой
        /// </summary>
        /// <param name="a">Первый вектор</param>
        /// <param name="b">Второй вектор</param>
        /// <returns>Результат произведения</returns>
        public static float operator *(Point a, Point b)
        {
            return a.Multiply(b);
        }

        /// <summary>
        ///     Проверяет две точки на равенство
        /// </summary>
        /// <param name="a">Первая точка</param>
        /// <param name="b">Вторая точка</param>
        /// <returns>Равны ли они</returns>
        public static bool operator ==(Point a, Point b)
        {
            // ReSharper disable once MergeConditionalExpression (так более понятно)
            return ReferenceEquals(a, null)
                ? ReferenceEquals(b, null)
                : a.Equals(b);
        }

        /// <summary>
        ///     Проверяет две точки на неравенство
        /// </summary>
        /// <param name="a">Первая точка</param>
        /// <param name="b">Вторая точка</param>
        /// <returns>Неравны ли они</returns>
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Point);
        }

        /// <summary>
        ///     Проверяет на равенство с данной точкой
        /// </summary>
        /// <param name="obj">Точка, с котрой надо осуществить проверку</param>
        /// <returns>Результат проверки</returns>
        public bool Equals(Point obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Math.Abs(X - obj.X) < Epsilon &&
                   Math.Abs(Y - obj.Y) < Epsilon;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (int) BitConverter.DoubleToInt64Bits(X) ^ (int) BitConverter.DoubleToInt64Bits(Y);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    /// <summary>
    ///     Каноническое уравнение прямой Ax + By + C = 0
    /// </summary>
    public class Line
    {
        // на таком отличии 1.GetHashCode() не замечает разницы 
        private const float Epsilon = 1e-6f;

        private readonly int _hash;

        /// <summary>
        ///     Прямая ax + by + c = 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Line(float a, float b, float c)
        {
            // Гарантирую, что 
            // |a| + |b| = 1
            // a > 0 или
            // a = 0 и b > 0
            var n = Math.Abs(a) + Math.Abs(b);
            if (n < Epsilon)
                throw new ArgumentException();

            if (Math.Abs(a) > Epsilon)
            {
                if (a < 0)
                    n = -n;
            }
            else // if (Math.Abs(a) <= EPSILON)
            {
                if (b < 0)
                    n = -n;
            }

            A = a / n;
            B = b / n;
            C = c / n;
            if (A + C < Epsilon || A + C > Epsilon)
                _hash = (int) BitConverter.DoubleToInt64Bits(A) ^ (int) BitConverter.DoubleToInt64Bits(C);
            else
                _hash = 0;
        }

        /// <summary>
        ///     Параметр A в уравнении прямой Ax + by + c = 0
        /// </summary>
        public float A { get; }

        /// <summary>
        ///     Параметр B в уравнении прямой ax + By + c = 0
        /// </summary>
        public float B { get; }

        /// <summary>
        ///     Параметр C в уравнении прямой ax + by + C = 0
        /// </summary>
        public float C { get; }

        /// <summary>
        ///     Если точка с одной стороны от прямой, то будет возвращено число того же знака, как и любой другой точки с той же
        ///     стороны прямой.
        ///     Если точка с другой стороны от прямой, то будет возвращено симло другого знака.
        ///     Если точка лежит на прямой, то будет возвращен 0.
        /// </summary>
        /// <param name="a">Точка, которую надо проверить</param>
        /// <returns></returns>
        public float Apply(Point a)
        {
            return A * a.X + B * a.Y + C;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(obj, this)) return true;
            return Equals(obj as Line);
        }

        /// <summary>
        ///     Проверяет на равенство с данной линией
        /// </summary>
        /// <param name="ln">Данная линия</param>
        /// <returns>Равны ли они</returns>
        public bool Equals(Line ln)
        {
            if (ReferenceEquals(ln, null)) return false;
            if (ReferenceEquals(ln, this)) return true;

            return Parallels(ln) &&
                   Math.Abs(C - ln.C) <= Epsilon;
        }

        /// <summary>
        ///     Проверяет, параллельна ли линия с данной
        /// </summary>
        /// <param name="ln">Данная линия</param>
        /// <returns>Параллельны ли они</returns>
        public bool Parallels(Line ln)
        {
            if (ReferenceEquals(ln, this)) return true;

            //var D = A * obj.B - obj.A * B;
            //return Math.Abs(D) < EPSILON;

            return Math.Abs(A - ln.A) <= Epsilon &&
                   Math.Abs(B - ln.B) <= Epsilon;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _hash;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{A}x + {B}y + {C} = 0";
        }
    }

    /// <summary>
    ///     Ориентированный отрезок
    /// </summary>
    public class Segment
    {
        /// <inheritdoc />
        /// <param name="a">Начало отрезка</param>
        /// <param name="b">Конец отрезка</param>
        public Segment(Point a, Point b)
        {
            B = b;
            A = a;
        }

        /// <summary>
        ///     Начало отрезка
        /// </summary>
        public Point A { get; }

        /// <summary>
        ///     Конец отрезка
        /// </summary>
        public Point B { get; }

        /// <summary>
        ///     Длина сегмента
        /// </summary>
        public float Length
        {
            get
            {
                var x = A.X - B.X;
                var y = A.Y - B.Y;
                return (float) Math.Sqrt(x * x + y * y);
            }
        }

        /// <summary>
        ///     Возвращает координаты точки A, сдвинутой по направлению к точке B
        /// </summary>
        /// <param name="s">
        ///     На сколько сдвинуть точку A. Если 0, то не изменится, а если 1, то будт совпадать с координатами точки
        ///     B
        /// </param>
        /// <returns>Изменённые координаты точки A</returns>
        public Point MoveFromAtoB(float s)
        {
            var p = s / Length;
            return new Point(
                A.X + p * (B.X - A.X),
                A.Y + p * (B.Y - A.Y)
            );
        }

        /// <summary>
        ///     Разбивает сегмент на точки с данным растоянием между ними
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="skip"></param>
        /// <returns>Массив точек и skip</returns>
        public (Point[], float) AsSetOfPoints(float distance, float skip)
        {
            const float epsilon = 1e-3f;
            var len = Length;
            if (Math.Abs(skip) < epsilon) skip = distance;

            if (len <= skip) return (new Point[0], skip - len);

            var x = A.X + (B.X - A.X) * skip / len;
            var y = A.Y + (B.Y - A.Y) * skip / len;

            var count = (int) ((len - skip) / distance);
            if (Math.Abs(skip + count * distance + distance - len) < epsilon)
            {
                count += 1;
                skip = 0;
            }
            else
            {
                skip = distance - (len - (skip + count * distance));
            }

            var result = new List<Point>(count);

            var p = distance / len;
            var kx = (B.X - A.X) * p;
            var ky = (B.Y - A.Y) * p;

            for (var i = 0; i <= count; i++)
            {
                result.Add(new Point(x, y));
                x += kx;
                y += ky;
            }

            return (result.ToArray(), skip);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as Segment);
        }

        /// <summary>
        ///     Проверяет на равенство с другим отрезком
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(Segment obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return A.Equals(obj.A) && B.Equals(obj.B);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (A.GetHashCode() << 1) ^ B.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{A}; {B}]";
        }
    }

    /// <summary>
    ///     Треугольник
    /// </summary>
    public class Triangle
    {
        /// <inheritdoc />
        /// <param name="a">Координаты первого угла</param>
        /// <param name="b">Координаты второго угла</param>
        /// <param name="c">Координаты третьего угла</param>
        public Triangle(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }

        /// <summary>
        ///     Координаты первого угла
        /// </summary>
        public Point A { get; }

        /// <summary>
        ///     Координаты второго угла
        /// </summary>
        public Point B { get; }

        /// <summary>
        ///     Координаты третьего угла
        /// </summary>
        public Point C { get; }

        /// <summary>
        ///     Проверяет, находится ли точка в пределах или на границах треугольника
        /// </summary>
        /// <param name="point">Точка, которую надо проверить</param>
        /// <returns>Результат проверки</returns>
        public bool Contains(Point point)
        {
            var t1 = Math.Sign((A.X - point.X) * (B.Y - A.Y) - (B.X - A.X) * (A.Y - point.Y));
            var t2 = Math.Sign((B.X - point.X) * (C.Y - B.Y) - (C.X - B.X) * (B.Y - point.Y));
            var t3 = Math.Sign((C.X - point.X) * (A.Y - C.Y) - (A.X - C.X) * (C.Y - point.Y));
            return t1 == 0 ||
                   t2 == 0 ||
                   t3 == 0 ||
                   t1 == t2 && t2 == t3;
        }

        /// <summary>
        ///     Провераяет, находится ли точка на границах треугольника
        /// </summary>
        /// <param name="point">Точка, которую надо проверить</param>
        /// <returns>Результат проверки</returns>
        public bool OnBound(Point point)
        {
            var t1 = Math.Sign((A.X - point.X) * (B.Y - A.Y) - (B.X - A.X) * (A.Y - point.Y));
            var t2 = Math.Sign((B.X - point.X) * (C.Y - B.Y) - (C.X - B.X) * (B.Y - point.Y));
            var t3 = Math.Sign((C.X - point.X) * (A.Y - C.Y) - (A.X - C.X) * (C.Y - point.Y));
            return t1 == 0 ||
                   t2 == 0 ||
                   t3 == 0;
        }

        /// <summary>
        ///     Разбивает тругольник на три его стороны
        /// </summary>
        /// <returns>Три стороны треугольника</returns>
        public Tuple<Segment, Segment, Segment> ToSegments()
        {
            return new Tuple<Segment, Segment, Segment>(
                new Segment(A, B),
                new Segment(B, C),
                new Segment(C, A)
            );
        }

        /// <summary>
        ///     К каждому вектору угла тругольика прибавляет переданный вектор
        /// </summary>
        /// <param name="shift">Переданный вектор</param>
        /// <returns>Треугольник с изменёнными координатами</returns>
        public Triangle Move(Point shift)
        {
            return new Triangle(A.Add(shift), B.Add(shift), C.Add(shift));
        }
    }

    /// <summary>
    ///     Вспомогательные математические функции
    /// </summary>
    public static class MathUtils
    {
        private const float Epsilon = 1e-16f;

        /// <summary>
        ///     Получить стандартную форму прямой, проходящей через заданные точки
        ///     X1 * x + X2 * y + X3 = 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Line StandardLine(Point a, Point b)
        {
            return new Line(a.Y - b.Y, b.X - a.X, a.X * b.Y - b.X * a.Y);
        }

        /// <summary>
        ///     Точка пересечения двух прямых, заданных в стандартной форме.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point Intersect(Line a, Line b)
        {
            var d = a.A * b.B - b.A * a.B;

            var x = a.B * b.C - a.C * b.B;
            var y = a.C * b.A - a.A * b.C;
            return new Point(x / d, y / d);
        }

        /// <summary>
        ///     Находятся ли точки по одну сторону от прямой.
        ///     -1 - по разные стороны
        ///     0 - одна или обе лежат на прямой
        ///     1 - по одну сторону
        /// </summary>
        /// <param name="line"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int AreOnTheSameSide(Line line, Point a, Point b)
        {
            var aa = line.Apply(a);
            if (Math.Abs(aa) < Epsilon) return 0;
            var bb = line.Apply(b);
            if (Math.Abs(bb) < Epsilon) return 0;
            var signA = Math.Sign(aa);
            var signB = Math.Sign(bb);
            return signA * signB;
        }

        /// <summary>
        ///     Пересекаются ли отрезки. Если совпадают более, чем в одной точке,
        ///     то возвращается произвольная точка пересечения.
        ///     Если не пересекаются, то null
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Point Intersect(Segment a, Segment b)
        {
            if (a.A == b.A || a.A == b.B) return a.A;
            if (a.B == b.A || a.B == b.B) return a.B;

            var aXmin = new[] {a.A.X, a.B.X}.Min();
            var aXmax = new[] {a.A.X, a.B.X}.Max();
            var aYmin = new[] {a.A.Y, a.B.Y}.Min();
            var aYmax = new[] {a.A.Y, a.B.Y}.Max();

            var bXmin = new[] {b.A.X, b.B.X}.Min();
            var bXmax = new[] {b.A.X, b.B.X}.Max();
            var bYmin = new[] {b.A.Y, b.B.Y}.Min();
            var bYmax = new[] {b.A.Y, b.B.Y}.Max();

            if (new[] {aXmin, bXmin}.Max() > new[] {aXmax, bXmax}.Min() ||
                new[] {aYmin, bYmin}.Max() > new[] {aYmax, bYmax}.Min())
                // если прямоугольники не пересекаются
                return null;

            var lineA = StandardLine(a.A, a.B);
            var lineB = StandardLine(b.A, b.B);
            if (lineA.Equals(lineB))
            {
                // на одной прямой и прямоугольники пересекаются
                if (a.A.X >= bXmin && a.A.X <= bXmax)
                    // точка a.A внутри b
                    return a.A;

                if (a.B.X >= bXmin && a.B.X <= bXmax)
                    // точка a.B внутри b
                    return a.B;

                // b внутри a
                return b.A;
            }

            if (lineA.Parallels(lineB))
                // параллельные прямые
                return null;

            var sideA = AreOnTheSameSide(lineA, b.A, b.B);
            if (sideA > 0) return null;
            var sideB = AreOnTheSameSide(lineB, a.A, a.B);
            if (sideB > 0) return null;

            if (sideA == 0)
            {
                // b касается a
                if (Math.Abs(lineA.Apply(b.A)) < Epsilon) return b.A;
                return b.B;
            }

            if (sideB == 0)
            {
                // a касается b
                if (Math.Abs(lineB.Apply(a.A)) < Epsilon) return a.A;
                return a.B;
            }

            return Intersect(lineA, lineB);
        }

        /// <summary>
        ///     Расстояние от точки до отрезка.
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static float Distance(Segment seg, Point pt)
        {
            var vecAB = seg.B - seg.A;
            var vecAP = pt - seg.A;
            var t = vecAB * vecAP / (vecAB * vecAB);
            if (t < 0) t = 0;
            if (t > 1) t = 1;
            var s = vecAP - t * vecAB;
            return (float) Math.Sqrt(s * s);
        }

        /// <summary>
        ///     Расстояние от точки до прямой
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static float Distance(Line line, Point pt)
        {
            var z = line.Apply(pt);
            var d = Math.Sqrt(line.A * line.A + line.B * line.B);
            return (float) Math.Abs(z / d);
        }

        /// <summary>
        ///     Вектор скорости после отскока от прямой.
        ///     Не проверяется, что точка касается прямой.
        /// </summary>
        /// <param name="line">Уравнение прямой</param>
        /// <param name="direction">Вектор начальной скорости</param>
        /// <returns>Вектор скорости после отскока</returns>
        public static Point Bounce(Line line, Point direction)
        {
            // a, b, c are not Line parameters, but triangle
            var a = line.A;
            var b = line.B;
            var c = (float) Math.Sqrt(a * a + b * b);
            a /= c;
            b /= c;
            var a2 = a * a;
            var b2 = b * b;
            var doubleAB = 2 * a * b;
            return new Point(
                (b2 - a2) * direction.X + // (b^2 - a^2)Vx +
                doubleAB * direction.Y, // + 2ab * Vy;
                doubleAB * direction.X + //  2ab * Vx +
                (a2 - b2) * direction.Y // + (a^2 - b^2)Vy
            );
        }
    }
}