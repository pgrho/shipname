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
    public abstract class SvgDrawingElement : SvgElement
    {
        public string Fill { get; set; }
        public string Stroke { get; set; }

        public float StrokeWidth { get; set; }

        protected static void Parse(SvgDrawingElement element, XmlReader reader)
        {
            element.Fill = ParseColor(reader.GetAttribute("fill"));
            element.Stroke = ParseColor(reader.GetAttribute("stroke"));

            float f;
            if (float.TryParse(reader.GetAttribute("stroke-width"), out f))
            {
                element.StrokeWidth = f;
            }
            else
            {
                element.StrokeWidth = 1;
            }
        }

        public override void CopyTo(SvgElement other)
        {
            base.CopyTo(other);

            var e = (SvgDrawingElement)other;
            e.Fill = Fill;
            e.Stroke = Stroke;
            e.StrokeWidth = StrokeWidth;
        }

        protected override void SetAttributes(XElement element)
        {
            base.SetAttributes(element);

            element.SetAttributeValue("fill", Fill == "#000000" ? null : Fill ?? "none");
            element.SetAttributeValue("stroke", Stroke == "#000000" ? null : Stroke ?? "none");
            element.SetAttributeValue("stroke-width", Stroke == null ? (float?)null : StrokeWidth);
        }
    }
}