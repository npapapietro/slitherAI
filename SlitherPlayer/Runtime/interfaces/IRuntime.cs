using OpenQA.Selenium.Interactions;
using Slither.Environment;

namespace Slither.Runtime
{
    public interface IRunTime
    {
        IEnvironment CurrentState();

        int StepCount { get; }

        int RunCount { get; }

        void Reset(float reward, IEnvironment currentState);

        void Reset();

        void Run();

        float Move(Actions action, ref IEnvironment currentState);

    }
}