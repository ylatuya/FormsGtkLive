using System;
using System.Linq;
using SkiaSharp;

namespace FormsGtkLive.Services
{
    public class EvalResult
    {
        public EvalMessage[] Messages;

        public Action<SKCanvas> Result;

        public bool HasErrors
        {
            get { return Messages != null && Messages.Any(m => m.MessageType == "error"); }
        }
    }
}

