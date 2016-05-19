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
    public enum StrokeLocaltion
    {
        Center = 0,
        Outside = 1,
        Inside = 2
    }

    public abstract class SvgDrawingElement : SvgElement
    {
        public string Fill { get; set; }

        public string Stroke { get; set; }

        public float StrokeWidth { get; set; }

        public StrokeLocaltion StrokeLocaltion { get; set; }

        protected static void Parse(SvgDrawingElement element, XmlReader reader)
        {
            element.Fill = ParseColor(reader.GetAttribute("fill"));
            element.Stroke = ParseColor(reader.GetAttribute("stroke"));

            float f;
            float.TryParse(reader.GetAttribute("stroke-width"), out f);
            element.StrokeWidth = f;

            switch ((reader.GetAttribute("stroke-location") ?? string.Empty).ToLower())
            {
                case "center":
                    element.StrokeLocaltion = StrokeLocaltion.Center;
                    break;
                case "outside":
                    element.StrokeLocaltion = StrokeLocaltion.Outside;
                    break;
                case "inside":
                    element.StrokeLocaltion = StrokeLocaltion.Inside;
                    break;
            }
        }

        public override void CopyTo(SvgElement other)
        {
            base.CopyTo(other);

            var e = (SvgDrawingElement)other;
            e.Fill = Fill;
            e.Stroke = Stroke;
            e.StrokeWidth = StrokeWidth;
            e.StrokeLocaltion = StrokeLocaltion;
        }

        protected override void SetAttributes(XElement element)
        {
            base.SetAttributes(element);

            element.SetAttributeValue("fill", Fill == "#000000" ? null : Fill ?? "none");
            if (Stroke == null || StrokeWidth <= 0)
            {
                element.SetAttributeValue("stroke", "none");
                element.SetAttributeValue("stroke-width", null);
            }
            else
            {
                element.SetAttributeValue("stroke", Stroke);
                element.SetAttributeValue("stroke-width", StrokeWidth);
            }

            element.SetAttributeValue("stroke-location", StrokeLocaltion == StrokeLocaltion.Center ? null : StrokeLocaltion.ToString().ToLower());
        }

        public abstract void Scale(float scaleX, float scaleY);

        public abstract void Translate(float x, float y);
    }
}