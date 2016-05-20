using Shipwreck.Svg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shipwreck.ShipNameFont.Services.Formatting
{
    internal sealed class PngMediaTypeFormatter : MediaTypeFormatter
    {
        public PngMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("image/png"));
        }

        public override bool CanReadType(Type type)
            => false;

        public override bool CanWriteType(Type type)
            => typeof(SvgElement).IsAssignableFrom(type);

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var tcs = new TaskCompletionSource<int>();

            var thread = new Thread(() =>
            {
                try
                {
                    WriteImage((SvgElement)value, writeStream);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    return;
                }
                tcs.SetResult(0);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return tcs.Task;
        }

        private void WriteImage(SvgElement element, Stream stream)
        {
            var b = element.Bounds;

            var c = new Canvas();
            c.Width = b.Width;
            c.Height = b.Height;

            foreach (var d in element.Items.OfType<SvgDrawingElement>())
            {
                Geometry g = null;
                if (d is SvgRectElement)
                {
                    var r = (SvgRectElement)d;
                    var sg = new StreamGeometry();
                    using (var sc = sg.Open())
                    {
                        sc.BeginFigure(new System.Windows.Point(r.X, r.Y), r.Fill != null, true);
                        sc.LineTo(new System.Windows.Point(r.X + r.Width, r.Y), true, false);
                        sc.LineTo(new System.Windows.Point(r.X + r.Width, r.Y + r.Height), true, false);
                        sc.LineTo(new System.Windows.Point(r.X, r.Y + r.Height), true, false);
                    }
                    sg.Freeze();
                    g = sg;
                }
                else if (d is SvgPolygonElement)
                {
                    var r = (SvgPolygonElement)d;
                    var sg = new StreamGeometry();
                    using (var sc = sg.Open())
                    {
                        var fp = r.Points.First();
                        sc.BeginFigure(new System.Windows.Point(fp.X, fp.Y), r.Fill != null, true);

                        foreach (var p in r.Points.Skip(1))
                        {
                            sc.LineTo(new System.Windows.Point(p.X, p.Y), true, false);
                        }
                    }
                    sg.Freeze();
                    g = sg;
                }
                else if (d is SvgPathElement)
                {
                    var r = (SvgPathElement)d;
                    var sg = new StreamGeometry();
                    using (var sc = sg.Open())
                    {
                        var fi = 0;
                        for (var i = 0; i < r.D.Count; i++)
                        {
                            var cmd = r.D[i];

                            if (cmd.Command == 'Z' || cmd.Command == 'z')
                            {
                                var ip = new StreamGeometryIntercepter(sc, r.Fill != null, true);

                                ip.Execute(r.D.Skip(fi).Take(i - fi));

                                fi = i + 1;
                            }
                        }
                        if (fi < r.D.Count)
                        {
                            var ip = new StreamGeometryIntercepter(sc, r.Fill != null, false);

                            ip.Execute(r.D.Skip(fi));
                        }
                    }
                    sg.Freeze();
                    g = sg;
                }
                if (g != null)
                {
                    var path = new System.Windows.Shapes.Path();
                    path.Data = g;

                    if (d.Fill != null)
                    {
                        path.Fill = new SolidColorBrush((Color)new ColorConverter().ConvertFrom(d.Fill));
                    }
                    if (d.Stroke != null && d.StrokeWidth > 0)
                    {
                        path.Stroke = new SolidColorBrush((Color)new ColorConverter().ConvertFrom(d.Stroke));
                        path.StrokeThickness = d.StrokeWidth;
                    }

                    c.Children.Add(path);
                }
            }

            c.Measure(new System.Windows.Size(b.Width, b.Height));
            c.Arrange(new System.Windows.Rect(0, 0, b.Width, b.Height));

            var rtb = new RenderTargetBitmap((int)b.Width, (int)b.Height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(c);
            rtb.Freeze();

            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            using (var ms = new MemoryStream())
            {
                png.Save(ms);

                ms.Position = 0;

                ms.CopyTo(stream);
            }
        }

        private class StreamGeometryIntercepter : SvgPathCommandInterpreter
        {
            private readonly StreamGeometryContext _c;
            private readonly bool _Fill;
            private readonly bool _closed;

            public StreamGeometryIntercepter(StreamGeometryContext c, bool isFilled, bool isClosed)
            {
                _c = c;
                _Fill = isFilled;
                _closed = isClosed;
            }

            protected override void OnMoveTo(Point location)
            {
                _c.BeginFigure(new System.Windows.Point(location.X, location.Y), _Fill, _closed);
            }

            protected override void OnLineTo(Point start, Point stop)
            {
                _c.LineTo(new System.Windows.Point(stop.X, stop.Y), true, true);
            }

            protected override void OnCubicCurveTo(Point start, Point controlPoint1, Point controlPoint2, Point stop)
            {
                _c.PolyBezierTo(new[] { new System.Windows.Point(controlPoint1.X, controlPoint1.Y), new System.Windows.Point(controlPoint2.X, controlPoint2.Y), new System.Windows.Point(stop.X, stop.Y) }, true, true);
            }

            protected override void OnQuadraticCurveTo(Point start, Point controlPoint, Point stop)
            {
                _c.QuadraticBezierTo(new System.Windows.Point(controlPoint.X, controlPoint.Y), new System.Windows.Point(stop.X, stop.Y), true, true);
            }

            protected override void OnClosePath()
            {
            }
        }
    }
}