using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Slither.Client;
using Slither.Environment;
using Slither.Models;
using Slither.Proto;

namespace Slither.Runtime
{
    public class SlitherPlayer : IRunTime, IDisposable
    {
        private readonly IWebDriver Driver;
        private readonly IFeaturizer Model;
        private readonly IClient gRPCClient;

        public int StepCount { get; private set; }

        public int RunCount { get; private set; }

        public SlitherPlayer()
        {

            var options = new ChromeOptions();
            foreach (var opt in PlayerConfig.Options)
            {
                options.AddArgument(opt);
            }
            var channel = new Channel(PlayerConfig.Channel, ChannelCredentials.Insecure);


            Driver = new ChromeDriver(options);
            gRPCClient = new GrpcClient(channel);
            Model = new Featurizer(PlayerConfig.ModelFile);
            RunCount = 0;
            StepCount = 0;
        }

        public SlitherPlayer(ChromeDriver driver, IClient client, IFeaturizer model)
        {
            Driver = driver;
            gRPCClient = client;
            Model = model;
            RunCount = 0;
            StepCount = 0;
        }

        public IEnvironment CurrentState()
        {
            return Driver.GetState(Model);
        }

        private void MainLoop()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            Driver.Navigate().GoToUrl("http://slither.io");
            wait.Until(driver => driver.FindPlayButton().Displayed);

            int total_step = 0;
            while (true)
            {
                RunCount++;
                Reset();

                IEnvironment currentState = CurrentState();

                int step = 0;
                int score = 0;

                while (true)
                {
                    step++;
                    total_step++;

                    var nextMove = ConvertToSlitherMove(gRPCClient.GetNextMove(currentState));
                    var reward = Move(nextMove, ref currentState);
                    score += step;

                    if (reward < 0.0)
                    {
                        Reset(reward, currentState);
                        break;
                    }

                    gRPCClient.SendReward(reward, currentState);
                }
            }
        }

        public void Run()
        {
            if (PlayerConfig.TestgRPC)
            {
                TestConnection();
                return;
            }
            MainLoop();
        }

        private void TestConnection()
        {

            var fakeState = EnvironmentUtils.MockState();
            var moveResult = gRPCClient.GetNextMove(fakeState);

            gRPCClient.SendReward(3.0f, fakeState);

            gRPCClient.SendReset(3.0f, fakeState);
        }

        public void Reset(float reward, IEnvironment currentState)
        {
            gRPCClient.SendReset(reward, currentState);

            var playButton = Driver.FindPlayButton();
            if (playButton.Displayed)
            {
                playButton.Click();
            }
            else
            {
                throw new Exception("Play button not clickable during reset.");
            }
        }

        public void Reset()
        {
            var playButton = Driver.FindPlayButton();
            if (playButton.Displayed)
            {
                playButton.Click();
            }
            else
            {
                throw new Exception("Play button not clickable during reset.");
            }
        }

        public float Move(Actions action, ref IEnvironment currentState)
        {
            action.Perform();
            var nextState = CurrentState();
            var reward = -1.0f;

            if (nextState.Dead)
            {
                reward = -1.0f;
            }
            else if (nextState.Length > currentState.Length)
            {
                reward = 3.0f;
            }

            currentState = nextState;
            return reward;

        }

        public void Dispose()
        {
            Driver.Dispose();
            Model.Dispose();
        }


        private Actions ConvertToSlitherMove(Moves move)
        {
            var action = new OpenQA.Selenium.Interactions.Actions(Driver);
            switch (move)
            {
                case Moves.Left:
                    return action.SendKeys(Keys.ArrowLeft);
                case Moves.Right:
                    return action.SendKeys(Keys.ArrowRight);
                case Moves.Boost:
                    return action.SendKeys(Keys.Space);
                case Moves.BoostLeft:
                    return action.SendKeys(Keys.Space).SendKeys(Keys.ArrowLeft);
                case Moves.BoostRight:
                    return action.SendKeys(Keys.Space).SendKeys(Keys.ArrowRight);
                case Moves.Wait:
                    return null; // Should I return null??
                default:
                    throw new Exception("How did you get here??");
            }
        }

    }
}