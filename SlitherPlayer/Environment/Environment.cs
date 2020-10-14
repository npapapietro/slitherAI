using System;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Slither.Models;
using Slither.Runtime;
using Slither.ScreenCapture;

namespace Slither.Environment
{
    public static class EnvironmentUtils
    {
        private class Environment : IEnvironment
        {
            public float[] ScreenState { get; set; }

            public int Length { get; set; }

            public bool Dead { get; set; }

            public int TimeStamp { get; set; }

        }
        public static IEnvironment GetState(this IWebDriver driver, ScreenStream stream, bool withWait = false)
        {
            var TimeStamp = DateTime.UtcNow.ToEpoch();
            try
            {
                if (withWait)
                {
                    new WebDriverWait(driver, TimeSpan.FromSeconds(15))
                    .Until(driver => driver.FindSlitherLength().Count > 0);
                }
                var lengthText = driver.FindSlitherLength().FirstOrDefault().Text;
                var playbuttonDisplayed = driver.FindPlayButton().Displayed;
                Int32.TryParse(lengthText, out var length);

                var state = new Environment
                {
                    TimeStamp = TimeStamp,
                    Length = Convert.ToInt32(length),
                    Dead = Convert.ToInt32(length) <= 0 || playbuttonDisplayed,
                    ScreenState = stream.ClosestImage(TimeStamp)
                } as IEnvironment;

                return state;
            }
            catch (StaleElementReferenceException)
            {
                return new Environment
                {
                    TimeStamp = TimeStamp,
                    Length = 0,
                    Dead = true,
                    ScreenState = stream.ClosestImage(TimeStamp)
                } as IEnvironment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        public static ReadOnlyCollection<IWebElement> FindSlitherLength(this IWebDriver driver)
        {
            return driver.FindElements(By.XPath(@"//*[contains(.,'Your length')]/span[2]"));
        }

        public static IWebElement FindPlayButton(this IWebDriver driver)
        {
            return driver.FindElement(By.XPath(@"//*[contains(text(),'Play')]"));
        }

        public static int ToEpoch(this DateTime time)
        {
            var t = time - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }

        public static IEnvironment MockState()
        {
            Random randNum = new Random();
            var fakeImage = Enumerable
                .Repeat(0, 2048)
                .Select(i => ((Single)randNum.Next(0, 1000)) / 1000f)
                .ToArray();

            var testRequest = new Environment
            {
                Dead = false,
                TimeStamp = DateTime.UtcNow.ToEpoch(),
                Length = 5,
                ScreenState = fakeImage
            };

            return testRequest as IEnvironment;
        }

        public static (int, int) GetCenterCoordinates(this IWebDriver driver)
        {
            var canvas = driver.FindElement(By.XPath(@"//canvas[@class='nsi' and @width > 500]"));
            var width = canvas.Size.Width;
            var height = canvas.Size.Height;
            return (width / 2, height / 2);
        }

    }
}