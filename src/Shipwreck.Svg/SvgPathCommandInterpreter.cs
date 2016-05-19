using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Shipwreck.Svg
{
    public abstract class SvgPathCommandInterpreter
    {
        public void Execute(IEnumerable<SvgPathCommand> commands)
        {
            var p = Point.Zero;
            var prevCp = Point.NaN;
            foreach (var cmd in commands)
            {
                Point np, cp;
                switch (cmd.Command)
                {
                    case 'M':
                        p = new Point(cmd[0], cmd[1]);
                        OnMoveTo(p);
                        break;

                    case 'm':
                        p += new Point(cmd[0], cmd[1]);
                        OnMoveTo(p);
                        break;

                    case 'L':
                        np = new Point(cmd[0], cmd[1]);
                        OnLineTo(p, np);
                        p = np;
                        break;

                    case 'l':
                        np = p + new Point(cmd[0], cmd[1]);
                        OnLineTo(p, np);
                        p = np;
                        break;

                    case 'H':
                        np = new Point(cmd[0], p.Y);
                        OnLineTo(p, np);
                        p = np;
                        break;

                    case 'h':
                        np = new Point(p.X + cmd[0], p.Y);
                        OnLineTo(p, np);
                        p = np;
                        break;

                    case 'V':
                        np = new Point(p.X, cmd[0]);
                        OnLineTo(p, np);
                        p = np;
                        break;

                    case 'v':
                        np = new Point(p.X, p.Y + cmd[0]);
                        OnLineTo(p, np);
                        p = np;
                        break;

                    case 'C':
                        np = new Point(cmd[0], cmd[1]);
                        prevCp = new Point(cmd[4], cmd[5]);
                        OnCubicCurveTo(p, np, new Point(cmd[2], cmd[3]), prevCp);
                        p = np;
                        break;

                    case 'c':
                        np = p + new Point(cmd[0], cmd[1]);
                        prevCp = p + new Point(cmd[4], cmd[5]);
                        OnCubicCurveTo(p, np, p + new Point(cmd[2], cmd[3]), prevCp);
                        p = np;
                        break;

                    case 'Q':
                        np = new Point(cmd[0], cmd[1]);
                        prevCp = new Point(cmd[2], cmd[3]);
                        OnQuadraticCurveTo(p, np, prevCp);
                        p = np;
                        break;

                    case 'q':
                        np = p + new Point(cmd[0], cmd[1]);
                        prevCp = p + new Point(cmd[2], cmd[3]);
                        OnQuadraticCurveTo(p, np, prevCp);
                        p = np;
                        break;

                    case 'S':

                        np = new Point(cmd[0], cmd[1]);
                        cp = prevCp.IsNaN ? p : 2 * p - prevCp;
                        prevCp = new Point(cmd[2], cmd[3]);
                        OnCubicCurveTo(p, np, cp, prevCp);
                        p = np;
                        break;

                    case 's':

                        np = p + new Point(cmd[0], cmd[1]);
                        cp = prevCp.IsNaN ? p : 2 * p - prevCp;
                        prevCp = p + new Point(cmd[2], cmd[3]);
                        OnCubicCurveTo(p, np, cp, prevCp);
                        p = np;
                        break;

                    case 'T':

                        np = new Point(cmd[0], cmd[1]);
                        prevCp = prevCp.IsNaN ? p : 2 * p - prevCp;
                        OnQuadraticCurveTo(p, np, prevCp);
                        p = np;
                        break;

                    case 't':

                        np = p + new Point(cmd[0], cmd[1]);
                        prevCp = prevCp.IsNaN ? p : 2 * p - prevCp;
                        OnQuadraticCurveTo(p, np, prevCp);
                        p = np;
                        break;

                    case 'A':
                    case 'a':
                        throw new NotSupportedException();

                    case 'Z':
                    case 'z':
                        OnClosePath();
                        break;
                }
            }
        }

        protected abstract void OnMoveTo(Point location);

        protected abstract void OnLineTo(Point start, Point stop);

        protected abstract void OnCubicCurveTo(Point start, Point stop, Point controlPoint1, Point controlPoint2);

        protected abstract void OnQuadraticCurveTo(Point start, Point stop, Point controlPoint);

        protected abstract void OnClosePath();
    }
}