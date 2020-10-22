using Slither.Proto;
using SlitherPlayer.Environment;

namespace SlitherPlayer.GRPC
{
    public interface IClient
    {
        void SendReset(float score, int step, int run);

        void StepUpdate(int step);

        MoveResponse GetNextMove(IEnvironmentState environment);

        void SendResults(float reward, IEnvironmentState currentState, MoveResponse nextMove, IEnvironmentState nextState);
    }
}