using System;
using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using ColorQuantizer.Initial;
using SkiaSharp;
using System.IO;
using System.Linq;
using System.Numerics;
using ColorQuantizer.Optimized3;
using ColorQuantizer.Optimized4;
using ColorQuantizer.Optimized5;
using ColorQuantizer.Optimized6;
using ColorQuantizer.Optimized7;
using ColorQuantizer.Optimized8;
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
        private readonly OptimizedColorQuantizer4 optimizedColorQuantizer4 = new();
        private readonly OptimizedColorQuantizer5 optimizedColorQuantizer5 = new();
        private readonly OptimizedColorQuantizer6 optimizedColorQuantizer6 = new();
        private readonly OptimizedColorQuantizer7 optimizedColorQuantizer7 = new();
        private readonly OptimizedColorQuantizer8 optimizedColorQuantizer8 = new();
        private readonly List<SKBitmap> _bitmap;
        private readonly List<SKColor[]> _colors;

        private readonly SKColor[][] _rangeTestColor;

        [Params(32, 128)]
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

            //_rangeTestColor = new SKColor[10][];
            //_rangeTestColor[0] = _colors[0].Take(10).ToArray();
            //_rangeTestColor[1] = _colors[0].Take(20).ToArray();
            //_rangeTestColor[2] = _colors[0].Take(30).ToArray();
            //_rangeTestColor[3] = _colors[0].Take(40).ToArray();
            //_rangeTestColor[4] = _colors[0].Take(50).ToArray();
            //_rangeTestColor[5] = _colors[0].Take(60).ToArray();
            //_rangeTestColor[6] = _colors[0].Take(70).ToArray();
            //_rangeTestColor[7] = _colors[0].Take(80).ToArray();
            //_rangeTestColor[8] = _colors[0].Take(90).ToArray();
            //_rangeTestColor[9] = _colors[0].Take(100).ToArray();
        }

        //[Params(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)]
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

        //[Benchmark]
        //public ColorRanges RangeBenchmark3()
        //{
        //    Span<SKColor> colors = _rangeTestColor[Amount];

        //    int elementsPerVector = Vector<byte>.Count / 3;
        //    int chunks = colors.Length / elementsPerVector;
        //    int missingElements = colors.Length - (chunks * elementsPerVector);

        //    Vector<byte> max = Vector<byte>.Zero;
        //    Vector<byte> min = new(byte.MaxValue);

        //    Span<byte> chunkData = stackalloc byte[Vector<byte>.Count];
        //    int dataIndex = 0;
        //    for (int i = 0; i < chunks; i++)
        //    {
        //        int chunkDataIndex = 0;
        //        for (int j = 0; j < elementsPerVector; j++)
        //        {
        //            SKColor color = colors[dataIndex];
        //            chunkData[chunkDataIndex] = color.Red;
        //            ++chunkDataIndex;
        //            chunkData[chunkDataIndex] = color.Green;
        //            ++chunkDataIndex;
        //            chunkData[chunkDataIndex] = color.Blue;
        //            ++chunkDataIndex;
        //            ++dataIndex;
        //        }

        //        Vector<byte> chunkVector = new(chunkData);
        //        max = Vector.Max(max, chunkVector);
        //        min = Vector.Min(min, chunkVector);
        //    }

        //    byte redMin = byte.MaxValue;
        //    byte redMax = byte.MinValue;
        //    byte greenMin = byte.MaxValue;
        //    byte greenMax = byte.MinValue;
        //    byte blueMin = byte.MaxValue;
        //    byte blueMax = byte.MinValue;

        //    int vectorEntries = elementsPerVector * 3;
        //    for (int i = 0; i < vectorEntries; i += 3)
        //    {
        //        if (min[i] < redMin) redMin = min[i];
        //        if (max[i] > redMax) redMax = max[i];
        //        if (min[i + 1] < greenMin) greenMin = min[i + 1];
        //        if (max[i + 1] > greenMax) greenMax = max[i + 1];
        //        if (min[i + 2] < blueMin) blueMin = min[i + 2];
        //        if (max[i + 2] > blueMax) blueMax = max[i + 2];
        //    }

        //    for (int i = 0; i < missingElements; i++)
        //    {
        //        SKColor color = colors[dataIndex];
        //        if (color.Red < redMin) redMin = color.Red;
        //        if (color.Red > redMax) redMax = color.Red;
        //        if (color.Green < greenMin) greenMin = color.Green;
        //        if (color.Green > greenMax) greenMax = color.Green;
        //        if (color.Blue < blueMin) blueMin = color.Blue;
        //        if (color.Blue > blueMax) blueMax = color.Blue;

        //        ++dataIndex;
        //    }

        //    return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
        //}

        //[Benchmark]
        //public ColorSwatch[] Old128()
        //{
        //    ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
        //    int i = 0;
        //    foreach (SKColor[] colors in _colors)
        //    {
        //        SKColor[] skClrs = initialColorQuantizer.Quantize(colors, Size);
        //        swatches[i++] = initialColorQuantizer.FindAllColorVariations(skClrs, true);
        //    }

        //    return swatches;
        //}

        //[Benchmark]
        //public ColorSwatch[] New()
        //{
        //    ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
        //    int i = 0;
        //    foreach (SKColor[] colors in _colors)
        //    {
        //        SKColor[] skClrs = optimizedColorQuantizer.Quantize(colors, Size);
        //        swatches[i++] = optimizedColorQuantizer.FindAllColorVariations(skClrs, true);
        //    }

        //    return swatches;
        //}

        //[Benchmark]
        //public ColorSwatch[] LastIteration()
        //{
        //    ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
        //    int i = 0;
        //    foreach (SKColor[] colors in _colors)
        //    {
        //        SKColor[] skClrs = optimizedColorQuantizer3.Quantize(colors, Size);
        //        swatches[i++] = optimizedColorQuantizer3.FindAllColorVariations(skClrs, true);
        //    }

        //    return swatches;
        //}

        //[Benchmark]
        //public ColorSwatch[] MoreOptimizations()
        //{
        //    ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
        //    int i = 0;
        //    foreach (SKColor[] colors in _colors)
        //    {
        //        SKColor[] skClrs = optimizedColorQuantizer4.Quantize(colors, Size);
        //        swatches[i++] = optimizedColorQuantizer4.FindAllColorVariations(skClrs, true);
        //    }

        //    return swatches;
        //}

        [Benchmark]
        public ColorSwatch[] EvenMoreOptimizations()
        {
            ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
            int i = 0;
            foreach (SKColor[] colors in _colors)
            {
                SKColor[] skClrs = optimizedColorQuantizer5.Quantize(colors, Size);
                swatches[i++] = optimizedColorQuantizer5.FindAllColorVariations(skClrs, true);
            }

            return swatches;
        }

        //[Benchmark]
        //public ColorSwatch[] CustomColor()
        //{
        //    ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
        //    int i = 0;
        //    foreach (SKColor[] colors in _colors)
        //    {
        //        SKColor[] skClrs = optimizedColorQuantizer6.Quantize(colors, Size);
        //        swatches[i++] = optimizedColorQuantizer6.FindAllColorVariations(skClrs, true);
        //    }

        //    return swatches;
        //}

        //[Benchmark]
        //public ColorSwatch[] MoreIntrinsicsCustom()
        //{
        //    ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
        //    int i = 0;
        //    foreach (SKColor[] colors in _colors)
        //    {
        //        SKColor[] skClrs = optimizedColorQuantizer7.Quantize(colors, Size);
        //        swatches[i++] = optimizedColorQuantizer7.FindAllColorVariations(skClrs, true);
        //    }

        //    return swatches;
        //}

        [Benchmark]
        public ColorSwatch[] MoreIntrinsics()
        {
            ColorSwatch[] swatches = new ColorSwatch[_colors.Count];
            int i = 0;
            foreach (SKColor[] colors in _colors)
            {
                SKColor[] skClrs = optimizedColorQuantizer8.Quantize(colors, Size);
                swatches[i++] = optimizedColorQuantizer8.FindAllColorVariations(skClrs, true);
            }

            return swatches;
        }
    }
}
