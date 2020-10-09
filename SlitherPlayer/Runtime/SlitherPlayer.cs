using System;
using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
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

        public SlitherPlayer()
        {

            var options = new ChromeOptions();
            // var options = new FirefoxOptions();
            options.SetLoggingPreference(LogType.Client, LogLevel.Off);
            
            foreach (var opt in PlayerConfig.Options)
            {
                options.AddArgument(opt);
            }
            var channel = new Channel(PlayerConfig.Channel, ChannelCredentials.Insecure);
            

            // Driver = new FirefoxDriver(options);
            Driver = new ChromeDriver(options);
            gRPCClient = new GrpcClient(channel);
            Model = new Featurizer(PlayerConfig.ModelFile);
        }

        public IEnvironment CurrentState()
        {
            return Driver.GetState(Model);
        }

        private int MainLoop()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            Driver.Navigate().GoToUrl("http://slither.io");
            

            int total_step = 0;
            int run = 0;
            while (true)
            {

                if (run >= PlayerConfig.RunLimit) return 0;

                try
                {   
                    Console.WriteLine($"Entering game.");
                    while(!wait.Until(driver => driver.FindPlayButton().Displayed)){}
                    Reset();
                    Console.WriteLine($"Entering game.");
                }
                catch(ElementClickInterceptedException)
                {
                    Console.WriteLine("Element Click intercepted, proceeding");
                }
                catch(Exception e)
                {
 
                    Console.WriteLine("Can't find or click play button " + e.Message);

                }
               
                var currentState = CurrentState();

                run++;
                int step = 0;
                int score = 0;
                Console.WriteLine($"Run {run}.");

                while (true)
                {
                    Console.WriteLine($"Step: {step}; Total Step {total_step}.");
                    if (total_step >= PlayerConfig.StepLimit)
                    {
                        return 0;
                    }
                    step++;
                    total_step++;

                    var nextMove = gRPCClient.GetNextMove(currentState);
                    Console.WriteLine($"Next move is {nextMove}.");

                    var reward = Move(ConvertToSlitherMove(nextMove), currentState, out var nextState);
                    Console.WriteLine($"Reward is {reward}.");
                    score += step;

                    // gRPCClient.SendResults(reward, currentState, nextMove, nextState);
                    currentState = nextState;

                    // gRPCClient.StepUpdate(total_step);

                    if (reward < 0.0)
                    {
                        Console.WriteLine($"Died, sending results.");
                        // Reset(score, step, run);
                        break;
                    }
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
            if (PlayerConfig.TestSelect)
            {
                TestSelection();
                return;
            }
            MainLoop();
        }

        private void TestSelection()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            Driver.Navigate().GoToUrl("http://slither.io");
            wait.Until(driver => driver.FindPlayButton().Displayed);

            Driver.FindPlayButton().Click();

            var wait2 = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            wait2.Until(driver => driver.FindElements(By.XPath(@"//*[contains(text(),'Your length')]")).Count > 0);

            var result = Driver.FindElement(By.XPath(@"//*[contains(.,'Your length')]/span[2]")).Text;

        }

        private void TestConnection()
        {

            var fakeState = EnvironmentUtils.MockState();
            var moveResult = gRPCClient.GetNextMove(fakeState);
            Console.WriteLine(moveResult);

            gRPCClient.SendReset(0f, 0, 0);

            gRPCClient.StepUpdate(1);

            gRPCClient.SendResults(0f, fakeState, Moves.Boost, fakeState);
            
        }

        private void TestKeys()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            Driver.Navigate().GoToUrl("http://slither.io");
            wait.Until(driver => driver.FindPlayButton().Displayed);

            Driver.FindPlayButton().Click();

            while(true)
            {
                new OpenQA.Selenium.Interactions.Actions(Driver)
                    .SendKeys(Keys.Left)
                    .SendKeys(Keys.Left)
                    .SendKeys(Keys.Left)
                    .SendKeys(Keys.Left)
                    .Perform();
            }
        }

        public void Reset(float score, int step, int run)
        {
            gRPCClient.SendReset(score, step, run);           
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

        public float Move(Actions action, IEnvironment currentState, out IEnvironment nextState)
        {
            action.Perform();
            nextState = CurrentState();
 
            if (nextState.Dead)
            {
                return -1.0f;
            }
            else if (nextState.Length > currentState.Length)
            {
                return 3.0f;
            }

            return 1.0f;

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
                    return action.SendKeys(Keys.Left);
                case Moves.Right:
                    return action.SendKeys(Keys.Right);
                case Moves.Boost:
                    return action.SendKeys(Keys.Space);
                case Moves.BoostLeft:
                    return action.SendKeys(Keys.Up).SendKeys(Keys.Left);
                case Moves.BoostRight:
                    return action.SendKeys(Keys.Up).SendKeys(Keys.Right);
                case Moves.Wait:
                    return action;
                default:
                    throw new Exception("How did you get here??");
            }
        }
    }
}