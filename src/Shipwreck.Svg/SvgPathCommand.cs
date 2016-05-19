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
    public sealed class SvgPathCommand : ICloneable
    {
        private readonly float[] _Values;

        public SvgPathCommand(char command, int valueLength)
        {
            Command = command;
            _Values = new float[valueLength];
        }

        internal SvgPathElement Element { get; set; }

        public char Command { get; }

        public float this[int index]
        {
            get
            {
                return _Values[index];
            }
            set
            {
                _Values[index] = value;
                Element?.InvalidateBounds();
            }
        }

        public int ArgumentCount => _Values.Length;

        public SvgPathCommand Clone()
        {
            var r = new SvgPathCommand(Command, ArgumentCount);
            for (var i = 0; i < _Values.Length; i++)
            {
                r._Values[i] = _Values[i];
            }

            return r;
        }

        object ICloneable.Clone() => Clone();

        public override string ToString()
        {
            if (_Values.Length == 0)
            {
                return Command.ToString();
            }
            var sb = new StringBuilder();
            sb.Append(Command);
            foreach (var v in _Values)
            {
                sb.Append(' ');
                sb.Append(v);
            }

            return sb.ToString();
        }
    }
}