using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Shipwreck.Svg
{
    public sealed class SvgPathElement : SvgDrawingElement
    {
        private sealed class BoundsInterpreter : SvgPathCommandInterpreter
        {
            private float lx = float.MaxValue;
            private float ux = float.MinValue;
            private float ly = float.MaxValue;
            private float uy = float.MinValue;

            protected override void OnClosePath()
            {
            }

            protected override void OnCubicCurveTo(Point start, Point controlPoint1, Point controlPoint2, Point stop)
            {
                AddPoint(stop);
                AddPoint(controlPoint1);
                AddPoint(controlPoint2);
            }

            protected override void OnLineTo(Point start, Point stop)
            {
                AddPoint(stop);
            }

            protected override void OnMoveTo(Point location)
            {
                AddPoint(location);
            }

            protected override void OnQuadraticCurveTo(Point start, Point controlPoint, Point stop)
            {
                AddPoint(stop);
                AddPoint(controlPoint);
            }

            private void AddPoint(Point v)
            {
                lx = Math.Min(lx, v.X);
                ux = Math.Max(ux, v.X);
                ly = Math.Min(ly, v.Y);
                uy = Math.Max(uy, v.Y);
            }

            public Rectangle Bounds => new Rectangle(lx, ly, ux - lx, uy - ly);
        }

        private sealed class ToStringInterpreter : SvgPathCommandInterpreter
        {
            private StringBuilder sb = new StringBuilder();

            private char Prev;
            private Point PrevPoint = Point.NaN;

            protected override void OnClosePath()
            {
                sb.Append("Z ");
            }

            protected override void OnCubicCurveTo(Point start, Point controlPoint1, Point controlPoint2, Point stop)
            {
                sb.Append("C ");
                sb.Append(controlPoint1.X.ToString("0.####"));
                sb.Append(',');
                sb.Append(controlPoint1.Y.ToString("0.####"));
                sb.Append(' ');
                sb.Append(controlPoint2.X.ToString("0.####"));
                sb.Append(',');
                sb.Append(controlPoint2.Y.ToString("0.####"));
                sb.Append(' ');
                sb.Append(stop.X.ToString("0.####"));
                sb.Append(',');
                sb.Append(stop.Y.ToString("0.####"));
                sb.Append(' ');

                Prev = 'C';
                PrevPoint = stop;
            }

            protected override void OnLineTo(Point start, Point stop)
            {
                sb.Append("L ");
                sb.Append(stop.X.ToString("0.####"));
                sb.Append(',');
                sb.Append(stop.Y.ToString("0.####"));
                sb.Append(' ');

                Prev = 'L';
                PrevPoint = stop;
            }

            protected override void OnMoveTo(Point location)
            {
                sb.Append("M ");
                sb.Append(location.X.ToString("0.####"));
                sb.Append(',');
                sb.Append(location.Y.ToString("0.####"));
                sb.Append(' ');

                Prev = 'M';
                PrevPoint = location;
            }

            protected override void OnQuadraticCurveTo(Point start, Point controlPoint, Point stop)
            {
                sb.Append("Q ");
                sb.Append(controlPoint.X.ToString("0.####"));
                sb.Append(',');
                sb.Append(controlPoint.Y.ToString("0.####"));
                sb.Append(' ');
                sb.Append(stop.X.ToString("0.####"));
                sb.Append(',');
                sb.Append(stop.Y.ToString("0.####"));
                sb.Append(' ');

                Prev = 'Q';
                PrevPoint = stop;
            }

            public override string ToString()
            {
                if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
                {
                    sb.Length--;
                }
                return sb.ToString();
            }
        }

        private SvgPathCommandCollection _D;

        private Rectangle? _Bounds;

        public override Rectangle Bounds
        {
            get
            {
                if (_Bounds == null)
                {
                    if (_D?.Count > 0)
                    {
                        var ip = new BoundsInterpreter();
                        ip.Execute(_D);
                        _Bounds = ip.Bounds;
                    }
                    else
                    {
                        return Rectangle.NaN;
                    }
                }
                return _Bounds.Value;
            }
        }

        internal void InvalidateBounds() => _Bounds = null;

        public IList<SvgPathCommand> D
        {
            get
            {
                return _D ?? (_D = new SvgPathCommandCollection(this));
            }
            set
            {
                if (value == _D)
                {
                    return;
                }
                _D?.Clear();
                if (value?.Count > 0)
                {
                    var l = D;
                    foreach (var v in value)
                    {
                        l.Add(v);
                    }
                }
            }
        }

        public static SvgPathElement Parse(XmlReader reader)
        {
            var r = new SvgPathElement();

            Parse(r, reader);

            r.D = SvgPathCommandParser.Parse(reader.GetAttribute("d") ?? string.Empty);

            return r;
        }

        protected override SvgElement CreateInstainceCore()
            => new SvgPathElement();

        public override void CopyTo(SvgElement other)
        {
            base.CopyTo(other);

            var e = (SvgPathElement)other;
            e.D = _D?.Select(_ => _.Clone()).ToArray();
        }

        public override string TagName => "path";

        protected override void SetAttributes(XElement element)
        {
            base.SetAttributes(element);

            var tci = new ToStringInterpreter();
            tci.Execute(D);

            element.SetAttributeValue("d", tci.ToString());
        }

        public override void Translate(float x, float y)
        {
            if (_D != null)
            {
                foreach (var c in _D)
                {
                    c.Translate(x, y);
                }
            }
        }
    }
}