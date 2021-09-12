using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorQuantizer.Initial;
using ColorQuantizer.Optimized;
using SkiaSharp;
using System.IO;

namespace ColorQuantizer.Benchmarks
{
    [MemoryDiagnoser]
    public class QuantizerBenchmarks
    {
        private readonly InitialColorQuantizer initialColorQuantizer = new();
        private readonly OptimizedColorQuantizer optimizedColorQuantizer = new();
        private readonly SKBitmap _bitmap;
        private readonly SKColor[] _colors;

        public QuantizerBenchmarks()
        {
            using var stream = File.OpenRead(@"..\..\..\..\sample_data\splash\Aatrox_0.jpg");
            _bitmap = SKBitmap.Decode(stream);
            _colors = _bitmap.Pixels;
        }

        [Benchmark]
        public ColorSwatch Old128()
        {
            SKColor[] skClrs = initialColorQuantizer.Quantize(_colors, 128);
            return initialColorQuantizer.FindAllColorVariations(skClrs, true);
        }

        [Benchmark]
        public ColorSwatch New128()
        {
            SKColor[] skClrs = optimizedColorQuantizer.Quantize(_colors, 128);
            return optimizedColorQuantizer.FindAllColorVariations(skClrs, true);
        }
    }
}
