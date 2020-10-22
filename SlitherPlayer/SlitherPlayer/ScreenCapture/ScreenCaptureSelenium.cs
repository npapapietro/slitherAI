using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SlitherPlayer.ScreenCapture
{
    class ScreenCaptureSelenium: IScreenCaptureHandler
    {
        readonly IWebDriver driver;

        public ScreenCaptureSelenium(IWebDriver driver) => this.driver = driver;

        public bool GetScreen(out byte[] img)
        {
            try
            {
                var ss = (driver as ITakesScreenshot).GetScreenshot();
                img = ss.AsByteArray;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            img = new byte[]{};
            return false;
        }
    }
}
