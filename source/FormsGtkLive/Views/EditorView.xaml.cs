using System.ComponentModel;
using FormsGtkLive.ViewModels;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace FormsGtkLive.Views
{
    public partial class EditorView : ContentPage
    {
        EditorViewModel vm;

        public EditorView()
        {
            InitializeComponent();

            BindingContext = vm = new EditorViewModel();
            vm.PropertyChanged += HandlePropertyChanged;
        }

        private void OnPaintSample(object sender, SKPaintSurfaceEventArgs e)
        {
            vm.PaintCommand.Execute(e);
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(vm.HasErrors))
            {
                if (!vm.HasErrors)
                {
                    canvas.InvalidateSurface();
                }
            }
        }

    }
}