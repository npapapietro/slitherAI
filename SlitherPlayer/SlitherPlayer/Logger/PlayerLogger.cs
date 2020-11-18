using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SixLabors.ImageSharp;

namespace SlitherPlayer.Logger
{
    public enum LogLevel 
    {
        Debug,
        Info,
        Warn,
        HandledError,
        Error
    }

    public static class PlayerLogger
    {
        /// <summary>
        /// Runs Console.Writeline on every log
        /// </summary>
        public static bool WriteLine { get; set; } = false;

        /// <summary>
        /// Minimum level for verbose logging (ie print line)
        /// </summary>
        public static LogLevel VerboseLevel {get; set;} = LogLevel.Debug;

        public static string LogPath { get; set; }

        static string LogFile => Path.Join(LogPath, "Player.log");
        static string ImageFolder => Path.Join(LogPath, "Images");
        public static void SetFolders()
        {
            if (!File.Exists(LogFile))
            {
                File.Create(LogFile);
            }

            if (!Directory.Exists(ImageFolder))
            {
                Directory.CreateDirectory(ImageFolder);
            }
        }

        public static void Debug<T>(T msg) => Log(msg, LogLevel.Debug);
        public static void Info<T>(T msg) => Log(msg, LogLevel.Info);
        public static void Warn<T>(T msg) => Log(msg, LogLevel.Warn);
        public static void HandledError<T>(T msg) => Log(msg, LogLevel.HandledError);
        public static void Error<T>(T msg) => Log(msg, LogLevel.Error);

        public static void Log<T>(T message, LogLevel level)
        {
            var logmssage = new {
                LevelName = level,
                UTCNow = DateTime.UtcNow.ToEpoch(),
                message = message
            };
            var msg = JsonConvert.SerializeObject(logmssage);
            using StreamWriter streamWriter = new StreamWriter(LogFile, true);            

            if(WriteLine && level > VerboseLevel) Console.WriteLine(msg);

            streamWriter.WriteLine(msg);
            streamWriter.Close(); 
        }


        public static void LogImage(Guid Id, Image image)
        {
            Task.Factory.StartNew(() =>
            {
                image.SaveAsPng(Path.Join(ImageFolder, Id.ToString() + ".png"));
            });
        }
    }
}