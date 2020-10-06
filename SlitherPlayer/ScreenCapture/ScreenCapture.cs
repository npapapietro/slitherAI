using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.IO;

namespace Slither.ScreenCapture
{
    /// <summary>
    /// Utility class for processing Screencaps and Images
    /// </summary>
    public static class ScreenCapture
    {
        public static Bitmap CurrentScreen(this IWebDriver driver)
        {
            try
            {
                var ss = (driver as ITakesScreenshot).GetScreenshot();
                return new Bitmap(new MemoryStream(ss.AsByteArray));
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static Mat AsCNNArray(this Bitmap mat)
        {
            var ary = mat.ToMat().Resize(new OpenCvSharp.Size(299,299));
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
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static MemoryStream StreamScreen(this IWebDriver driver)
        {
            try
            {
                var ss = (driver as ITakesScreenshot).GetScreenshot();
                return new MemoryStream(ss.AsByteArray);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}