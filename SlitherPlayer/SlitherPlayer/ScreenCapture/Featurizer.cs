using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenQA.Selenium;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SlitherPlayer.ScreenCapture
{
    public class Featurizer: IFeaturizer
    {
        private readonly InferenceSession session;

        public Featurizer(string OnnxModelFile)
        {
            session = new InferenceSession(OnnxModelFile);
        }

        public void Dispose()
        {
            session.Dispose();
        }

        public float[] GetImageFeatures(byte[] stream)
        {
            var input = PreProcess(stream);

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_1", input) // (3, 299, 299)
            };

            using var results = session.Run(inputs); // (2048,)
            return results.First().AsTensor<float>().ToArray();
        }

        private Tensor<float> PreProcess(ReadOnlySpan<byte> stream)
        {
            using Image<Rgb24> image = Image.Load(stream, out var format).CloneAs<Rgb24>();
            var paddedHeight = 299;
            var paddedWidth = 299;

            image.Mutate(x => x.Resize(paddedHeight, paddedWidth));

            Tensor<float> input = new DenseTensor<float>(new[] { 1, 3, paddedHeight, paddedWidth });

            var mean = new[] { 102.9801f, 115.9465f, 122.7717f };
            for (int y = paddedHeight - image.Height; y < image.Height; y++)
            {
                Span<Rgb24> pixelSpan = image.GetPixelRowSpan(y);
                for (int x = paddedWidth - image.Width; x < image.Width; x++)
                {
                    input[0, 0, y, x] = pixelSpan[x].B - mean[0];
                    input[0, 1, y, x] = pixelSpan[x].G - mean[1];
                    input[0, 2, y, x] = pixelSpan[x].R - mean[2];
                }
            }
            return input;
        }

    }
}
