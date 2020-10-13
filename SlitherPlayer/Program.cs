using OpenQA.Selenium.Chrome;
using Slither.Runtime;
using Slither.ScreenCapture;

namespace Slither
{
    class Program
    {
        static void Main(string[] args)
        {
    
            
            using var runtime = new SlitherPlayer();

            runtime.Run();

        }
    }

    public static class PlayerConfig
    {
        public static string Channel = "localhost:50051";
        public static string ModelFile = @"C:\Users\Nate-PC\Documents\git\Slither\ResNet50.onnx";
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

        public static bool TestScreen = true;

        public static bool TestKeys = false;
    }
}
