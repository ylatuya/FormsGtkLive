using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.GTK;

namespace FormsGtkLive.GTK
{
    class Program
    {
        static void Main(string[] args)
        {
            Gtk.Application.Init();
            Forms.Init();
            var app = new AppLive();
            var window = new FormsWindow();
            window.LoadApplication(app);
            window.SetApplicationTitle("Live Skia Editor");
            window.SetApplicationIcon("Images/xaml.png");
            window.Show();
            Gtk.Application.Run();
        }
    }
}