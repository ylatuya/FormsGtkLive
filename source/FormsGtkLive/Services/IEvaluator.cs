using System.Threading.Tasks;

namespace FormsGtkLive.Services
{

    public interface IEvaluator
    {
        Task<EvalResult> Evaluate(string code);
    }

}

