using Shipwreck.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Shipwreck.ShipNameFont.Cli
{
    class Program
    {
        private static void Main(string[] args)
        {
            var d = new Uri(new Uri(typeof(Program).Assembly.Location), @"../../../../jmsdf/").LocalPath;

            var e = SvgElement.Parse(Path.Combine(d, "__all.svg"));

            e.ToElement().Save("test.svg");
        }
    }
}