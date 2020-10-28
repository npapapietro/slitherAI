using Grpc.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Slither.Proto;
using SlitherPlayer.Environment;
using SlitherPlayer.GRPC;
using SlitherPlayer.ScreenCapture;
using System;
using System.Threading;

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
            driver = ConfigureDriver();
            client = ConfigureClient();
            screenStream = ConfigureThread();
            environment = ConfigureEnvmnt();
            coordinates = ConfigureCoords();
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
            options.SetLoggingPreference(LogType.Client, LogLevel.Off);

            foreach (var opt in RuntimeConfigurations.Options)
            {
                options.AddArgument(opt);
            }

            return new ChromeDriver(options);
        }

        private IClient ConfigureClient()
        {
            var channel = new Channel(RuntimeConfigurations.Channel, ChannelCredentials.Insecure);

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
            driver.Navigate().GoToUrl("http://slither.io");
            
            wait.Until(environment.GetPlayButton);
            screenStream.Run(3);

            int totalSteps = 0;
            int runCount = 0;

            while (true)
            {
                if (runCount >= RuntimeConfigurations.RunLimit) return;

                try
                {
                    IWebElement playButton;
                    while(!environment.GetPlayButton(driver, out playButton)){}
                    Reset(playButton);

                    if (RuntimeConfigurations.Verbose) Console.WriteLine("Entering Game");
                }
                catch (ElementClickInterceptedException)
                {
                    if (RuntimeConfigurations.Verbose) Console.WriteLine("Element Click intercepted, proceeding");
                }
                catch (Exception e)
                {
                    if (RuntimeConfigurations.Verbose) Console.WriteLine("Can't find or click play button " + e.Message);

                    // Unhandled error
                    throw;
                }

                var currentState = CurrentState(true);
                runCount++;
                int step = 0;
                float score = 0;

                if (RuntimeConfigurations.Verbose) Console.WriteLine($"Beginning run: {runCount}");

                while (true)
                {
                    if (totalSteps >= RuntimeConfigurations.StepLimit) return;

                    step++;
                    totalSteps++;

                    var nextMove = client.GetNextMove(currentState);
                    PerformAction(nextMove);

                    var reward = AssesReward(currentState, out var nextState);
                    score += reward;

                    client.SendResults(reward, currentState, nextMove, nextState);
                    currentState = nextState;

                    client.StepUpdate(totalSteps);

                    if (reward <= 0.0f)
                    {
                        if (RuntimeConfigurations.Verbose) Console.WriteLine("Died, sending results");
                        client.SendReset(score, step, runCount);
                        break;
                    }
                }
            }
        }

        float AssesReward(IEnvironmentState currentState, out IEnvironmentState nextState)
        {
            
            nextState = CurrentState();

            if (nextState.Dead)
            {
                return 0.0f;
            }
            else if (nextState.Length > currentState.Length)
            {
                return 100.0f;
            }

            return 1.0f;

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

                if (RuntimeConfigurations.Verbose)
                    Console.WriteLine($"Moving from ({elm.Location.X}, {elm.Location.Y}) ==> ({x},{y}) with angle {nextMove.Action}" + (nextMove.Boost ? " with boost" : ""));

                action = action.MoveToElement(elm).MoveByOffset(x, y);
            }

            if (nextMove.Boost) action = action.Release();
            
            action.Build().Perform();

        }
    }

    #region Configurations
    /// <summary>
    /// Inline the config for now, move to a toml shared by python and c# later
    /// </summary>
    public static class RuntimeConfigurations
    {
        public static string Channel = "localhost:50051";
        public static string ModelFile = @"C:\Users\Nate-PC\Documents\git\Slither\ResNet50.onnx";
        public static string[] Options = {
                "--window-size=1000x1000",
                // "--headless"
            };
        public static int StepLimit = 5000000;
        public static int RunLimit = int.MaxValue;
        public static bool TestSelect = false;
        public static bool Verbose = true;

    }
    #endregion
}
