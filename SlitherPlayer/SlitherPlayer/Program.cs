using System;
using System.IO;
using System.Runtime.InteropServices;
using SlitherPlayer.Logger;

namespace SlitherPlayer
{

    #region Configurations
    /// <summary>
    /// Inline the config for now, move to a toml shared by python and c# later
    /// </summary>
    public static class Configurations
    {
        public static string Channel = "localhost:50051";
        private static string modelFile = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "data", "ResNet50.onnx");

        public static string LogPath = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "data", "logs");
        public static string[] Options = {
                "--window-size=1000x1000",
                // "--headless"
            };
        public static int StepLimit = 5000000;
        public static int RunLimit = int.MaxValue;
        public static bool TestSelect = false;
        public static bool Verbose = true;

        public static bool UseSeleniumScreenShot = false;

        public static string ModelFile { get => modelFile; set => modelFile = value; }
    }
    #endregion
    
    class Program
    {
        static void Main(string[] args)
        {
            PlayerLogger.WriteLine = Configurations.Verbose;
            PlayerLogger.LogPath = Configurations.LogPath;
            PlayerLogger.VerboseLevel = LogLevel.HandledError;
            PlayerLogger.SetFolders();

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                PlayerLogger.Warn("Operating system doesn't expose screen shot easily, deferring to selenium's built in method.");
            }
            
            PlayerLogger.Info("Starting...");
            while (true)
            {
                try
                {
                    using var runtime = new Runtime();
                    runtime.Run();
                }
                catch(Exception e)
                {
                    PlayerLogger.Error("Restarting whole game" + e.Message);
                }   
            }
        }

    }
}