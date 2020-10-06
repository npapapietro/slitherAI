using System;
using System.Linq;
using OpenQA.Selenium;
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
        public static IEnvironment GetState(this IWebDriver driver, IFeaturizer model)
        {
            var length = driver.FindSlitherLength();
            var playbuttonDisplayed = driver.FindPlayButton().Displayed;

            var env = new Environment
            {
                TimeStamp = DateTime.UtcNow.ToEpoch(),
                Length = length,
                Dead = length < 0 || playbuttonDisplayed,
                ScreenState = model.GetImage(driver.StreamScreen())
            };

            return env as IEnvironment;
        }

        public static int FindSlitherLength(this IWebDriver driver)
        {
            var lengthQry = By.XPath(@"//span[contains(.,'Your length')]/span[2]/text()");
            try
            {
                return Convert.ToInt32(driver.FindElement(lengthQry).Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting length " + e.Message);
                return -1;
            }
        }

        public static IWebElement FindPlayButton(this IWebDriver driver)
        {
            var buttonQry = By.XPath(@"//*[contains(text(),'Play')]");
            try
            {
                return driver.FindElement(buttonQry);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error getting playbutton " + e.Message);
                return null;
            }
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
                .Select(i => ((Single)randNum.Next(0,1000)) / 1000f)
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

    }
}