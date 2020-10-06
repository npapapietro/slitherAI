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

        public void SendReset()
        {
            throw new NotImplementedException();
        }

        public void SendReset(float reward, IEnvironment state)
        {
            var resetRequest = new RewardRequest{
                State = ToRequest(state),
                Reward = reward
            };
            client.Reset(resetRequest);
        }

        public void SendReward(float reward, IEnvironment state)
        {
            var rewardRequest = new RewardRequest{
                State = ToRequest(state),
                Reward = reward
            };
            client.Reward(rewardRequest);
            
        }

        public Moves GetNextMove(IEnvironment environment)
        {

            var request = ToRequest(environment);
            var response = client.NextMove(request);

            return response.Action;
        }

        private Request ToRequest(IEnvironment environment)
        {
            var request = new Request
            {
                Died = environment.Dead,
                TimeStamp = environment.TimeStamp,
                Length = environment.Length
            };
            request.Image.Add(environment.ScreenState);
            return request;
        }
    }

}