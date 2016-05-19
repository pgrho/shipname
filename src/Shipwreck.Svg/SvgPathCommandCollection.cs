using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Shipwreck.Svg
{
    internal sealed class SvgPathCommandCollection : Collection<SvgPathCommand>
    {
        private readonly SvgPathElement _Element;

        public SvgPathCommandCollection(SvgPathElement element)
        {
            _Element = element;
        }

        protected override void ClearItems()
        {
            foreach (var item in this)
            {
                item.Element = null;
            }
            base.ClearItems();
            _Element.InvalidateBounds();
        }

        protected override void InsertItem(int index, SvgPathCommand item)
        {
            if (item.Element != null)
            {
                throw new ArgumentException();
            }
            item.Element = _Element;
            base.InsertItem(index, item);
            _Element.InvalidateBounds();
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            item.Element = null;
            base.RemoveItem(index);
            _Element.InvalidateBounds();
        }

        protected override void SetItem(int index, SvgPathCommand item)
        {
            var old = this[index];
            if (old == item)
            {
                return;
            }
            if (item.Element != null)
            {
                throw new ArgumentException();
            }
            old.Element = null;
            item.Element = _Element;
            base.SetItem(index, item);
            _Element.InvalidateBounds();
        }
    }
}