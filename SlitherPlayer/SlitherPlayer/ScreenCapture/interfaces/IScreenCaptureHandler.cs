using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SlitherPlayer.ScreenCapture
{
    public interface IScreenCaptureHandler
    {
        /// <summary>
        /// Gets the screen of the browser
        /// </summary>
        /// <param name="img">Out variable of screen shot</param>
        /// <returns>If screen shot was successful</returns>
        bool GetScreen(out byte[] img);
    }
}
