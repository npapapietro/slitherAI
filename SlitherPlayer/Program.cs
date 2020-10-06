using System;
using OpenCvSharp;
using OpenQA.Selenium.Chrome;
using Slither.Models;
using Slither.ScreenCapture;
using Grpc.Core;
using Slither.Client;
using Slither.Proto;
using Slither.Runtime;

namespace Slither
{
    class Program
    {
        static void Main(string[] args)
        {
            // GrpcOnly();
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage"); //!!!should be enabled for Jenkins
            options.AddArgument("--window-size=1920x1080"); //!!!should be enabled for Jenkins

            using var runtime = new SlitherPlayer();

            runtime.Run();

        }
    }

    public static class PlayerConfig
    {
        public static string Channel = "localhost:50051";
        public static string ModelFile = @"C:\Users\Nate-PC\Documents\git\Slither\ResNet50.onnx";
        public static string[] Options = {
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--window-size=1920x1080"
        };

        public static bool TestgRPC = true;
    }
}
