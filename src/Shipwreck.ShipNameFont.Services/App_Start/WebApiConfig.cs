using Shipwreck.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;

namespace Shipwreck.ShipNameFont.Services
{
    internal class SvgMediaTypeFormatter : MediaTypeFormatter
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

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Add(new SvgMediaTypeFormatter());
            // Web API の設定およびサービス

            // Web API ルート
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
