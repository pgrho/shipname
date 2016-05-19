using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.Svg
{
    [TestClass]
    public class SvgPathCommandParserTest
    {
        [TestMethod]
        public void ParseTest()
        {
            var arg = "M-168.034,1044.646h-20v99.999l8.819,8.819h20v-99.999L-168.034,1044.646z M-170.239,1142.44h-15.59v-95.589h15.59V1142.44 z";
            var cmds = SvgPathCommandParser.Parse(arg);

            var pe = new SvgPathElement();
            pe.D = cmds;
            pe.ToString();
        }
    }
}
