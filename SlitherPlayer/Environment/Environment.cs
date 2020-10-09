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
        public static IEnvironment GetState(this IWebDriver driver, IFeaturizer model)
        {
            try
            {
                new WebDriverWait(driver, TimeSpan.FromSeconds(15))
                    .Until(driver => driver.FindSlitherLength().Count > 0);

                var lengthText = driver.FindSlitherLength().FirstOrDefault().Text;
                var playbuttonDisplayed = driver.FindPlayButton().Displayed;

                Int32.TryParse(lengthText, out var length);

                return new Environment
                {
                    TimeStamp = DateTime.UtcNow.ToEpoch(),
                    Length = Convert.ToInt32(length),
                    Dead = Convert.ToInt32(length) <= 0 || playbuttonDisplayed,
                    ScreenState = model.GetImage(driver.StreamScreen())
                } as IEnvironment;
            }
            catch(StaleElementReferenceException)
            {
                return new Environment
                {
                    TimeStamp = DateTime.UtcNow.ToEpoch(),
                    Length = 0,
                    Dead = true,
                    ScreenState = model.GetImage(driver.StreamScreen())
                } as IEnvironment;
            }
            catch(Exception e)
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