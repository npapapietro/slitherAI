using OpenQA.Selenium.Interactions;
using Slither.Environment;
using Slither.Proto;
using Slither.Runtime;

namespace Slither.Client
{
    public interface IClient
    {
        void SendReset(float score, int step, int run);

        void StepUpdate(int step);

        Moves GetNextMove(IEnvironmentState environment);

        void SendResults(float reward, IEnvironmentState currentState, Moves nextMove, IEnvironmentState nextState);
    }
}