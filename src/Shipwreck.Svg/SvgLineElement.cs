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
    public sealed class SvgLineElement : SvgDrawingElement
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }

        public static SvgLineElement Parse(XmlReader reader)
        {
            var r = new SvgLineElement();

            Parse(r, reader);

            float f;

            float.TryParse(reader.GetAttribute("x1"), out f);
            r.X1 = f;

            float.TryParse(reader.GetAttribute("y1"), out f);
            r.Y1 = f;

            float.TryParse(reader.GetAttribute("x2"), out f);
            r.X2 = f;

            float.TryParse(reader.GetAttribute("y2"), out f);
            r.Y2 = f;

            return r;
        }

        public override Rectangle Bounds
        {
            get
            {
                float lx, ux, ly, uy;
                if (X1 < X2)
                {
                    lx = X1;
                    ux = X2;
                }
                else
                {
                    lx = X2;
                    ux = X1;
                }
                if (Y1 < Y2)
                {
                    ly = Y1;
                    uy = Y2;
                }
                else
                {
                    ly = Y2;
                    uy = Y1;
                }
                return new Rectangle(lx, ly, ux - lx, uy - ly);
            }
        }

        protected override SvgElement CreateInstainceCore() => new SvgLineElement();

        public override void CopyTo(SvgElement other)
        {
            base.CopyTo(other);

            var e = (SvgLineElement)other;
            e.X1 = X1;
            e.Y1 = Y1;
            e.X2 = X2;
            e.Y2 = Y2;
        }
        public override string TagName => "line";

        protected override void SetAttributes(XElement element)
        {
            base.SetAttributes(element);

            element.SetAttributeValue("x1", X1);
            element.SetAttributeValue("y1", Y1);
            element.SetAttributeValue("x2", X2);
            element.SetAttributeValue("y2", Y2);
        }

        public override void Scale(float scaleX, float scaleY)
        {
            X1 *= scaleX;
            Y1 *= scaleY;
            X2 *= scaleX;
            Y2 *= scaleY;
        }

        public override void Translate(float x, float y)
        {
            X1 += x;
            Y1 += y;
            X2 += x;
            Y2 += y;
        }
    }
}