using Grpc.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SlitherPlayer.Logger;
using Slither.Proto;
using SlitherPlayer.Environment;
using SlitherPlayer.GRPC;
using SlitherPlayer.ScreenCapture;
using System;
using SlitherPlayer;
using System.Threading;

using LogLevel = SlitherPlayer.Logger.LogLevel;
namespace SlitherPlayer
{
    public class Runtime : IDisposable
    {


        readonly IWebDriver driver;
        readonly IClient client;
        readonly IScreenThread screenStream;
        readonly ICoordinates coordinates;
        readonly IEnvironment environment;

        public Runtime(IWebDriver driver, IClient client, IScreenThread screenStream, ICoordinates coordinates, IEnvironment environment)
        {
            this.driver = driver;
            this.client = client;
            this.screenStream = screenStream;
            this.coordinates = coordinates;
            this.environment = environment;
        }

        public Runtime()
        {
            driver          = ConfigureDriver();
            client          = ConfigureClient();
            screenStream    = ConfigureThread();
            environment     = ConfigureEnvmnt();
            coordinates     = ConfigureCoords();
        }

        public void Run()
        {
            MainLoop();
        }

        public void Dispose()
        {
            driver.Dispose();
            screenStream.Dispose();
        }

        private IWebDriver ConfigureDriver()
        {
            var options = new ChromeOptions();

            foreach (var opt in Configurations.Options)
            {
                options.AddArgument(opt);
            }

            return new ChromeDriver(options);
        }

        private IClient ConfigureClient()
        {
            var channel = new Channel(Configurations.Channel, ChannelCredentials.Insecure);

            return new Client(channel);
        }

        private IScreenThread ConfigureThread()
        {
            return new ScreenCapture.ScreenCapture(driver);
        }

        private IEnvironment ConfigureEnvmnt()
        {
            return new SlitherEnvironment();
        }

        private ICoordinates ConfigureCoords()
        {
            return new Coordinates();
        }

        private void MainLoop()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            PlayerLogger.Info("Navigating to website.");
            driver.Navigate().GoToUrl("http://slither.io");
            try
            {
                wait.Until(environment.GetPlayButton);
                screenStream.Run(3);

                int totalSteps = 0;
                int runCount = 0;

                while (true)
                {
                    if (runCount >= Configurations.RunLimit) return;

                    try
                    {
                        IWebElement playButton;
                        while (!environment.GetPlayButton(driver, out playButton)) { }
                        Reset(playButton);

                        PlayerLogger.Info("Entering Game");
                    }
                    catch (ElementClickInterceptedException)
                    {
                        PlayerLogger.HandledError("Element Click intercepted, proceeding");
                    }
                    catch (ElementNotInteractableException)
                    {
                        PlayerLogger.HandledError("Element Click not interactable, waiting");
                        Thread.Sleep(2);
                        continue;
                    }
                    catch (Exception e)
                    {
                        PlayerLogger.Error("Can't find or click play button " + e.Message);
                        throw;
                    }

                    PlayerLogger.Info("Getting current state.");
                    var currentState = CurrentState(true);
                    runCount++;
                    int step = 0;
                    float score = 0;

                    PlayerLogger.Info($"Beginning run: {runCount}");

                    while (true)
                    {
                        if (totalSteps >= Configurations.StepLimit)
                        {
                            PlayerLogger.Info("Over step limit.");
                            return;
                        }

                        step++;
                        totalSteps++;

                        PlayerLogger.Info("Getting next move.");
                        var nextMove = client.GetNextMove(currentState);

                        PlayerLogger.Info("Performing next move.");
                        PerformAction(nextMove);

                        PlayerLogger.Info("Access reward.");
                        var reward = AccessReward(currentState, out var nextState);
                        score += reward;

                        PlayerLogger.Info("Sending results.");
                        client.SendResults(reward, currentState, nextMove, nextState);
                        currentState = nextState;

                        PlayerLogger.Info("Sending results.");
                        client.StepUpdate(totalSteps);

                        if (currentState.Dead)
                        {
                            PlayerLogger.Info("Died, sending results");
                            client.SendReset(score, step, runCount);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PlayerLogger.Error("Main loop unhandled: " + e.Message);
                driver.Dispose();
                return;

            }
        }

        float AccessReward(IEnvironmentState currentState, out IEnvironmentState nextState)
        {
            // float reward;
            nextState = CurrentState();

            return nextState.Length - currentState.Length;
            // if (nextState.Dead)
            // {
            //     reward = -1f;
            // }
            // else if (nextState.Length - currentState.Length > 0f)
            // {
            //     reward = 1f;
            // }
            // else if (nextState.Length - currentState.Length < 0f)
            // {
            //     reward = -0.5f;
            // }
            // else
            // {
            //     reward = 0.0f;
            // }

            // return reward;

        }

        void Reset(IWebElement playButton)
        {
            playButton.Click();
            Thread.Sleep(500);
        }

        IEnvironmentState CurrentState(bool withWait = false)
        {
            return environment.GetState(driver, screenStream, withWait);
        }

        void PerformAction(MoveResponse nextMove)
        {

            var elm = driver.FindElement(By.XPath(@"//canvas[@class='nsi' and @width > 500]"));
            var action = new Actions(driver);

            if (nextMove.Boost) action = action.ClickAndHold();

            if (nextMove.Action != Moves.NoMove)
            {
                var (x, y) = coordinates.PositionMapping[(int)nextMove.Action];

                PlayerLogger.Info($"Moving from ({elm.Location.X}, {elm.Location.Y}) ==> ({x},{y}) with angle {nextMove.Action}" + (nextMove.Boost ? " with boost" : ""));

                action = action.MoveToElement(elm).MoveByOffset(x, y);
            }

            if (nextMove.Boost) action = action.Release();

            action.Build().Perform();

        }
    }

}
