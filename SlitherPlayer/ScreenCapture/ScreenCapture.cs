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

        [StructLayout(LayoutKind.Sequential)]
        public struct Rectangle
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public static Bitmap SeleniumScreenshot(this IWebDriver driver)
        {
            try
            {
                var ss = (driver as ITakesScreenshot).GetScreenshot();
                var ms = new MemoryStream(ss.AsByteArray);
                return new Bitmap(ms);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rectangle);

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

        public static bool GetScreenOSX(out Bitmap img)
        {
            try
            {
                IntPtr windowinfo = CGWindowListCopyWindowInfo(0, 0);
                Console.WriteLine(windowinfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            img = new Bitmap(0,0);
            return false;
        }

        public static bool GetScreen(out Bitmap img, IWebDriver driver=null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) )
            {
                return GetScreenWindows(out img);
            }
            else
            {
                if (driver is null)
                {
                    throw new NullReferenceException("driver must not be null");
                }
                img = driver.SeleniumScreenshot();
            }
        }
    }
}