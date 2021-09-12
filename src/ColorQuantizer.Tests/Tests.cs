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

        private readonly SKBitmap _bitmap;
        private readonly SKColor[] _colors;

        public Tests()
        {
            using var stream = File.OpenRead(@"..\..\..\..\sample_data\splash\Aatrox_0.jpg");
            _bitmap = SKBitmap.Decode(stream);
            _colors = _bitmap.Pixels;
        }

        [Fact]
        public void Test1()
        {
            var a = initialQuantizer.Quantize(_colors, 128);
            var b = optimizedQuantizer.Quantize(_colors, 128);

            Assert.Equal(initialQuantizer.FindAllColorVariations(a, true), initialQuantizer.FindAllColorVariations(b, true));
        }
    }
}
