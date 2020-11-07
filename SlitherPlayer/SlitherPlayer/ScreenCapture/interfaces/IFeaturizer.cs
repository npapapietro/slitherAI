using OpenQA.Selenium;
using System;
using System.IO;

namespace SlitherPlayer.ScreenCapture
{
    public interface IFeaturizer : IDisposable
    {
        /// <summary>
        /// Featurizes the web browser image using headless tf.keras.application models.
        /// </summary>
        /// <param name="stream">Stream of image in bytes</param>
        /// <returns>Array of image features with saved image for logging purposes</returns>
        float[] GetImageFeatures(byte[] stream, Guid id);

    }
}
