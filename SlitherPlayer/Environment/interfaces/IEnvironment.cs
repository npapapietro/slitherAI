using System;
using OpenQA.Selenium;
using Slither.ScreenCapture;

namespace Slither.Environment
{
    public interface IEnvironmentState
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

    public interface IEnvironment
    {
        IEnvironmentState GetState(IWebDriver driver, ScreenStream stream, bool withWath=false);

        bool GetSlitherLength(IWebDriver driver, out int length);

        /// <summary>
        /// This tries to grab the IWebElement playbutton returning a success flag.
        /// Any errors are returned as false and null out var.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <param name="playButton">Playbutton if successful, null if not</param>
        /// <returns>Flag of success</returns>
        bool GetPlayButton(IWebDriver driver, out IWebElement playButton);

        /// <summary>
        /// Overloaded variant of <see cref="GetPlayButton(IWebDriver, out IWebElement)"/> that
        /// just checks for success.
        /// </summary>
        /// <param name="driver">Web driver</param>
        /// <returns>Flag of success</returns>
        bool GetPlayButton(IWebDriver driver);

        bool GetCenter(IWebDriver driver, out int x, out int y);
    }
}