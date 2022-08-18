using ColorQuantizer.Initial;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColorQuantizer.Optimized3;
using ColorQuantizer.Opzimized2;
using Xunit;

namespace ColorQuantizer.Tests
{
    public class Tests
    {
        private readonly InitialColorQuantizer initialQuantizer = new();
        private readonly OptimizedColorQuantizer3 optimizedQuantizer = new();
        
        private static IEnumerable<string> GetTestImages() => Directory.EnumerateFiles(@"..\..\..\..\sample_data", "*.jpg", SearchOption.AllDirectories);

        [Fact]
        public void Both_impls_same_quantization()
        {
            foreach (string item in GetTestImages())
            {
                using FileStream stream = File.OpenRead(item);
                using SKBitmap bitmap = SKBitmap.Decode(stream);
                SKColor[] colors = bitmap.Pixels;

                SKColor[] a = initialQuantizer.Quantize(colors, 128);
                SKColor[] b = optimizedQuantizer.Quantize(colors, 128);

                Assert.Equal(a, b);
            }
        }

        [Fact]
        public void Both_impls_same_swatch()
        {
            foreach (string item in GetTestImages())
            {
                using FileStream stream = File.OpenRead(item);
                using SKBitmap bitmap = SKBitmap.Decode(stream);
                SKColor[] colors = bitmap.Pixels;

                //quantize with one, check that both find the same final swatch.
                SKColor[] quantized = initialQuantizer.Quantize(colors, 128);

                Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true), optimizedQuantizer.FindAllColorVariations(quantized, true));
            }
        }
    }
}
