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
    public sealed class SvgRectElement : SvgDrawingElement
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public static SvgRectElement Parse(XmlReader reader)
        {
            var r = new SvgRectElement();

            Parse(r, reader);

            float f;

            float.TryParse(reader.GetAttribute("x"), out f);
            r.X = f;

            float.TryParse(reader.GetAttribute("y"), out f);
            r.Y = f;

            float.TryParse(reader.GetAttribute("width"), out f);
            r.Width = f;

            float.TryParse(reader.GetAttribute("height"), out f);
            r.Height = f;

            return r;
        }

        public override Rectangle Bounds => new Rectangle(X, Y, Width, Height);

        protected override SvgElement CreateInstainceCore() => new SvgRectElement();

        public override void CopyTo(SvgElement other)
        {
            base.CopyTo(other);

            var e = (SvgRectElement)other;

            e.X = X;
            e.Y = Y;
            e.Width = Width;
            e.Height = Height;
        }
        public override string TagName => "rect";

        protected override void SetAttributes(XElement element)
        {
            base.SetAttributes(element);

            element.SetAttributeValue("x", X);
            element.SetAttributeValue("y", Y);
            element.SetAttributeValue("width", Width);
            element.SetAttributeValue("height", Height);
        }

        public override void Translate(float x, float y)
        {
            X += x;
            Y += y;
        }

        public override void Scale(float scaleX, float scaleY)
        {
            X *= scaleX;
            Y *= scaleY;
            Width *= scaleX;
            Height *= scaleY;
        }
    }
}