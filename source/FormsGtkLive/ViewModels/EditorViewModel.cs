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
        string errorMessage;
        bool hasErrors;

        public string LiveCode
        {
            get => _liveCode;
            set
            {
                _liveCode = value;
                Preview();
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            protected set
            {
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool HasErrors
        {
            get => hasErrors;
            protected set
            {
                hasErrors = value;
                OnPropertyChanged(nameof(HasErrors));
            }
        }

        public ICommand PaintCommand => new Command<SKPaintSurfaceEventArgs>(Paint);

        public ICommand PreviewCommand => new Command(() => Preview());

        public void Paint(SKPaintSurfaceEventArgs args)
        {
            if (_result == null || _result.HasErrors) return;

            try
            {
                _result?.Result?.Invoke(args.Surface.Canvas);
            }
            catch (Exception ex)
            {
                HasErrors = true;
                ErrorMessage = ex.Message;
            }
        }

        async Task Preview()
        {
            _result = await DependencyService.Get<IEvaluator>().Evaluate(LiveCode);
            ErrorMessage = FormatError();
            HasErrors = _result.HasErrors || _result.Messages.Any(m => m.MessageType == "error");
        }

        string FormatError()
        {
            if (_result == null || !_result.HasErrors || _result.Messages.Length == 0)
                return "";

            var message = _result.Messages[0];

            return $"{message.Text} line:{message.Line} col:{message.Column}";
        }

    }
}