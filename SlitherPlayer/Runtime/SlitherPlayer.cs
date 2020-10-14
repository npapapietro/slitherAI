using System;
using System.Diagnostics;
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
using Slither.ScreenCapture;

namespace Slither.Runtime
{
    public class SlitherPlayer : IRunTime, IDisposable
    {
        private readonly IWebDriver Driver;
        
        private readonly IClient gRPCClient;

        private readonly ScreenStream ImageStream;

        private Coorindates MoveSet;

        public SlitherPlayer()
        {
            var useFF = false;


            if (useFF)
            {
                var options = new FirefoxOptions();
                options.SetLoggingPreference(LogType.Client, LogLevel.Off);

                foreach (var opt in PlayerConfig.Options)
                {
                    options.AddArgument(opt);
                }

                Driver = new FirefoxDriver(options);
            }
            else
            {
                var options = new ChromeOptions();
                options.SetLoggingPreference(LogType.Client, LogLevel.Off);

                foreach (var opt in PlayerConfig.Options)
                {
                    options.AddArgument(opt);
                }

                Driver = new ChromeDriver(options);
            }

            gRPCClient = new GrpcClient(new Channel(PlayerConfig.Channel, ChannelCredentials.Insecure));
            
            ImageStream = new ScreenStream(Driver);

        }

        public IEnvironment CurrentState(bool withWait = false)
        {
            return Driver.GetState(ImageStream, withWait);
        }

        private int MainLoop()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            Driver.Navigate().GoToUrl("http://slither.io");

            var (x, y) = Driver.GetCenterCoordinates();
            MoveSet = new Coorindates(x, y, R: 20);

            ImageStream.Run();

            int total_step = 0;
            int run = 0;
            while (true)
            {

                if (run >= PlayerConfig.RunLimit) return 0;

                try
                {
                    Console.WriteLine($"Entering game.");
                    while (!wait.Until(driver => driver.FindPlayButton().Displayed)) { }
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
            if (PlayerConfig.TestgRPC)
            {
                TestConnection();

            }
            else if (PlayerConfig.TestSelect)
            {
                TestSelection();
            }
            else if (PlayerConfig.TestScreen)
            {
                TestScreen();
            }
            else if (PlayerConfig.TestKeys)
            {
                TestKeys();
            }
            else
            {
                MainLoop();
            }
        }

        private void TestScreen()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            Driver.Navigate().GoToUrl("http://slither.io");
            wait.Until(driver => driver.FindPlayButton().Displayed);
            Driver.FindPlayButton().Click();
            ImageStream.Run();
            while (Stopwatch.StartNew().ElapsedMilliseconds < 15 * 1000)
            {

            }

            Console.WriteLine(ImageStream.ScreenShots.Count);
        }

        private void TestSelection()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            Driver.Navigate().GoToUrl("http://slither.io");
            wait.Until(driver => driver.FindPlayButton().Displayed);

            Driver.FindPlayButton().Click();

            var (xx, y) = Driver.GetCenterCoordinates();
            MoveSet = new Coorindates(xx, y, R: 10);

            // var wait2 = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            // wait2.Until(driver => driver.FindElements(By.XPath(@"//*[contains(text(),'Your length')]")).Count > 0);

            // var result = Driver.FindElement(By.XPath(@"//*[contains(.,'Your length')]/span[2]")).Text;

            while (Stopwatch.StartNew().ElapsedMilliseconds < 15 * 1000)
            {
                var x = Driver.FindElement(By.XPath(@"//canvas[@class='nsi' and @width > 500]"));
                Console.WriteLine(x.Size);
                var r = new Random().Next(7);
                try
                {
                    ConvertToSlitherMove(r,true).Build().Perform();
                }
                catch(Exception e)
                {
                    Console.WriteLine(r);
                    Console.WriteLine(e);
                    continue;
                }
            }

        }

        private void TestConnection()
        {

            var fakeState = EnvironmentUtils.MockState();
            var moveResult = gRPCClient.GetNextMove(fakeState);
            Console.WriteLine(moveResult);

            gRPCClient.SendReset(0f, 0, 0);

            gRPCClient.StepUpdate(1);

            // gRPCClient.SendResults(0f, fakeState, Moves.Boost, fakeState);

        }


        private void TestKeys()
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            Driver.Navigate().GoToUrl("http://slither.io");
            wait.Until(driver => driver.FindPlayButton().Displayed);

            Driver.FindPlayButton().Click();

            while (true)
            {
                new OpenQA.Selenium.Interactions.Actions(Driver)
                    .MoveByOffset(15, -20)
                    .Perform();
                new OpenQA.Selenium.Interactions.Actions(Driver)
                    .MoveByOffset(-15, 20)
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
            ImageStream.Dispose();
            Driver.Dispose();
            
        }


        private Actions ConvertToSlitherMove(Moves move)
        {
            return ConvertToSlitherMove((int) move);

        }

        private Actions ConvertToSlitherMove(int move, bool verbose=false)
        {
            var action = new OpenQA.Selenium.Interactions.Actions(Driver);
            var elm = Driver.FindElement(By.XPath(@"//canvas[@class='nsi' and @width > 500]"));
            

            var (x, y) = MoveSet.PositionMapping[move];

            if (verbose)
            {
                Console.WriteLine($"Moving to point ({x},{y})");
            }
                        
            return action.MoveToElement(elm).MoveByOffset(x,y);
        }

    }
}