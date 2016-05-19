using Shipwreck.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace Shipwreck.ShipNameFont.Services.Controllers
{
    public class ImagesController : ApiController
    {
        private static bool TryGetSetSmallKana(char smallKana, out char normalKana)
        {
            switch (smallKana)
            {
                case 'ぁ':
                case 'ァ':
                case 'ｧ':
                    normalKana = 'あ';
                    return true;

                case 'ぃ':
                case 'ィ':
                case 'ｨ':
                    normalKana = 'い';
                    return true;

                case 'ぅ':
                case 'ゥ':
                case 'ｩ':
                    normalKana = 'う';
                    return true;

                case 'ぇ':
                case 'ェ':
                case 'ｪ':
                    normalKana = 'え';
                    return true;

                case 'ぉ':
                case 'ォ':
                case 'ｫ':
                    normalKana = 'お';
                    return true;

                case 'ヵ':
                    normalKana = 'か';
                    return true;

                case 'ヶ':
                    normalKana = 'け';
                    return true;

                case 'っ':
                case 'ッ':
                case 'ｯ':
                    normalKana = 'つ';
                    return true;

                case 'ゃ':
                case 'ャ':
                case 'ｬ':
                    normalKana = 'や';
                    return true;

                case 'ゅ':
                case 'ュ':
                case 'ｭ':
                    normalKana = 'ゆ';
                    return true;

                case 'ょ':
                case 'ョ':
                case 'ｮ':
                    normalKana = 'よ';
                    return true;
            }

            normalKana = default(char);

            return false;
        }

        private static char GetCanonicalName(char value)
        {
            if ('０' <= value && value <= '９')
            {
                return (char)(value - '０' + '0');
            }
            if ('a' <= value && value <= 'z')
            {
                return (char)(value - 'a' + 'A');
            }
            if ('Ａ' <= value && value <= 'Ｚ')
            {
                return (char)(value - 'Ａ' + 'A');
            }
            if ('ａ' <= value && value <= 'ｚ')
            {
                return (char)(value - 'ａ' + 'A');
            }

            return value;
        }

        [Route("image/{name}.svg")]
        public async Task<FormattedContentResult<SvgSvgElement>> GetImage(string name)
        {
            SvgSvgElement elem = null;

            foreach (var c in name)
            {
                char cc;

                float scale, dy;

                if (TryGetSetSmallKana(c, out cc))
                {
                    scale = 2f / 3;
                    dy = 100f / 3;
                }
                else
                {
                    scale = 1;
                    dy = 0;
                    cc = GetCanonicalName(c);
                }

                string p;

                if (HttpContext.Current.IsDebuggingEnabled)
                {
                    p = new Uri(new Uri(HttpContext.Current.Server.MapPath("/")), $"../../jmsdf/{cc}.svg").LocalPath;
                }
                else
                {
                    p = HttpContext.Current.Server.MapPath($"/Content/jmsdf/{cc}.svg");
                }
                var e = await SvgElement.LoadFileAsync(p);

                if (elem == null)
                {
                    elem = (SvgSvgElement)e.Clone();
                    foreach (var ce in elem.Items.OfType<SvgDrawingElement>())
                    {
                        if (scale != 1)
                        {
                            ce.Scale(scale, scale);
                            ce.Translate(0, dy);
                            if (ce.Fill == "#ffffff")
                            {
                                ce.Stroke = "#000000";
                                ce.StrokeWidth = 1.5f;
                                ce.StrokeLocaltion = StrokeLocaltion.Outside;
                            }
                        }
                    }
                }
                else
                {
                    var pb = elem.ChildBounds;

                    var cloned = e.Clone();

                    if (scale != 1)
                    {
                        foreach (var ce in cloned.Items.OfType<SvgDrawingElement>())
                        {
                            ce.Scale(scale, scale);

                            if (ce.Fill == "#ffffff")
                            {
                                ce.Stroke = "#000000";
                                ce.StrokeWidth = 1.5f;
                                ce.StrokeLocaltion = StrokeLocaltion.Outside;
                            }
                        }
                    }

                    var eb = cloned.ChildBounds;

                    var dx = pb.Right + 40 - eb.Left;

                    foreach (var ce in cloned.Items)
                    {
                        var nc = ce.Clone();
                        (nc as SvgDrawingElement)?.Translate(dx, dy);

                        elem.AddChild(nc);
                    }
                }
            }
            var cb = elem.ChildBounds;
            elem.Width = cb.Width;
            elem.Height = cb.Height;
            foreach (var ce in elem.Items)
            {
                (ce as SvgDrawingElement).Translate(-cb.Left, -cb.Top);
            }

            return Content(System.Net.HttpStatusCode.OK, elem, new SvgMediaTypeFormatter());
        }
    }
}