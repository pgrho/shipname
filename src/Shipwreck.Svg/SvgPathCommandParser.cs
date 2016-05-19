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
    internal static class SvgPathCommandParser
    {
        public static List<SvgPathCommand> Parse(string d)
        {
            var dl = new List<SvgPathCommand>();
            var isInDigit = false;
            var cmd = default(char);
            SvgPathCommand co = null;
            var cvi = 0;
            var sb = new StringBuilder();
            foreach (var c in d)
            {
                if (isInDigit)
                {
                    if (CanConfinueFloat(sb, c))
                    {
                        sb.Append(c);
                    }
                    else if (IsFloatStart(c))
                    {
                        co[cvi++] = float.Parse(sb.ToString());
                        sb.Clear();
                        sb.Append(c);
                    }
                    else if (IsSeparator(c))
                    {
                        co[cvi++] = float.Parse(sb.ToString());
                        sb.Clear();
                        isInDigit = false;
                    }
                    else if (IsCommand(c))
                    {
                        co[cvi++] = float.Parse(sb.ToString());

                        cmd = c;
                        co = GetCommand(cmd);
                        dl.Add(co);
                        cvi = 0;
                        isInDigit = false;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    if (IsSeparator(c))
                    {
                        // Do nothing
                    }
                    else if (IsFloatStart(c))
                    {
                        if (co == null)
                        {
                            throw new NotSupportedException();
                        }
                        else if (cvi >= co.ArgumentCount)
                        {
                            co = GetCommand(cmd);
                            if (co.ArgumentCount == 0)
                            {
                                throw new InvalidOperationException();
                            }
                            dl.Add(co);
                            cvi = 0;
                        }

                        sb.Clear();

                        sb.Append(c);
                        isInDigit = true;
                    }
                    else if (IsCommand(c))
                    {
                        cmd = c;
                        co = GetCommand(cmd);
                        dl.Add(co);
                        cvi = 0;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
            }

            if (isInDigit)
            {
                co[cvi++] = float.Parse(sb.ToString());
            }

            return dl;
        }

        private static SvgPathCommand GetCommand(char command)
        {
            switch (command)
            {
                case 'M':
                case 'm':
                case 'L':
                case 'l':
                    return new SvgPathCommand(command, 2);

                case 'H':
                case 'h':
                case 'V':
                case 'v':
                    return new SvgPathCommand(command, 1);

                case 'C':
                case 'c':
                    return new SvgPathCommand(command, 6);

                case 'Q':
                case 'q':
                    return new SvgPathCommand(command, 4);

                case 'S':
                case 's':
                    return new SvgPathCommand(command, 4);

                case 'T':
                case 't':
                    return new SvgPathCommand(command, 2);

                case 'A':
                case 'a':
                    return new SvgPathCommand(command, 7);

                case 'Z':
                case 'z':
                    return new SvgPathCommand(command, 0);
            }
            throw new NotSupportedException();
        }

        private static bool IsFloatStart(char c)
            => ('0' <= c && c <= '9') || c == '-' || c == '-' || c == '.';

        private static bool CanConfinueFloat(StringBuilder sb, char c)
        {
            if ('0' <= c && c <= '9')
            {
                return true;
            }
            if (c == '.')
            {
                for (var i = 0; i < sb.Length; i++)
                {
                    if (sb[i] == '.')
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private static bool IsSeparator(char c)
            => char.IsWhiteSpace(c) || c == ',';

        private static bool IsCommand(char c)
        {
            switch (c)
            {
                case 'M':
                case 'm':
                case 'L':
                case 'l':
                case 'H':
                case 'h':
                case 'V':
                case 'v':

                case 'C':
                case 'c':

                case 'Q':
                case 'q':

                case 'T':
                case 't':

                case 'S':
                case 's':

                case 'A':
                case 'a':
                case 'Z':
                case 'z':
                    return true;
            }
            return false;
        }
    }
}