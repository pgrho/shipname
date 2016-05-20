using Shipwreck.ShipNameFont.Services.Formatting;
using Shipwreck.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace Shipwreck.ShipNameFont.Services.Controllers
{
    public sealed class ImagesController : ApiController
    {
        private static readonly ReaderWriterLockSlim _Lock = new ReaderWriterLockSlim();
        private static Dictionary<Tuple<string, char>, SvgElement> _Templates = new Dictionary<Tuple<string, char>, SvgElement>();

        private static async Task<SvgElement> GetTemplateAsync(string type, char character)
        {
            SvgElement t;
            var tp = Tuple.Create(type, character);

            _Lock.EnterReadLock();
            try
            {
                if (_Templates.TryGetValue(tp, out t))
                {
                    return t;
                }
            }
            finally
            {
                _Lock.ExitReadLock();
            }

            var c = HttpContext.Current;
            string p;

            if (c.IsDebuggingEnabled)
            {
                p = new Uri(new Uri(c.Server.MapPath("~/")), $"../../{type}/{character }.svg").LocalPath;
            }
            else
            {
                p = c.Server.MapPath($"~/Content/{type}/{character}.svg");
            }

            if (File.Exists(p))
            {
                t = await SvgElement.LoadFileAsync(p);
            }
            else
            {
                t = null;
            }

            _Lock.EnterWriteLock();
            try
            {
                _Templates[tp] = t;
            }
            finally
            {
                _Lock.ExitWriteLock();
            }

            return t;
        }

        #region 文字

        private static bool TryGetCanonicalChar(char value, out char normalKana)
        {
            switch (value)
            {
                case ' ':
                case '　':
                    normalKana = ' ';
                    return true;

                case 'あ':
                case 'ア':
                case 'ｱ':
                    normalKana = 'あ';
                    return true;

                case 'い':
                case 'ゐ':
                case 'イ':
                case 'ヰ':
                case 'ｲ':
                    normalKana = 'い';
                    return true;

                case 'う':
                case 'ウ':
                case 'ｳ':
                    normalKana = 'う';
                    return true;

                case 'え':
                case 'ゑ':
                case 'エ':
                case 'ヱ':
                case 'ｴ':
                    normalKana = 'え';
                    return true;

                case 'お':
                case 'オ':
                case 'ｵ':
                    normalKana = 'お';
                    return true;

                case 'か':
                case 'カ':
                case 'ｶ':
                    normalKana = 'か';
                    return true;

                case 'き':
                case 'キ':
                case 'ｷ':
                    normalKana = 'き';
                    return true;

                case 'く':
                case 'ク':
                case 'ｸ':
                    normalKana = 'く';
                    return true;

                case 'け':
                case 'ケ':
                case 'ｹ':
                    normalKana = 'け';
                    return true;

                case 'こ':
                case 'コ':
                case 'ｺ':
                    normalKana = 'こ';
                    return true;

                case 'さ':
                case 'サ':
                case 'ｻ':
                    normalKana = 'さ';
                    return true;

                case 'し':
                case 'シ':
                case 'ｼ':
                    normalKana = 'し';
                    return true;

                case 'す':
                case 'ス':
                case 'ｽ':
                    normalKana = 'す';
                    return true;

                case 'せ':
                case 'セ':
                case 'ｾ':
                    normalKana = 'せ';
                    return true;

                case 'そ':
                case 'ソ':
                case 'ｿ':
                    normalKana = 'そ';
                    return true;

                case 'た':
                case 'タ':
                case 'ﾀ':
                    normalKana = 'た';
                    return true;

                case 'ち':
                case 'チ':
                case 'ﾁ':
                    normalKana = 'ち';
                    return true;

                case 'つ':
                case 'ツ':
                case 'ﾂ':
                    normalKana = 'つ';
                    return true;

                case 'て':
                case 'テ':
                case 'ﾃ':
                    normalKana = 'て';
                    return true;

                case 'と':
                case 'ト':
                case 'ﾄ':
                    normalKana = 'と';
                    return true;

                case 'な':
                case 'ナ':
                case 'ﾅ':
                    normalKana = 'な';
                    return true;

                case 'に':
                case 'ニ':
                case 'ﾆ':
                    normalKana = 'に';
                    return true;

                case 'ぬ':
                case 'ヌ':
                case 'ﾇ':
                    normalKana = 'ぬ';
                    return true;

                case 'ね':
                case 'ネ':
                case 'ﾈ':
                    normalKana = 'ね';
                    return true;

                case 'の':
                case 'ノ':
                case 'ﾉ':
                    normalKana = 'の';
                    return true;

                case 'は':
                case 'ぱ':
                case 'ハ':
                case 'パ':
                case 'ﾊ':
                    normalKana = 'は';
                    return true;

                case 'ひ':
                case 'ぴ':
                case 'ヒ':
                case 'ピ':
                case 'ﾋ':
                    normalKana = 'ひ';
                    return true;

                case 'ふ':
                case 'ぷ':
                case 'フ':
                case 'プ':
                case 'ﾌ':
                    normalKana = 'ふ';
                    return true;

                case 'へ':
                case 'ぺ':
                case 'ヘ':
                case 'ペ':
                case 'ﾍ':
                    normalKana = 'へ';
                    return true;

                case 'ほ':
                case 'ぽ':
                case 'ホ':
                case 'ポ':
                case 'ﾎ':
                    normalKana = 'ほ';
                    return true;

                case 'ま':
                case 'マ':
                case 'ﾏ':
                    normalKana = 'ま';
                    return true;

                case 'み':
                case 'ミ':
                case 'ﾐ':
                    normalKana = 'み';
                    return true;

                case 'む':
                case 'ム':
                case 'ﾑ':
                    normalKana = 'む';
                    return true;

                case 'め':
                case 'メ':
                case 'ﾒ':
                    normalKana = 'め';
                    return true;

                case 'も':
                case 'モ':
                case 'ﾓ':
                    normalKana = 'も';
                    return true;

                case 'や':
                case 'ヤ':
                case 'ﾔ':
                    normalKana = 'や';
                    return true;

                case 'ゆ':
                case 'ユ':
                case 'ﾕ':
                    normalKana = 'ゆ';
                    return true;

                case 'よ':
                case 'ヨ':
                case 'ﾖ':
                    normalKana = 'よ';
                    return true;

                case 'ら':
                case 'ラ':
                case 'ﾗ':
                    normalKana = 'ら';
                    return true;

                case 'り':
                case 'リ':
                case 'ﾘ':
                    normalKana = 'り';
                    return true;

                case 'る':
                case 'ル':
                case 'ﾙ':
                    normalKana = 'る';
                    return true;

                case 'れ':
                case 'レ':
                case 'ﾚ':
                    normalKana = 'れ';
                    return true;

                case 'ろ':
                case 'ロ':
                case 'ﾛ':
                    normalKana = 'ろ';
                    return true;

                case 'わ':
                case 'ワ':
                case 'ﾜ':
                    normalKana = 'わ';
                    return true;

                case 'を':
                case 'ヲ':
                case 'ｦ':
                    normalKana = 'を';
                    return true;

                case 'ん':
                case 'ン':
                case 'ﾝ':
                    normalKana = 'ん';
                    return true;

                case 'が':
                case 'ガ':
                    normalKana = 'が';
                    return true;

                case 'ぎ':
                case 'ギ':
                    normalKana = 'ぎ';
                    return true;

                case 'ぐ':
                case 'グ':
                    normalKana = 'ぐ';
                    return true;

                case 'げ':
                case 'ゲ':
                    normalKana = 'げ';
                    return true;

                case 'ご':
                case 'ゴ':
                    normalKana = 'ご';
                    return true;

                case 'ざ':
                case 'ザ':
                    normalKana = 'ざ';
                    return true;

                case 'じ':
                case 'ジ':
                    normalKana = 'じ';
                    return true;

                case 'ず':
                case 'ズ':
                    normalKana = 'ず';
                    return true;

                case 'ぜ':
                case 'ゼ':
                    normalKana = 'ぜ';
                    return true;

                case 'ぞ':
                case 'ゾ':
                    normalKana = 'ぞ';
                    return true;

                case 'だ':
                case 'ダ':
                    normalKana = 'だ';
                    return true;

                case 'ぢ':
                case 'ヂ':
                    normalKana = 'ぢ';
                    return true;

                case 'づ':
                case 'ヅ':
                    normalKana = 'づ';
                    return true;

                case 'で':
                case 'デ':
                    normalKana = 'で';
                    return true;

                case 'ど':
                case 'ド':
                    normalKana = 'ど';
                    return true;

                case 'ば':
                case 'バ':
                    normalKana = 'ば';
                    return true;

                case 'び':
                case 'ビ':
                    normalKana = 'び';
                    return true;

                case 'ぶ':
                case 'ブ':
                case 'ヴ':
                    normalKana = 'ぶ';
                    return true;

                case 'べ':
                case 'ベ':
                    normalKana = 'べ';
                    return true;

                case 'ぼ':
                case 'ボ':
                    normalKana = 'ぼ';
                    return true;

                case '0':
                case '０':
                    normalKana = '0';
                    return true;

                case '1':
                case '１':
                    normalKana = '1';
                    return true;

                case '2':
                case '２':
                    normalKana = '2';
                    return true;

                case '3':
                case '３':
                    normalKana = '3';
                    return true;

                case '4':
                case '４':
                    normalKana = '4';
                    return true;

                case '5':
                case '５':
                    normalKana = '5';
                    return true;

                case '6':
                case '６':
                    normalKana = '6';
                    return true;

                case '7':
                case '７':
                    normalKana = '7';
                    return true;

                case '8':
                case '８':
                    normalKana = '8';
                    return true;

                case '9':
                case '９':
                    normalKana = '9';
                    return true;

                case 'A':
                case 'a':
                case 'Ａ':
                case 'ａ':
                    normalKana = 'A';
                    return true;

                case 'B':
                case 'b':
                case 'Ｂ':
                case 'ｂ':
                    normalKana = 'B';
                    return true;

                case 'C':
                case 'c':
                case 'Ｃ':
                case 'ｃ':
                    normalKana = 'C';
                    return true;

                case 'D':
                case 'd':
                case 'Ｄ':
                case 'ｄ':
                    normalKana = 'D';
                    return true;

                case 'E':
                case 'e':
                case 'Ｅ':
                case 'ｅ':
                    normalKana = 'E';
                    return true;

                case 'F':
                case 'f':
                case 'Ｆ':
                case 'ｆ':
                    normalKana = 'F';
                    return true;

                case 'G':
                case 'g':
                case 'Ｇ':
                case 'ｇ':
                    normalKana = 'G';
                    return true;

                case 'H':
                case 'h':
                case 'Ｈ':
                case 'ｈ':
                    normalKana = 'H';
                    return true;

                case 'I':
                case 'i':
                case 'Ｉ':
                case 'ｉ':
                    normalKana = 'I';
                    return true;

                case 'J':
                case 'j':
                case 'Ｊ':
                case 'ｊ':
                    normalKana = 'J';
                    return true;

                case 'K':
                case 'k':
                case 'Ｋ':
                case 'ｋ':
                    normalKana = 'K';
                    return true;

                case 'L':
                case 'l':
                case 'Ｌ':
                case 'ｌ':
                    normalKana = 'L';
                    return true;

                case 'M':
                case 'm':
                case 'Ｍ':
                case 'ｍ':
                    normalKana = 'M';
                    return true;

                case 'N':
                case 'n':
                case 'Ｎ':
                case 'ｎ':
                    normalKana = 'N';
                    return true;

                case 'O':
                case 'o':
                case 'Ｏ':
                case 'ｏ':
                    normalKana = 'O';
                    return true;

                case 'P':
                case 'p':
                case 'Ｐ':
                case 'ｐ':
                    normalKana = 'P';
                    return true;

                case 'Q':
                case 'q':
                case 'Ｑ':
                case 'ｑ':
                    normalKana = 'Q';
                    return true;

                case 'R':
                case 'r':
                case 'Ｒ':
                case 'ｒ':
                    normalKana = 'R';
                    return true;

                case 'S':
                case 's':
                case 'Ｓ':
                case 'ｓ':
                    normalKana = 'S';
                    return true;

                case 'T':
                case 't':
                case 'Ｔ':
                case 'ｔ':
                    normalKana = 'T';
                    return true;

                case 'U':
                case 'u':
                case 'Ｕ':
                case 'ｕ':
                    normalKana = 'U';
                    return true;

                case 'V':
                case 'v':
                case 'Ｖ':
                case 'ｖ':
                    normalKana = 'V';
                    return true;

                case 'W':
                case 'w':
                case 'Ｗ':
                case 'ｗ':
                    normalKana = 'W';
                    return true;

                case 'X':
                case 'x':
                case 'Ｘ':
                case 'ｘ':
                    normalKana = 'X';
                    return true;

                case 'Y':
                case 'y':
                case 'Ｙ':
                case 'ｙ':
                    normalKana = 'Y';
                    return true;

                case 'Z':
                case 'z':
                case 'Ｚ':
                case 'ｚ':
                    normalKana = 'Z';
                    return true;
            }

            normalKana = default(char);
            return false;
        }

        private static bool TryGetBaseChar(char smallKana, out char normalKana)
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

                case 'ゎ':
                case 'ヮ':
                    normalKana = 'わ';
                    return true;
            }

            normalKana = default(char);

            return false;
        }

        #endregion 文字

        [Route("image/{type}/{text}.svg")]
        public async Task<FormattedContentResult<SvgSvgElement>> GetSvgAsync(string type, string text)
            => await GetImageAsync(type, text, new SvgMediaTypeFormatter());

        [Route("image/{type}/{text}.png")]
        public async Task<FormattedContentResult<SvgSvgElement>> GetPngAsync(string type, string text)
            => await GetImageAsync(type, text, new PngMediaTypeFormatter());

        [Route("image/{type}/{text}")]
        public async Task<FormattedContentResult<SvgSvgElement>> GetUnknownTypeAsync(string type, string text)
        {
            MediaTypeFormatter formatter = null;
            var ofn = GetOriginalFileName();

            if (ofn != null)
            {
                switch (Path.GetExtension(ofn)?.ToLowerInvariant())
                {
                    case ".png":
                        formatter = new PngMediaTypeFormatter();
                        break;
                        //case ".svg":
                        //    formatter = new SvgMediaTypeFormatter();
                        //    break;
                }
            }

            return await GetImageAsync(type, ofn != null ? Path.GetFileNameWithoutExtension(ofn) : text, formatter ?? new SvgMediaTypeFormatter());
        }

        private string GetOriginalFileName()
        {
            var ou = HttpContext.Current.Request.Headers["X-Original-URL"];

            if (string.IsNullOrEmpty(ou))
            {
                return null;
            }

            var uri = new Uri(HttpContext.Current.Request.Url, ou);

            return Uri.UnescapeDataString(Path.GetFileName(uri.AbsolutePath));
        }

        private async Task<FormattedContentResult<SvgSvgElement>> GetImageAsync(string type, string text, MediaTypeFormatter formatter)
        {
            var svg = await GetImageAsync(type, text, 0.4f);
            if (svg == null)
            {
                var ofn = GetOriginalFileName();

                if (ofn != null)
                {
                    var ot = Path.GetFileNameWithoutExtension(ofn); if (ot != text)
                    {
                        return await GetImageAsync(type, ot, formatter);
                    }
                }

                HttpContext.Current.Request.SaveAs(Path.GetTempFileName(), true);
                throw new HttpException(404, "指定されたフォントは存在しません。");
            }
            return Content(System.Net.HttpStatusCode.OK, svg, formatter);
        }

        private static float GetDefaultMargin(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            if (Regex.IsMatch(text, "^[0-9０-９ ]+$"))
            {
                return 0.4f;
            }

            switch (text.Length)
            {
                case 1:
                case 2:
                    return 2;
                case 3:
                case 4:
                    return 1.2f;
                default:
                    return 1;
            }
        }

        public async Task<SvgSvgElement> GetImageAsync(string type, string text, float margin)
        {
            SvgSvgElement elem = null;

            foreach (var baseChar in text)
            {
                char canonical;

                float scale, dy;

                if (TryGetBaseChar(baseChar, out canonical))
                {
                    scale = 2f / 3;
                    dy = 100f / 3;
                }
                else if (TryGetCanonicalChar(baseChar, out canonical))
                {
                    scale = 1;
                    dy = 0;
                }
                else
                {
                    return null;
                }

                var e = await GetTemplateAsync(type, canonical);

                if (e == null)
                {
                    return null;
                }

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

                    var dx = pb.Right + 100 * margin - eb.Left;

                    foreach (var ce in cloned.Items)
                    {
                        var nc = ce.Clone();
                        (nc as SvgDrawingElement)?.Translate(dx, dy);

                        elem.AddChild(nc);
                    }
                }
            }
            var cb = elem.ChildBounds;
            elem.Width = (float)Math.Ceiling((double)cb.Width);
            elem.Height = (float)Math.Ceiling((double)cb.Height);
            foreach (var ce in elem.Items)
            {
                (ce as SvgDrawingElement).Translate(-cb.Left, -cb.Top);
            }

            return elem;
        }
    }
}