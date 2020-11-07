using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SlitherPlayer.Environment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace SlitherPlayer.Tests
{
    public class SeleniumTests
    {
        private readonly ITestOutputHelper output;

        public SeleniumTests(ITestOutputHelper o) => output = o;

        [Fact]
        public void TestGrabHtml()
        {
            var driver = new ChromeDriver();
            var env = new SlitherEnvironment();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            driver.Navigate().GoToUrl("http://slither.io");

            wait.Until(env.GetPlayButton);
            IWebElement playButton;
            while (!env.GetPlayButton(driver, out playButton)) { }
            playButton.Click();

            Thread.Sleep(1000);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var elms = driver.FindElements(By.XPath(@"//canvas"))
                .Select(x => x.GetAttribute("outerHTML"))
                .ToList();
        
            var stream = (driver as ITakesScreenshot).GetScreenshot().AsByteArray;

            using Image<Rgb24> image = Image.Load(stream, out var format).CloneAs<Rgb24>();

            image.Mutate(x => x.Resize(299, 299));

            var p = Path.Join(Directory.GetCurrentDirectory(), "testImage.png");
            output.WriteLine(p);
            //image.Save(p);

            var el = stopwatch.ElapsedMilliseconds;

            output.WriteLine($"{el} elapsed ms");
        }
    }
}
