using System;
using System.ComponentModel;
using FormsGtkLive.Views;
using SkiaSharp.Views.Forms;
using SkiaSharp.Views.Gtk;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;
using SKFormsView = FormsGtkLive.Views.SKCanvasView;

[assembly: ExportRenderer(typeof(SKFormsView), typeof(FormsGtkLive.GTK.SKGtkViewRenderer))]
namespace FormsGtkLive.GTK
{
    public class SKGtkViewRenderer : ViewRenderer<SKFormsView, SKWidget>
    {
        protected override void Dispose(bool disposing)
        {
            var control = Control;
            if (control != null)
            {
                control.PaintSurface -= OnPaintSurface;
            }
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SKFormsView> e)
        {
            if (e.OldElement != null)
            {
                (e.NewElement as ISKCanvasViewController).SurfaceInvalidated -= HandleSurfaceInvalidated;
            }
            if (e.NewElement != null)
            {
                // create the native view
                if (Control == null)
                {
                    var view = CreateNativeControl();
                    view.PaintSurface += OnPaintSurface;
                    SetNativeControl(view);
                }
                (e.NewElement as ISKCanvasViewController).SurfaceInvalidated += HandleSurfaceInvalidated;
            }
            base.OnElementChanged(e);
        }

        void OnPaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            // the control is being repainted, let the user know
            (Element as ISKCanvasViewController)?.OnPaintSurface(new SKPaintSurfaceEventArgs(e.Surface, e.Info));
        }

        protected virtual SKWidget CreateNativeControl()
        {
            return new SKWidget();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // FIXME: uncomment when supported
            /*
            if (e.PropertyName == SKFormsView.IgnorePixelScalingProperty.PropertyName)
            {
                Control.IgnorePixelScaling = Element.IgnorePixelScaling;
            }
            */
        }

        void HandleSurfaceInvalidated(object sender, EventArgs e)
        {
            QueueDraw();
        }
    }
}

