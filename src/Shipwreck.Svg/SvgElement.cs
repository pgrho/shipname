using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Shipwreck.Svg
{
    [DebuggerDisplay("<{" + nameof(SvgElement.TagName) + "}>")]
    public abstract class SvgElement : ICloneable
    {
        internal const string XMLNS = "http://www.w3.org/2000/svg";

        public abstract string TagName { get; }

        private List<SvgElement> _Items;

        public List<SvgElement> Items
            => _Items ?? (_Items = new List<SvgElement>());

        public abstract Rectangle Bounds { get; }

        public virtual void AddChild(SvgElement element)
        {
            throw new InvalidOperationException();
        }

        public static string ParseColor(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "#000000";
            }
            var v = value.ToLowerInvariant();
            if (v == "none")
            {
                return null;
            }
            return v;
        }

        public SvgElement Clone()
        {
            var r = CreateInstainceCore();

            CopyTo(r);

            return r;
        }

        protected abstract SvgElement CreateInstainceCore();

        public virtual void CopyTo(SvgElement other)
        {
            other.Items.Clear();
            if (_Items != null)
            {
                foreach (var item in _Items)
                {
                    other.Items.Add(item.Clone());
                }
            }
        }

        object ICloneable.Clone() => Clone();

        public XElement ToElement()
        {
            var e = new XElement(XName.Get(TagName, XMLNS));

            SetAttributes(e);

            if (_Items != null)
            {
                foreach (var item in _Items)
                {
                    e.Add(item.ToElement());
                }
            }

            return e;
        }

        protected virtual void SetAttributes(XElement element)
        {
        }

        public static SvgElement Parse(string fileName)
        {
            var elems = new Stack<SvgElement>();

            using (var xr = XmlReader.Create(fileName, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore }))
            {
                while (xr.Read())
                {
                    switch (xr.NodeType)
                    {
                        case XmlNodeType.Element:
                            SvgElement ne;
                            switch (xr.Name)
                            {
                                case "svg":
                                    ne = SvgSvgElement.Parse(xr);
                                    break;

                                case "g":
                                    ne = SvgGroupElement.Parse(xr);
                                    break;

                                case "line":
                                    ne = SvgLineElement.Parse(xr);
                                    break;

                                case "rect":
                                    ne = SvgRectElement.Parse(xr);
                                    break;

                                case "polygon":
                                    ne = SvgPolygonElement.Parse(xr);
                                    break;

                                case "path":
                                    ne = SvgPathElement.Parse(xr);
                                    break;

                                default:
                                    throw new NotSupportedException();
                            }
                            elems.FirstOrDefault()?.AddChild(ne);

                            if (!xr.IsEmptyElement)
                            {
                                elems.Push(ne);
                            }
                            break;

                        case XmlNodeType.EndElement:
                            var le = elems.Pop();
                            if (!elems.Any())
                            {
                                return le;
                            }
                            break;
                    }
                }

                throw new InvalidOperationException();
            }
        }
    }
}