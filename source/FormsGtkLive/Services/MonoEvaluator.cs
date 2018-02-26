using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Mono.CSharp;
using SkiaSharp;

[assembly: Xamarin.Forms.Dependency(typeof(FormsGtkLive.Services.MonoEvaluator))]
namespace FormsGtkLive.Services
{

    public class MonoEvaluator : IEvaluator
    {
        Evaluator evaluator;
        readonly Printer printer = new Printer();

        public Task<EvalResult> Evaluate(string code)
        {
            return Task.Run(() =>
            {
                object result = null;
                bool hasResult;

                Init();
                printer.Messages.Clear();

                var code1 = $"new Action<SKCanvas>((SKCanvas canvas) => {{ {code} }});";

                try
                {
                    object res = evaluator.Evaluate(code1, out result, out hasResult);
                }
                catch (InternalErrorException ex)
                {
                    evaluator = null;
                    printer.AddError(ex);
                }
                catch (Exception ex)
                {
                    // Sometimes Mono.CSharp fails when constructing failure messages
                    if (ex.StackTrace.Contains("Mono.CSharp.InternalErrorException"))
                    {
                        evaluator = null; // Force re-init
                    }
                    printer.AddError(ex);
                }

                return new EvalResult
                {
                    Messages = printer.Messages.ToArray(),
                    Result = result as Action<SKCanvas>,
                };

            });
        }

        void Init()
        {
            object res;
            bool hasRes;

            if (evaluator != null) return;

            CompilerSettings settings = new CompilerSettings();
            var context = new CompilerContext(settings, printer);
            evaluator = new Evaluator(context);

            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                AddReference(a);
            }

            evaluator.Evaluate("using System;", out res, out hasRes);
            evaluator.Evaluate("using System.Collections.Generic;", out res, out hasRes);
            evaluator.Evaluate("using System.Linq;", out res, out hasRes);
            evaluator.Evaluate("using System.IO;", out res, out hasRes);
            evaluator.Evaluate("using SkiaSharp;", out res, out hasRes);
        }

        void AddReference(Assembly a)
        {
            var name = a.GetName().Name;
            if (name == "mscorlib" || name == "System" || name == "System.Core") return;
            evaluator.ReferenceAssembly(a);
        }
    }

    class Printer : ReportPrinter
    {
        public readonly List<EvalMessage> Messages = new List<EvalMessage>();

        public override void Print(AbstractMessage msg, bool showFullPath)
        {
            var line = 0;
            var column = 0;
            try
            {
                line = msg.Location.Row;
                column = msg.Location.Column;
            }
            catch
            {
                //Log (ex);
            }
            var m = new EvalMessage
            {
                MessageType = msg.MessageType,
                Text = msg.Text,
                Line = line,
                Column = column,
            };

            Messages.Add(m);

            //
            // Print it to the console if there's an error
            //
            if (msg.MessageType == "error")
            {
                var tm = msg.Text;
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                   Console.WriteLine("ERROR: {0}", tm));
            }
        }

        public void AddError(Exception ex)
        {
            var text = ex.ToString();

            var m = new EvalMessage
            {
                MessageType = "error",
                Text = text,
                Line = 0,
                Column = 0,
            };

            Messages.Add(m);

            //
            // Print it to the console if there's an error
            //
            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
               Console.WriteLine("EVAL ERROR: {0}", text));
        }
    }
}

