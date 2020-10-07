using System;
using Grpc.Core;
using Slither.Environment;
using Slither.Proto;


namespace Slither.Client
{
    public class GrpcClient : IClient
    {
        readonly SlitherTrainer.SlitherTrainerClient client;

        public GrpcClient(Channel channel)
        {

            this.client = new SlitherTrainer.SlitherTrainerClient(channel);
        }

        public Moves GetNextMove(IEnvironment environment)
        {
            var request = new MoveRequest();
            request.Image.Add(environment.ScreenState);

            var response = client.NextMove(request);

            return response.Action;
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

        public void SendResults(float reward, IEnvironment currentState, Moves nextMove, IEnvironment nextState)
        {
            var request = new RememberRequest{
                Action = nextMove,
                Reward = reward,
                Died = reward < 0.0,
            };
            request.CurrentImage.Add(currentState.ScreenState);
            request.NextImage.Add(nextState.ScreenState);

            client.Remember(request);
        }

        public void StepUpdate(int step)
        {
            client.StepUpdate(new StepRequest{TotalStep = step});
        }
    }

}