using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
// using ObjCRuntime;

namespace Slither.ScreenCapture
{
    /// <summary>
    /// Utility class for processing Screencaps and Images
    /// </summary>
    public static class ScreenCapture
    {
        /// <summary>
        /// Uses the selenium web driver to screen shot, rather slow on most systems over native screen shot calls.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <param name="img">Out image</param>
        /// <returns>Flag if screen shot was successful</returns>
        public static bool SeleniumScreenshot(this IWebDriver driver, out Bitmap img)
        {
            try
            {
                var ss = (driver as ITakesScreenshot).GetScreenshot();
                var ms = new MemoryStream(ss.AsByteArray);
                img = new Bitmap(ms);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            img = new Bitmap(0,0);
            return false;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Rectangle
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rectangle);

        /// <summary>
        /// Screen shots the web browser launched by selenium and crops it a bit
        /// </summary>
        /// <param name="img">Out variable of screen shot</param>
        /// <returns>If screen shot was successful</returns>
        public static bool GetScreenWindows(out Bitmap img)
        {
            try
            {
                var slitherWindowHandle = Process.GetProcesses()
                    .Where(process => !String.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle.ToLower().Contains("slither.io"))
                    .Select(a => a.MainWindowHandle)
                    .First();
                if (GetWindowRect(slitherWindowHandle, out var rect))                
                {
                    // Getting image and cropping the edges and explorer bar
                    rect.left += 20;
                    rect.right -= 20;
                    rect.top += 130;
                    rect.bottom -= 20;
                    int width = rect.right - rect.left;
                    int height = rect.bottom - rect.top;

                    img = new Bitmap(width, height);
                    using Graphics screen = Graphics.FromImage(img);

                    screen.CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(width, height));
                    return true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            img = new Bitmap(0,0);
            return false;
        }

        /// <summary>
        /// Captures the web browser screen, depending on which operating system, will call different implementations of it
        /// </summary>
        /// <param name="img">Out variable of the image</param>
        /// <param name="driver">Web driver is needed for OSX and Linux until native calls can be made for screen shotting.</param>
        /// <returns></returns>
        public static bool GetScreen(out Bitmap img, IWebDriver driver=null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetScreenWindows(out img);
            }

            if (driver is null)
            {
                throw new NullReferenceException("driver must not be null");
            }
            return driver.SeleniumScreenshot(out img);
        
        }
    }
}