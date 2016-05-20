using System;
using System.Linq;
using System.Collections.Generic;
using Shipwreck.Svg;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace Shipwreck.ShipNameFont.Services.Formatting
{
    internal sealed class SvgMediaTypeFormatter : MediaTypeFormatter
    {
        public SvgMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/svg+xml"));
            SupportedEncodings.Add(new UTF8Encoding(false));
        }

        public override bool CanReadType(Type type)
            => false;

        public override bool CanWriteType(Type type)
            => typeof(SvgElement).IsAssignableFrom(type);

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext) => Task.Run(() =>
        {
            using (var sw = new StreamWriter(writeStream, SelectCharacterEncoding(content?.Headers), 1024, true))
            {
                ((SvgElement)value).ToElement().Save(sw);
            }
        });
    }

}