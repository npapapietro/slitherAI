using OpenQA.Selenium.Interactions;
using Slither.Environment;

namespace Slither.Runtime
{
    public interface IRunTime
    {
        IEnvironmentState CurrentState(bool withWait=false);

        void Reset(float score, int step, int run);

        void Reset();

        void Run();

        float Move(Actions action, IEnvironmentState currentState, out IEnvironmentState nextState);

    }
}