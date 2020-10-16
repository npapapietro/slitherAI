using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Slither.Models;
using Slither.Runtime;
using Slither.ScreenCapture;
using Slither.Utils;

namespace Slither.Environment
{
    public class Environment : IEnvironment
    {
        private class EnvironmentState : IEnvironmentState
        {
            public float[] ScreenState { get; set; }

            public int Length { get; set; }

            public bool Dead { get; set; }

            public int TimeStamp { get; set; }

        }
        public IEnvironmentState GetState(IWebDriver driver, ScreenStream stream, bool withWait = false)
        {

            Thread.Sleep(withWait ? 1000 : 0);

            if (!GetSlitherLength(driver, out var length))
            {
                length = 0;
            }

            var playbuttonDisplayed = GetPlayButton(driver, out var playbutton) && (playbutton?.Displayed ?? false);

            var TimeStamp = DateTime.UtcNow.ToEpoch();

            return new EnvironmentState
            {
                TimeStamp = TimeStamp,
                Length = length,
                Dead = length <= 0 || playbuttonDisplayed,
                ScreenState = stream.ClosestImage(TimeStamp)
            } as IEnvironmentState;

        }

        public bool GetSlitherLength(IWebDriver driver, out int length)
        {
            length = -1;
            try
            {
                var lengthElment = driver.FindElement(By.XPath(@"//*[contains(.,'Your length')]/span[2]"));
                return Int32.TryParse(lengthElment.Text, out length);
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
            return this.GetPlayButton(driver, out _);
        }

        public bool GetCenter(IWebDriver driver, out int x, out int y)
        {
            (x,y) = (0, 0);
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