using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SlitherPlayer.ScreenCapture
{
    class ScreenCaptureWindows : IScreenCaptureHandler
    {
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


        public bool GetScreen(out byte[] img)
        {
            try
            {
                var slitherWindowHandle = Process.GetProcesses()
                    .Where(process => !string.IsNullOrEmpty(process.MainWindowTitle) && process.MainWindowTitle.ToLower().Contains("slither.io"))
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

                    using var image = new Bitmap(width, height);
                    using Graphics screen = Graphics.FromImage(image);

                    screen.CopyFromScreen(rect.left, rect.top, 0, 0, new System.Drawing.Size(width, height));
                    using var bm = new MemoryStream();
                    image.Save(bm, ImageFormat.Jpeg);
                    img = bm.ToArray();
                    return true;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            img = new byte[]{};
            return false;
        }
    }
}
