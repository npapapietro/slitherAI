using OpenQA.Selenium;
using SlitherPlayer.ScreenCapture;

namespace SlitherPlayer.Environment
{
    public interface IEnvironment
    {
        IEnvironmentState GetState(IWebDriver driver, IScreenThread stream, bool withWath=false);

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