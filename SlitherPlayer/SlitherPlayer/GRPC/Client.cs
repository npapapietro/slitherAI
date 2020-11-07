using Grpc.Core;
using Slither.Proto;
using SlitherPlayer.Environment;

namespace SlitherPlayer.GRPC
{
    public class Client: IClient
    {
        readonly SlitherTrainer.SlitherTrainerClient client;

        public Client(Channel channel) => client = new SlitherTrainer.SlitherTrainerClient(channel);

        public MoveResponse GetNextMove(IEnvironmentState state)
        {
            var request = new MoveRequest();
            request.Image.Add(state.ScreenState);

            return client.NextMove(request);
        }

        public void SendReset(float score, int step, int run)
        {
            var request = new ResetRequest
            {
                Score = score,
                Step = step,
                Run = run
            };

            client.Reset(request);
        }

        public void SendResults(float reward, IEnvironmentState currentState, MoveResponse nextMove, IEnvironmentState nextState)
        {
            var request = new RememberRequest
            {
                Action = nextMove.Action,
                Reward = reward,
                Died = reward < 0.0,
                DidBoost = nextMove.Boost,
                Guid = currentState.Id.ToString()
            };
            request.CurrentImage.Add(currentState.ScreenState);
            request.NextImage.Add(nextState.ScreenState);

            client.Remember(request);
        }

        public void StepUpdate(int step)
        {
            client.StepUpdate(new StepRequest { TotalStep = step });
        }

    }
}