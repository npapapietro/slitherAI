using System;
using System.IO;

namespace Slither.Models
{
    public interface IFeaturizer: IDisposable
    {
        float[] GetImageFeatures(MemoryStream imgStream);
        float[] GetImageFeatures();
    }
}