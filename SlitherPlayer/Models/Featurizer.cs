using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using OpenQA.Selenium;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Slither.Models
{
    public class Featurizer : IFeaturizer, IDisposable
    {
        private readonly InferenceSession Session;

        public Featurizer(string ModelFile)
        {
            Session = new InferenceSession(ModelFile);
        }

        private Tensor<float> PreProcess(MemoryStream stream)
        {
            using Image<Rgb24> image = Image<Rgba32>.Load(stream, out var format).CloneAs<Rgb24>();
            using Stream imageStream = new MemoryStream();
            var paddedHeight = 299;
            var paddedWidth = 299;

            image.Mutate(x => x.Resize(paddedHeight, paddedWidth));
            image.Save(imageStream, format);

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
        public bool GetImageFeatures(IWebDriver driver, out float[] image)
        {
            if (ScreenCapture.ScreenCapture.GetScreen(out var bitmap, driver))
            {
                using var imgStream = new MemoryStream();
                bitmap.Save(imgStream, ImageFormat.Png);
                var input = PreProcess(imgStream);

                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("input_1", input) // (3, 299, 299)
                };

                using var results = Session.Run(inputs); // (2048,)
                image = results.First().AsTensor<float>().ToArray();   
                return true; 
            }
            
            image = new float[]{};
            return false;
        }

        public void Dispose()
        {
            Session.Dispose();
        }
    }
}