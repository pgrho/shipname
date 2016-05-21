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
using Shipwreck.ShipNameFont.Services.Formatting;
using System.Web.Http.Routing;

namespace Shipwreck.ShipNameFont.Services
{
    public static class WebApiConfig
    {

        private sealed class CustomHttpRoute : HttpRoute
        {
            public CustomHttpRoute(string routeTemplate, HttpRouteValueDictionary defaults)
                : base(routeTemplate, defaults)
            {
            }

            public override IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request)
            {
                var om = GetOriginalMessage(request);

                if (om != null)
                {
                    return base.GetRouteData(virtualPathRoot, om);
                }


                return base.GetRouteData(virtualPathRoot, request);
            }

            public override IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values)
            {
                var om = GetOriginalMessage(request);

                if (om != null)
                {
                    return base.GetVirtualPath(om, values);
                }

                return base.GetVirtualPath(request, values);
            }

            private static HttpRequestMessage GetOriginalMessage(HttpRequestMessage request)
            {
                object obj;
                if (request.Properties.TryGetValue($"{nameof(CustomHttpRoute)}_{nameof(HttpRequestMessage)}", out obj) && obj is HttpRequestMessage)
                {
                    return (HttpRequestMessage)obj;
                }
                IEnumerable<string> originalUrls;

                if (request.Headers.TryGetValues("X-Original-URL", out originalUrls))
                {
                    var url = originalUrls?.FirstOrDefault();

                    if (url != request.RequestUri.AbsolutePath)
                    {
                        var hrm = new HttpRequestMessage(request.Method, new Uri(request.RequestUri, url));
                        hrm.Content = request.Content;
                        hrm.Version = request.Version;

                        foreach (var kv in request.Headers)
                        {
                            hrm.Headers.Add(kv.Key, kv.Value);
                        }

                        request.Properties[$"{nameof(CustomHttpRoute)}_{nameof(HttpRequestMessage)}"] = hrm;

                        return hrm;
                    }
                }
                return null;
            }
        }
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Add(new SvgMediaTypeFormatter());
            config.Formatters.Add(new PngMediaTypeFormatter());
            // Web API の設定およびサービス

            // Web API ルート
            config.MapHttpAttributeRoutes();

            config.Routes.Add(
                "ImageApi",
                new CustomHttpRoute("image/{type}/{text}.{extension}", new HttpRouteValueDictionary(new { controller = "Images" })));


            config.Routes.Add(
                "ImageFileNameApi",
                new CustomHttpRoute("image/{type}/{fileName}", new HttpRouteValueDictionary(new { controller = "Images" })));


            //config.Routes.Add(
            //    "PngApi",
            //    new CustomHttpRoute("image/{type}/{text}", new HttpRouteValueDictionary(new { controller = "Images", text = RouteParameter.Optional })));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
