using System;
using System.Diagnostics;
using Grpc.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Slither.Client;
using Slither.Environment;
using Slither.Proto;
using Slither.ScreenCapture;

namespace Slither.Runtime
{
    public class SlitherPlayer : IRunTime, IDisposable
    {
        private readonly IWebDriver Driver;

        private readonly IClient gRPCClient;

        private readonly ScreenStream ImageStream;

        private readonly IEnvironment environment;

        private Coorindates MoveSet;

        public SlitherPlayer()
        {
            var options = new ChromeOptions();
            options.SetLoggingPreference(LogType.Client, LogLevel.Off);

            foreach (var opt in PlayerConfig.Options)
            {
                options.AddArgument(opt);
            }

            Driver = new ChromeDriver(options);

            gRPCClient = new GrpcClient(new Channel(PlayerConfig.Channel, ChannelCredentials.Insecure));

            ImageStream = new ScreenStream(Driver);

            environment = new Environment.Environment();

        }

        public IEnvironmentState CurrentState(bool withWait = false)
        {
            return environment.GetState(Driver, ImageStream, withWait);
        }

        private int MainLoop()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            Driver.Navigate().GoToUrl("http://slither.io");

            int x, y;
            while (!environment.GetCenter(Driver, out x, out y))
                MoveSet = new Coorindates(x, y, R: 20);

            ImageStream.Run();

            int total_step = 0;
            int run = 0;
            while (true)
            {

                if (run >= PlayerConfig.RunLimit) return 0;

                try
                {
                    while (!wait.Until(environment.GetPlayButton)) { }

                    Reset();

                    Console.WriteLine($"Entering game.");
                }
                catch (ElementClickInterceptedException)
                {
                    Console.WriteLine("Element Click intercepted, proceeding");
                }
                catch (Exception e)
                {

                    Console.WriteLine("Can't find or click play button " + e.Message);

                }

                var currentState = CurrentState(true);

                run++;
                int step = 0;
                int score = 0;
                Console.WriteLine($"Run {run}.");

                while (true)
                {
                    var stopWatch = System.Diagnostics.Stopwatch.StartNew();
                    // Console.WriteLine($"Step: {step}; Total Step {total_step}.");
                    if (total_step >= PlayerConfig.StepLimit)
                    {
                        return 0;
                    }
                    step++;
                    total_step++;

                    var nextMove = gRPCClient.GetNextMove(currentState);

                    var reward = Move(ConvertToSlitherMove(nextMove), currentState, out var nextState);
                    score += step;

                    gRPCClient.SendResults(reward, currentState, nextMove, nextState);
                    currentState = nextState;

                    gRPCClient.StepUpdate(total_step);

                    if (reward < 0.0)
                    {
                        Console.WriteLine($"Died, sending results.");
                        Reset(score, step, run);
                        break;
                    }
                    stopWatch.Stop();
                    Console.WriteLine(String.Format("|{0,10}|{1,10}|{2,10}|{3,10}|{4,10}|", "Step", "Total Step", "Next Move", "Reward", "Elapsed Loop"));
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine(String.Format("|{0,10}|{1,10}|{2,10}|{3,10}|{4,10}|", step, total_step, nextMove, reward, stopWatch.Elapsed.Seconds));
                }
            }
        }

        public void Run()
        {

            if (PlayerConfig.TestSelect)
            {
                TestSelection();
            }

            else
            {
                MainLoop();
            }
        }



        private void TestSelection()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            Driver.Navigate().GoToUrl("http://slither.io");
            while (!wait.Until(environment.GetPlayButton)) { }
            Reset();

            environment.GetCenter(Driver, out var xr, out var yr);
            MoveSet = new Coorindates(xr, yr, R: 10);

            while (Stopwatch.StartNew().ElapsedMilliseconds < 15 * 1000)
            {
                var x = Driver.FindElement(By.XPath(@"//canvas[@class='nsi' and @width > 500]"));
                Console.WriteLine(x.Size);
                var testMoveSet = new []{
                    Moves.Pi // left
                    ,Moves.Pi0 // right
                    ,Moves.Pi12 // up
                    ,Moves.Pi32 // down
                    ,Moves.Pi14 // upright
                    ,Moves.Pi54 // downleft
                    ,Moves.Pi34 // upleft
                    ,Moves.Pi74 // downright
                };
                foreach (int move in testMoveSet){
                    try
                    {
                        ConvertToSlitherMove(move, true).Build().Perform();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(move);
                        Console.WriteLine(e);
                        continue;
                    }
                }          
            }
        }
        
        public void Reset(float score, int step, int run)
        {
            gRPCClient.SendReset(score, step, run);
        }

        public void Reset()
        {

            if (environment.GetPlayButton(Driver, out var playButton))
            {
                playButton.Click();
            }
            else
            {
                throw new Exception("Play button not clickable during reset.");
            }
        }

        public float Move(Actions action, IEnvironmentState currentState, out IEnvironmentState nextState)
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
            ImageStream.Dispose();
            Driver.Dispose();

        }


        private Actions ConvertToSlitherMove(Moves move)
        {
            return ConvertToSlitherMove((int)move);

        }

        private Actions ConvertToSlitherMove(int move, bool verbose = false)
        {
            var action = new OpenQA.Selenium.Interactions.Actions(Driver);
            var elm = Driver.FindElement(By.XPath(@"//canvas[@class='nsi' and @width > 500]"));


            var (x, y) = MoveSet.PositionMapping[move];

            if (verbose)
            {
                Console.WriteLine($"Moving to point ({x},{y})");
            }

            return action.MoveToElement(elm).MoveByOffset(x, y);
        }

    }
}