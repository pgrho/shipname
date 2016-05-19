using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Shipwreck.Svg
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        public Rectangle(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public static Rectangle NaN => new Rectangle(float.NaN, float.NaN, float.NaN, float.NaN);

        public float Left { get; set; }
        public float Top { get; set; }

        public float Right => Left + Width;
        public float Bottom => Top + Height;

        public Point Location
        {
            get
            {
                return new Point(Left, Top);
            }
            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public float Width { get; set; }
        public float Height { get; set; }

        public bool IsNaN => float.IsNaN(Left) || float.IsNaN(Top) || float.IsNaN(Width) || float.IsNaN(Height);

        public static bool operator ==(Rectangle left, Rectangle right)
            => left.Left == right.Left && left.Top == right.Top && left.Width == right.Width && left.Height == right.Height;

        public static bool operator !=(Rectangle left, Rectangle right)
            => left.Left != right.Left || left.Top != right.Top || left.Width != right.Width || left.Height != right.Height;

        public override bool Equals(object obj)
            => obj is Point && this == (Rectangle)obj;

        public bool Equals(Rectangle other)
            => this == other;

        public override int GetHashCode()
            => Left.GetHashCode() ^ Top.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();

        public Rectangle UnionWith(Rectangle other)
        {
            var lx = Math.Min(Left, other.Left);
            var ux = Math.Max(Right, other.Right);
            var ly = Math.Min(Top, other.Top);
            var uy = Math.Max(Bottom, other.Bottom);

            return new Rectangle(lx, ly, ux - lx, uy - ly);
        }

        public bool IntersectsWith(Rectangle other)
            => Left <= other.Right
                && other.Left <= Right
                && Top <= other.Bottom
                && other.Top <= Bottom;
    }
}