using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using Slither.Environment;
using Slither.Models;
using Slither.Utils;

namespace Slither.ScreenCapture
{
    public class ScreenStream : IDisposable
    {
        private volatile bool KillSignal;

        /// <summary>
        /// List of records of screen shots, max 50
        /// </summary>
        public volatile IList<ScreenSample> ScreenShots;

        private readonly IWebDriver driver;

        private IList<Thread> ImageThreads;

        private readonly IFeaturizer Model;

        public ScreenStream(IWebDriver driver)
        {
            this.driver = driver;
            KillSignal = false;
            ScreenShots = new List<ScreenSample>();
            ImageThreads = new List<Thread>();
            Model = new Featurizer(PlayerConfig.ModelFile);
        }

        /// <summary>
        /// Spawns threads that continuously screenshot the web browser
        /// </summary>
        /// <param name="threadCount">Number of threads to spawn</param>
        public void Run(int threadCount = 1)
        {
            for(var i = 0; i < threadCount; i++){
                var thread = new Thread(new ThreadStart(CaptureImage)); 
                ImageThreads.Add(thread);
            }

            foreach(var t in ImageThreads){
                t.Start();
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// This function takes a screenshot and appends the featurized result
        /// to `ScreenShots`
        /// </summary>
        private void CaptureImage()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            while(!KillSignal)
            {
                try
                {
                    if(Model.GetImageFeatures(driver, out var img))
                    {
                        ScreenShots.Add(new ScreenSample{
                            Screen = img,
                        });
                    }                   

                    if(ScreenShots.Count > 50)
                    {
                        ScreenShots.RemoveAt(0);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Image capturing thread error;");
                    Console.WriteLine(e.Message);
                    return;
                }
            }

            Console.WriteLine($"Aborting image thread {id}");
        }

        public void Dispose()
        {
            KillSignal = true;
            Model.Dispose();
        }

        /// <summary>
        /// Record to hold screenshot (featurized) and timestamp taken at.
        /// </summary>
        public class ScreenSample
        {
            public int Epoch { get; set; } = DateTime.UtcNow.ToEpoch();

            public float[] Screen { get; set; }
        }

        /// <summary>
        /// Returns the featurized image closest in time.
        /// </summary>
        /// <param name="epoch">Timestamp in epoch units</param>
        /// <returns>Featurized Image (2048,)</returns>
        public float[] ClosestImage(int epoch)
        {
            return ScreenShots
                .Select( x => new {x.Screen, epoch = Math.Abs(epoch - x.Epoch)})
                .Aggregate((x, y) => x.epoch < y.epoch ? x : y).Screen;
        }

    }


}