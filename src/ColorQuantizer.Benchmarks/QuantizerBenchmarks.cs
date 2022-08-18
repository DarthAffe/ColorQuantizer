using System;
using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using ColorQuantizer.Initial;
using SkiaSharp;
using System.IO;
using System.Linq;
using ColorQuantizer.Optimized3;
using ColorQuantizer.Opzimized2;

namespace ColorQuantizer.Benchmarks
{
    [MemoryDiagnoser]
    [HtmlExporter]
    public class QuantizerBenchmarks
    {
        private readonly InitialColorQuantizer initialColorQuantizer = new();
        private readonly OptimizedColorQuantizer2 optimizedColorQuantizer = new();
        private readonly OptimizedColorQuantizer3 optimizedColorQuantizer3 = new();
        private readonly List<SKBitmap> _bitmap;
        private readonly List<SKColor[]> _colors;

        private readonly SKColor[][] _rangeTestColor;
        
        [Params(128)]
        public int Size { get; set; }

        public QuantizerBenchmarks()
        {
            _bitmap = new List<SKBitmap>();
            _colors = new List<SKColor[]>();

            IEnumerable<string> files = Directory.EnumerateFiles(@"..\..\..\..\..\..\..\..\sample_data", "*.jpg", SearchOption.AllDirectories);
            //using FileStream stream = File.OpenRead(@"..\..\..\..\..\..\..\..\sample_data\splash\Aatrox_0.jpg");
            foreach (string file in files)
            {
                using FileStream stream = File.OpenRead(file);
                SKBitmap bitmap = SKBitmap.Decode(stream);
                SKColor[] colors = bitmap.Pixels;

                _bitmap.Add(bitmap);
                _colors.Add(colors);
            }

            //_rangeTestColor = new SKColor[7][];
            //_rangeTestColor[0] = _colors[0].Take(500).ToArray();
            //_rangeTestColor[1] = _colors[0].Take(600).ToArray();
            //_rangeTestColor[2] = _colors[0].Take(700).ToArray();
            //_rangeTestColor[3] = _colors[0].Take(800).ToArray();
            //_rangeTestColor[4] = _colors[0].Take(900).ToArray();
            //_rangeTestColor[5] = _colors[0].Take(1000).ToArray();
            //_rangeTestColor[6] = _colors[0].Take(1100).ToArray();
        }

        //[Params(0, 1, 2, 3, 4, 5, 6)]
        //public int Amount { get; set; }

        //[Benchmark]
        //public ColorRanges RangeBenchmark1()
        //{
        //    Span<SKColor> colors = _rangeTestColor[Amount];

        //    Span<bool> redBuckets = stackalloc bool[256];
        //    Span<bool> greenBuckets = stackalloc bool[256];
        //    Span<bool> blueBuckets = stackalloc bool[256];

        //    for (int i = 0; i < colors.Length; i++)
        //    {
        //        SKColor color = colors[i];
        //        redBuckets[color.Red] = true;
        //        greenBuckets[color.Green] = true;
        //        blueBuckets[color.Blue] = true;
        //    }

        //    byte redMin = 0;
        //    byte redMax = 0;
        //    byte greenMin = 0;
        //    byte greenMax = 0;
        //    byte blueMin = 0;
        //    byte blueMax = 0;

        //    for (byte i = 0; i < redBuckets.Length; i++)
        //        if (redBuckets[i])
        //        {
        //            redMin = i;
        //            break;
        //        }

        //    for (int i = redBuckets.Length - 1; i >= 0; i--)
        //        if (redBuckets[i])
        //        {
        //            redMax = (byte)i;
        //            break;
        //        }

        //    for (byte i = 0; i < greenBuckets.Length; i++)
        //        if (greenBuckets[i])
        //        {
        //            greenMin = i;
        //            break;
        //        }

        //    for (int i = greenBuckets.Length - 1; i >= 0; i--)
        //        if (greenBuckets[i])
        //        {
        //            greenMax = (byte)i;
        //            break;
        //        }

        //    for (byte i = 0; i < blueBuckets.Length; i++)
        //        if (blueBuckets[i])
        //        {
        //            blueMin = i;
        //            break;
        //        }

        //    for (int i = blueBuckets.Length - 1; i >= 0; i--)
        //        if (blueBuckets[i])
        //        {
        //            blueMax = (byte)i;
        //            break;
        //        }

        //    return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
        //}

        //[Benchmark]
        //public ColorRanges RangeBenchmark2()
        //{
        //    Span<SKColor> colors = _rangeTestColor[Amount];

        //    byte redMin = byte.MaxValue;
        //    byte redMax = byte.MinValue;
        //    byte greenMin = byte.MaxValue;
        //    byte greenMax = byte.MinValue;
        //    byte blueMin = byte.MaxValue;
        //    byte blueMax = byte.MinValue;

        //    for (int i = 0; i < colors.Length; i++)
        //    {
        //        SKColor color = colors[i];
        //        if (color.Red < redMin) redMin = color.Red;
        //        if (color.Red > redMax) redMax = color.Red;
        //        if (color.Green < greenMin) greenMin = color.Green;
        //        if (color.Green > greenMax) greenMax = color.Green;
        //        if (color.Blue < blueMin) blueMin = color.Blue;
        //        if (color.Blue > blueMax) blueMax = color.Blue;
        //    }

        //    return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
        //}

        [Benchmark]
        public ColorSwatch[] Old128()
        {
            ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
            int i = 0;
            foreach (SKColor[] colors in _colors)
            {
                SKColor[] skClrs = initialColorQuantizer.Quantize(colors, 128);
                swatches[i++] = initialColorQuantizer.FindAllColorVariations(skClrs, true);
            }

            return swatches;
        }

        [Benchmark]
        public ColorSwatch[] New()
        {
            ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
            int i = 0;
            foreach (SKColor[] colors in _colors)
            {
                SKColor[] skClrs = optimizedColorQuantizer.Quantize(colors, Size);
                swatches[i++] = optimizedColorQuantizer.FindAllColorVariations(skClrs, true);
            }

            return swatches;
        }

        [Benchmark]
        public ColorSwatch[] LastIteration()
        {
            ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
            int i = 0;
            foreach (SKColor[] colors in _colors)
            {
                SKColor[] skClrs = optimizedColorQuantizer3.Quantize(colors, Size);
                swatches[i++] = optimizedColorQuantizer3.FindAllColorVariations(skClrs, true);
            }

            return swatches;
        }
    }
}
