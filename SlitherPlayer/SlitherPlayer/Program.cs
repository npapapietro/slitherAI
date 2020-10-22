using System;
using System.Runtime.InteropServices;

namespace SlitherPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Warning: Operating system doesn't expose screen shot easily, deferring to selenium's built in method.");
            }

            using var runtime = new Runtime();
            runtime.Run();
        }

        
    }
}
