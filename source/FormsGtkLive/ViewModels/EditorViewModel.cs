using System.Windows.Input;
using System.Linq;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using FormsGtkLive.Services;
using System.Threading.Tasks;
using System;

namespace FormsGtkLive.ViewModels
{
    public class EditorViewModel : BindableObject
    {
        string _liveCode;
        EvalResult _result;

        public string LiveCode
        {
            get
            {
                return _liveCode;
            }
            set
            {
                _liveCode = value;
                Preview();
            }
        }

        public string ErrorMessage => FormatError();

        private string FormatError()
        {
            if (_result == null || !_result.HasErrors || _result.Messages.Length == 0)
                return "";

            var message = _result.Messages[0];

            return $"{message.Text} line:{message.Line} col:{message.Column}";
        }

        public bool HasErrors => _result?.Messages.Any(m => m.MessageType == "error") ?? false;

        public ICommand PaintCommand => new Command<SKPaintSurfaceEventArgs>(Paint);

        public ICommand PreviewCommand => new Command(() => Preview());

        async Task Preview()
        {
            _result = await DependencyService.Get<IEvaluator>().Evaluate(LiveCode);
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(ErrorMessage));
        }

        public void Paint(SKPaintSurfaceEventArgs args)
        {
            if (_result?.HasErrors ?? false)
            {

            }
            else
            {
                _result?.Result?.Invoke(args.Surface.Canvas);
            }
        }
    }
}