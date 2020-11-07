using System;

namespace SlitherPlayer.Environment
{
    public interface IEnvironmentState
    {
        /// <summary>
        /// Featurized image. Comes from the ScreenCapture functions
        /// that, depending on OS, grab the screen and featurize it 
        /// throught a CNN.
        /// </summary>
        float[] ScreenState { get; }


        /// <summary>
        /// Length is taken from the html element. If there's any issue with it
        /// then a default of 10 is returned, which is what is displayed at start
        /// and lowest length you can have. Sometimes at start there is small delay 
        /// in rendering it on screen.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// If the playbutton is visible.
        /// </summary>
        bool Dead { get; }

        /// <summary>
        /// Unix epoch format
        /// </summary>
        int TimeStamp { get; }   

        Guid Id {get;}
    }
}