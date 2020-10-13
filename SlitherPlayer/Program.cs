﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenQA.Selenium.Chrome;
using Slither.Runtime;
using Slither.ScreenCapture;

namespace Slither
{
    class Program
    {
        static void Main(string[] args)
        {
            int x = 3;
            x.GetScreenOSX(out var y);
        }
    }

    public static class PlayerConfig
    {
        internal static readonly bool TestScreen =false;
        internal static readonly bool TestKeys = false;
        public static string Channel = "localhost:50051";
        public static string ModelFile = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 
            @"/Users/nathan/Git/slitherAI/SlitherPlayer/ResNet50.onnx" : @"C:\Users\Nate-PC\Documents\git\Slither\ResNet50.onnx";
        public static string[] Options = {
            // "--no-sandbox",
            // "--disable-dev-shm-usage",
            "--window-size=1000x1000",
            // "--disable-logging",
            // "--headless"
        };

        public static bool TestgRPC = false;

        public static int StepLimit = 5000000;

        public static int RunLimit = int.MaxValue;
        public static bool TestSelect = false;
    }
}
