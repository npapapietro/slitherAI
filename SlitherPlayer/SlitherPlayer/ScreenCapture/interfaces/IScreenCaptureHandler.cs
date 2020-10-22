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
        /// Screen shots the web browser launched by selenium and crops it a bit
        /// </summary>
        /// <param name="img">Out variable of screen shot</param>
        /// <returns>If screen shot was successful</returns>
        bool GetScreen(out byte[] img);
    }
}
