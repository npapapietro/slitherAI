using System;
using System.IO;
using OpenQA.Selenium;

namespace Slither.Models
{
    public interface IFeaturizer: IDisposable
    {
        float[] GetImageFeatures(MemoryStream imgStream);
        bool GetImageFeatures(IWebDriver driver, out float[] image);
    }
}