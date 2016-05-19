using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Shipwreck.Svg
{
    public sealed class SvgPolygonElement : SvgDrawingElement
    {
        private sealed class PointCollection : Collection<Point>
        {
            private readonly SvgPolygonElement _Element;

            public PointCollection(SvgPolygonElement element)
            {
                _Element = element;
            }

            protected override void ClearItems()
            {
                base.ClearItems();
                _Element.InvalidateBounds();
            }

            protected override void InsertItem(int index, Point item)
            {
                base.InsertItem(index, item);
                _Element.InvalidateBounds();
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                _Element.InvalidateBounds();
            }

            protected override void SetItem(int index, Point item)
            {
                base.SetItem(index, item);
                _Element.InvalidateBounds();
            }
        }

        private PointCollection _Points;

        private Rectangle? _Bounds;

        public override Rectangle Bounds
        {
            get
            {
                if (_Bounds == null)
                {
                    if (_Points?.Count > 0)
                    {
                        var lx = float.MaxValue;
                        var ux = float.MinValue;
                        var ly = float.MaxValue;
                        var uy = float.MinValue;

                        foreach (var v in _Points)
                        {
                            lx = Math.Min(lx, v.X);
                            ux = Math.Max(ux, v.X);
                            ly = Math.Min(ly, v.Y);
                            uy = Math.Max(uy, v.Y);
                        }

                        _Bounds = new Rectangle(lx, ly, ux - lx, uy - ly);
                    }
                    else
                    {
                        return Rectangle.NaN;
                    }
                }
                return _Bounds.Value;
            }
        }

        internal void InvalidateBounds()
            => _Bounds = null;

        public IList<Point> Points
        {
            get
            {
                return _Points ?? (_Points = new PointCollection(this));
            }
            set
            {
                if (value == _Points)
                {
                    return;
                }
                _Points?.Clear();
                if (value?.Count > 0)
                {
                    var l = Points;
                    foreach (var v in Points)
                    {
                        l.Add(v);
                    }
                }
            }
        }

        public static SvgPolygonElement Parse(XmlReader reader)
        {
            var r = new SvgPolygonElement();

            Parse(r, reader);

            var ps = (reader.GetAttribute("points") ?? string.Empty).Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(float.Parse).ToArray();

            r.Points = Enumerable.Range(0, ps.Length / 2).Select(i => new Point(ps[i * 2], ps[2 * i + 1])).ToArray();

            return r;
        }

        protected override SvgElement CreateInstainceCore() => new SvgPolygonElement();

        public override void CopyTo(SvgElement other)
        {
            base.CopyTo(other);

            var e = (SvgPolygonElement)other;
            e.Points = _Points?.ToArray();
        }
        public override string TagName => "polygon";

        protected override void SetAttributes(XElement element)
        {
            base.SetAttributes(element);

            element.SetAttributeValue("points", string.Join(" ", Points.Select(p => $"{p.X},{p.Y}")));
        }
    }
}