using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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

        public static Bitmap CurrentScreen(this IWebDriver driver)
        {
            try
            {
                var ss = (driver as ITakesScreenshot).GetScreenshot();
                return new Bitmap(new MemoryStream(ss.AsByteArray));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static Mat AsCNNArray(this Bitmap mat)
        {
            var ary = mat.ToMat().Resize(new OpenCvSharp.Size(299, 299));
            ary /= 127.5;
            ary -= 1.0;
            return ary;
        }


        public static byte[] CurrentScreenBytes(this IWebDriver driver)
        {
            try
            {
                return (driver as ITakesScreenshot).GetScreenshot().AsByteArray;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static MemoryStream StreamScreen(this IWebDriver driver)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var ss = (driver as ITakesScreenshot).GetScreenshot();

                stopwatch.Stop();
                Console.WriteLine($"Time to take screen {stopwatch.ElapsedMilliseconds}");
                var stream = new MemoryStream(ss.AsByteArray);
                return stream;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rectangle rectangle);

        public static bool GetScreen(out Bitmap img)
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
    }
}