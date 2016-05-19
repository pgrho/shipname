using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Shipwreck.Svg
{
    public sealed class SvgGroupElement : SvgElement
    {
        public static SvgGroupElement Parse(XmlReader reader)
        {
            return new SvgGroupElement();
        }

        public override void AddChild(SvgElement element)
            => Items.Add(element);

        public override Rectangle Bounds
        {
            get
            {
                var r = Rectangle.NaN;

                foreach (var item in Items)
                {
                    var b = item.Bounds;

                    if (r.IsNaN)
                    {
                        r = b;
                    }
                    else if (!b.IsNaN)
                    {
                        r = r.UnionWith(b);
                    }
                }

                return r;
            }
        }

        protected override SvgElement CreateInstainceCore()
            => new SvgGroupElement();

        public override string TagName => "g";
    }
}