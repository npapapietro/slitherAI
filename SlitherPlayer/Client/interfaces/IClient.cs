using OpenQA.Selenium.Interactions;
using Slither.Environment;
using Slither.Proto;
using Slither.Runtime;

namespace Slither.Client
{
    public interface IClient
    {
        void SendReset(float reward, IEnvironment state);

        Moves GetNextMove(IEnvironment environment);

        void SendReward(float reward, IEnvironment state);
    }
}