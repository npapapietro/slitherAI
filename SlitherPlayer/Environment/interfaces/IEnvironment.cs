using System;

namespace Slither.Environment
{
    public interface IEnvironment
    {
        float[] ScreenState { get; }

        int Length { get; }

        bool Dead { get; }

        /// <summary>
        /// Unix epoch format
        /// </summary>
        /// <value></value>
        int TimeStamp { get; }     
    }
}