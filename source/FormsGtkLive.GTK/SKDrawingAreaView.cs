using System;
using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Cairo;

namespace FormsGtkLive.GTK
{
    public class SKDrawingAreaView : DrawingArea
    {
        public event EventHandler<SKPaintSurfaceEventArgs> PaintSurface;

        SKImageInfo info;
        SKBitmap bitmap;
        bool ignorePixelScaling;

        public SKDrawingAreaView()
        {
            Initialize();
        }

        private void Initialize()
        {
            // create the initial info
            info = new SKImageInfo(0, 0, SKColorType.Rgba8888, SKAlphaType.Premul);
        }

        public bool IgnorePixelScalling { get; set; }

        public SKSize CanvasSize => info.Size;

        public bool IgnorePixelScaling
        {
            get { return ignorePixelScaling; }
            set
            {
                ignorePixelScaling = value;
                UpdateCanvasSize(info.Width, info.Height);
                QueueDraw();
            }
        }

        protected override bool OnExposeEvent(Gdk.EventExpose evnt)
        {
            IntPtr len;
            // create a surface
            using (var surface = SKSurface.Create(bitmap.Info.Width,
                                                  bitmap.Info.Height,
                                                  info.ColorType,
                                                  info.AlphaType,
                                                  bitmap.GetPixels(out len),
                                                  bitmap.Info.RowBytes))
            {
                // draw using SkiaSharp
                OnDraw(surface, info);

                surface.Canvas.Flush();

                using (Context cr = Gdk.CairoHelper.Create(GdkWindow))
                {
                    var csurface = new ImageSurface(
                            bitmap.GetPixels(out len),
                        Format.Rgb24,
                        bitmap.Width, bitmap.Height,
                        bitmap.Width * 4);

                    csurface.MarkDirty();
                    cr.SetSourceSurface(csurface, 0, 0);
                    cr.Paint();
                }
            }

            return true;
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation)
        {
            base.OnSizeAllocated(allocation);
            UpdateCanvasSize(allocation.Width, allocation.Height);
        }

        private void UpdateCanvasSize(int w, int h)
        {
            info.Width = w;
            info.Height = h;
            FreeBitmap();
            bitmap = new SKBitmap(info);
        }

        protected virtual void OnDraw(SKSurface surface, SKImageInfo info)
        {
            PaintSurface?.Invoke(this, new SKPaintSurfaceEventArgs(surface, info));
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();
            FreeBitmap();
        }

        void FreeBitmap()
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }
    }
}

