using System;
using System.Collections.Generic;
using System.Text;

namespace SlitherPlayer.ScreenCapture
{
    public interface IScreenSample
    {
        int Epoch { get; set; }

        byte[] RawData {get; set;}
    }

    public interface IScreenThread: IDisposable
    {
        void Run(int threadCount = 1);

        float[] ClosestScreen(int epoch);
    }
}
