using System;
using System.IO;
using OpenQA.Selenium;

namespace Slither.Models
{
    public interface IFeaturizer : IDisposable
    {
        /// <summary>
        /// Featurizes the web browser image using headless tf.keras.application models.
        /// </summary>
        /// <param name="driver">Web driver, needed for non Windows implementations</param>
        /// <param name="image">Out array of image features</param>
        /// <returns>Flag if featurization was sucessful</returns>
        bool GetImageFeatures(IWebDriver driver, out float[] image);
    }
}