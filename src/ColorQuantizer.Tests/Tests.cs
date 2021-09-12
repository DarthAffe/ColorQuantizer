using ColorQuantizer.Initial;
using ColorQuantizer.Optimized;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace ColorQuantizer.Tests
{
    public class Tests
    {
        private readonly InitialColorQuantizer initialQuantizer = new();
        private readonly OptimizedColorQuantizer optimizedQuantizer = new();

        private static IEnumerable<string> GetTestImages() => Directory.EnumerateFiles(@"..\..\..\..\sample_data", "*.jpg", SearchOption.AllDirectories);

        [Fact]
        public void Test1()
        {
            foreach (var item in GetTestImages())
            {
                using var stream = File.OpenRead(item);
                using var bitmap = SKBitmap.Decode(stream);
                var colors = bitmap.Pixels;

                var a = initialQuantizer.Quantize(colors, 128);
                var b = optimizedQuantizer.Quantize(colors, 128);

                Assert.Equal(initialQuantizer.FindAllColorVariations(a, true), initialQuantizer.FindAllColorVariations(b, true));
            }
        }
    }
}
