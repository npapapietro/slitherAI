using OpenQA.Selenium;
using SlitherPlayer.ScreenCapture;
using System;
using System.Threading;

namespace SlitherPlayer.Environment
{
    public class SlitherEnvironment: IEnvironment
    {
        private class EnvironmentState: IEnvironmentState
        {
            public float[] ScreenState { get; set; }

            public int Length { get; set; }

            public bool Dead { get; set; }

            public int TimeStamp { get; set; }

            public Guid Id {get; set;}

        }

        public IEnvironmentState GetState(IWebDriver driver, IScreenThread stream, bool withWait = false)
        {

            Thread.Sleep(withWait ? 1000 : 0);

            
            if (!GetSlitherLength(driver, out var length))
            {
                length = 10;
            }

            var playbuttonDisplayed = GetPlayButton(driver, out var playbutton) && (playbutton?.Displayed ?? false);

            var TimeStamp = DateTime.UtcNow.ToEpoch();
            
            var (screen, id) = stream.ClosestScreen(TimeStamp);

            return new EnvironmentState
            {
                TimeStamp = TimeStamp,
                Length = length,
                Dead = playbuttonDisplayed,
                ScreenState = screen,
                Id = id
            };

        }

        public bool GetSlitherLength(IWebDriver driver, out int length)
        {
            length = -1;
            try
            {
                var lengthElment = driver.FindElement(By.XPath(@"//*[contains(.,'Your length')]/span[2]"));
                return int.TryParse(lengthElment.Text, out length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool GetPlayButton(IWebDriver driver, out IWebElement playButton)
        {
            playButton = null;
            try
            {
                playButton = driver.FindElement(By.XPath(@"//*[contains(text(),'Play')]"));
                return playButton?.Displayed ?? false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        public bool GetPlayButton(IWebDriver driver)
        {
            return GetPlayButton(driver, out _);
        }

        public bool GetCenter(IWebDriver driver, out int x, out int y)
        {
            (x, y) = (0, 0);
            try
            {
                var canvas = driver.FindElement(By.XPath(@"//canvas[@class='nsi' and @width > 500]"));
                x = canvas.Size.Width / 2;
                y = canvas.Size.Height / 2;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}