using Microsoft.ML.OnnxRuntime.Tensors;

namespace Slither.Models
{
    public static class FeatureUtils
    {
        public static Tensor<T> Flatten<T>(this Tensor<T> t)
        {
            int total = 1;

            foreach(var d in t.Dimensions)
            {
                total *= d;
            }

            return t.Reshape(new int[]{total});
        }
    }
}