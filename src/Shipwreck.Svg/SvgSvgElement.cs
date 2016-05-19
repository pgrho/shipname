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
    public sealed class SvgSvgElement : SvgElement
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public static SvgSvgElement Parse(XmlReader reader)
        {
            var r = new SvgSvgElement();

            float f;

            float.TryParse(reader.GetAttribute("width"), out f);
            r.Width = f;

            float.TryParse(reader.GetAttribute("height"), out f);
            r.Height = f;

            return r;
        }

        public override void AddChild(SvgElement element)
            => Items.Add(element);

        public override Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        protected override SvgElement CreateInstainceCore() => new SvgSvgElement();

        public override void CopyTo(SvgElement other)
        {
            base.CopyTo(other);

            var e = (SvgSvgElement)other;

            e.Width = Width;
            e.Height = Height;
        }
        public override string TagName => "svg";
        protected override void SetAttributes(XElement element)
        {
            base.SetAttributes(element);
             
            element.SetAttributeValue("width", Width);
            element.SetAttributeValue("height", Height);
        }
    }
}