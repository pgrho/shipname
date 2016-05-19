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
    public static class SvgElementExtensions
    {
        public static IEnumerable<SvgElement> Descendants(this SvgElement element)
            => element.Descendants(null);

        public static IEnumerable<SvgElement> Descendants(this SvgElement element, string name)
        {
            foreach (var c in element.Items)
            {
                if (name == null || c.TagName == name)
                {
                    yield return c;
                }

                foreach (var d in c.Descendants(name))
                {
                    yield return d;
                }
            }
        }

        public static IEnumerable<SvgElement> Descendants(this IEnumerable<SvgElement> elements)
            => elements.SelectMany(e => e.Descendants());

        public static IEnumerable<SvgElement> Descendants(this IEnumerable<SvgElement> elements, string name)
            => elements.SelectMany(e => e.Descendants(name));

        public static IEnumerable<SvgElement> DescendantsAndSelf(this SvgElement element)
            => element.DescendantsAndSelf(null);

        public static IEnumerable<SvgElement> DescendantsAndSelf(this SvgElement element, string name)
        {
            if (name == null || element.TagName == name)
            {
                yield return element;
            }

            foreach (var c in element.Items)
            {
                if (name == null || c.TagName == name)
                {
                    yield return c;
                }

                foreach (var d in c.Descendants())
                {
                    yield return d;
                }
            }
        }

        public static IEnumerable<SvgElement> DescendantsAndSelf(this IEnumerable<SvgElement> elements)
            => elements.SelectMany(e => e.Descendants());

        public static IEnumerable<SvgElement> DescendantsAndSelf(this IEnumerable<SvgElement> elements, string name)
            => elements.SelectMany(e => e.Descendants(name));
    }
}