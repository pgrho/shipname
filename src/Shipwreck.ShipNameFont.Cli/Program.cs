using Shipwreck.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Shipwreck.ShipNameFont.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var d = new Uri(new Uri(typeof(Program).Assembly.Location), @"../../../../jmsdf/").LocalPath;

            var e = SvgElement.Parse(Path.Combine(d, "__all.svg"));

            var rects = e.Descendants("rect").OfType<SvgRectElement>().Where(_ => _.Width * _.Height > 5000).ToArray();

            var i = 0;

            foreach (var r in rects)
            {
                var svg = new SvgSvgElement();
                var rb = r.Bounds;
                svg.Width = (float)Math.Round((double)rb.Width);
                svg.Height = (float)Math.Round((double)rb.Height);

                var xel = new List<XElement>();

                foreach (var p in e.Descendants().Where(_ => (_.TagName == "polygon" || _.TagName == "path" || _.TagName == "rect") && _.Bounds.IntersectsWith(rb)))
                {
                    if (p == r)
                    {
                        continue;
                    }

                    var c = p.Clone();

                    (c as SvgDrawingElement).Translate(-rb.Left, -rb.Top);

                    svg.Items.Add(c);

                    // xel.Add(p.ToElement());
                }

                var xe = svg.ToElement();

                //foreach (var b in xel)
                //{
                //    b.SetAttributeValue("fill", "none");
                //    b.SetAttributeValue("stroke", "red");
                //    b.SetAttributeValue("stroke-width", "0.25");
                //    b.SetAttributeValue("transform", $"transform({rb.Left:0.###} {rb.Top:0.###})");

                //    xe.Add(b);
                //}

                xe.Save((i++) + ".svg");
            }

            e.ToElement().Save("__all.svg");
        }
    }
}