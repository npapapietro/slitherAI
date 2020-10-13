using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

namespace Slither.ScreenCapture
{
    public class ScreenStream : IDisposable
    {
        public volatile bool KillSignal;

        public volatile IList<ScreenSample> ScreenShots;

        private readonly IWebDriver driver;

        private IList<Thread> ImageThreads;

        public ScreenStream(IWebDriver driver)
        {
            this.driver = driver;
            KillSignal = false;
            ScreenShots = new List<ScreenSample>();
            ImageThreads = new List<Thread>();
        }

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

        private void CaptureImage()
        {
            int id = Thread.CurrentThread.ManagedThreadId;
            while(!KillSignal)
            {
                try
                {
                    using MemoryStream ms = new MemoryStream();
                    if(new ScreenSelection().Run(ms))
                    {   
                        ScreenShots.Add(new ScreenSample
                        {
                            TimeStamp = DateTime.UtcNow,
                            Screen = ms.ToArray()
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
        }

        public class ScreenSample
        {
            public DateTime TimeStamp { get; set; }

            public byte[] Screen { get; set; }
        }

    }


}