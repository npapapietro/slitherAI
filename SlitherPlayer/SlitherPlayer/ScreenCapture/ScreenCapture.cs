using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SlitherPlayer.ScreenCapture
{
    public class ScreenCapture : IScreenThread
    {
        private class ScreenSample : IScreenSample
        {
            public int Epoch { get; set; }

            public byte[] RawData { get; set; }

            public Guid Id {get; set;}
        }

        private bool KillSignal;

        readonly IScreenCaptureHandler handler;
        readonly IFeaturizer featurizer;
        readonly IList<IScreenSample> ScreenShots;
        readonly IList<Thread> ImageThreads;

        public ScreenCapture(IFeaturizer featurizer, IScreenCaptureHandler handler)
        {
            this.featurizer = featurizer;
            this.handler = handler;
            ScreenShots = new List<IScreenSample>();
            ImageThreads = new List<Thread>();
            KillSignal = false;
        }

        public ScreenCapture(IWebDriver driver)
        {
            featurizer = new Featurizer(Configurations.ModelFile);
            ImageThreads = new List<Thread>();
            ScreenShots = new List<IScreenSample>();
            KillSignal = false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Configurations.UseSeleniumScreenShot)
            {
                handler = new ScreenCaptureWindows();
            }
            else
            {
                handler = new ScreenCaptureSelenium(driver);
            }
        }

        public void Run(int threadCount = 3)
        {
            for (var i = 0; i < threadCount; i++)
            {
                var thread = new Thread(new ThreadStart(CaptureImage));
                thread.Start();
                ImageThreads.Add(thread);
            }
        }

        public (float[], Guid)  ClosestScreen(int epoch)
        {
            if (ScreenShots.Count == 0)
            {
                return (null, new Guid());
            }            
            
            lock(this.ScreenShots){
                var closest = ScreenShots
                .Select(x => new { x.RawData, diff = Math.Abs(epoch - x.Epoch), x.Epoch , x.Id})
                .Aggregate(
                    (x, y) => x.diff < y.diff ? x : y);

                return (featurizer.GetImageFeatures(closest.RawData, closest.Id), closest.Id);
            }
            
            
        }

        public void Dispose()
        {
            KillSignal = true;
            featurizer.Dispose();
        }

        private void CaptureImage()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            while (!KillSignal)
            {
                try
                {
                    // var stopWatch = System.Diagnostics.Stopwatch.StartNew();
                    if (handler.GetScreen(out var bytes))
                    {
                        lock (this.ScreenShots)
                        {
                            ScreenShots.Add(new ScreenSample
                            {
                                RawData = bytes,
                                Id = Guid.NewGuid(),
                            });
                        }



                    }
                    // Console.WriteLine($"Screenshot time {stopWatch.ElapsedMilliseconds}");

                    if (ScreenShots.Count > 250)
                    {
                        ScreenShots.RemoveAt(0);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Image capturing thread error;");
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            Console.WriteLine($"Aborting image thread {id}");

        }
    }
}
