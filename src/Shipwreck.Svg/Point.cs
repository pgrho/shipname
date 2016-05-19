using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Shipwreck.Svg
{
    public struct Point : IEquatable<Point>
    {
        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point NaN = new Point(float.NaN, float.NaN);

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public bool IsNaN => float.IsNaN(X) || float.IsNaN(Y);

        public static bool operator ==(Point left, Point right)
            => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Point left, Point right)
            => left.X != right.X || left.Y != right.Y;

        public static Point operator +(Point left, Point right)
            => new Point(left.X + right.X, left.Y + right.Y);

        public static Point operator -(Point left, Point right)
            => new Point(left.X - right.X, left.Y - right.Y);

        public static Point operator *(float left, Point right)
            => new Point(left * right.X, left * right.Y);

        public static Point operator *(Point left, float right)
            => new Point(left.X * right, left.Y * right);

        public static Point operator /(Point left, float right)
            => new Point(left.X / right, left.Y / right);

        public override bool Equals(object obj)
            => obj is Point && this == (Point)obj;

        public bool Equals(Point other)
            => this == other;

        public override int GetHashCode()
            => X.GetHashCode() ^ Y.GetHashCode();
    }
}