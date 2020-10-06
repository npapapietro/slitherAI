using System;
using System.IO;

namespace Slither.Models
{
    public interface IFeaturizer: IDisposable
    {
        float[] GetImage(MemoryStream imgStream);
    }
}