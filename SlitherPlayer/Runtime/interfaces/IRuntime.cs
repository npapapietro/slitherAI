using OpenQA.Selenium.Interactions;
using Slither.Environment;

namespace Slither.Runtime
{
    public interface IRunTime
    {
        IEnvironment CurrentState(bool withWait=false);

        void Reset(float score, int step, int run);

        void Reset();

        void Run();

        float Move(Actions action, IEnvironment currentState, out IEnvironment nextState);

    }
}