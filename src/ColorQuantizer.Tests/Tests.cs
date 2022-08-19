using ColorQuantizer.Initial;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using ColorQuantizer.Optimized3;
using ColorQuantizer.Optimized4;
using ColorQuantizer.Optimized5;
using ColorQuantizer.Opzimized2;
using Xunit;

namespace ColorQuantizer.Tests
{
    public readonly struct ColorRanges
    {
        public readonly byte RedRange;
        public readonly byte GreenRange;
        public readonly byte BlueRange;

        public ColorRanges(byte redRange, byte greenRange, byte blueRange)
        {
            this.RedRange = redRange;
            this.GreenRange = greenRange;
            this.BlueRange = blueRange;
        }
    }

    public class Tests
    {
        private readonly InitialColorQuantizer initialQuantizer = new();
        private readonly OptimizedColorQuantizer3 optimizedQuantizer = new();
        private readonly OptimizedColorQuantizer4 optimizedQuantizer2 = new();
        private readonly OptimizedColorQuantizer5 optimizedQuantizer3 = new();
        
        private static IEnumerable<string> GetTestImages() => Directory.EnumerateFiles(@"..\..\..\..\sample_data", "*.jpg", SearchOption.AllDirectories);

        [Fact]
        public void All_impls_same_quantization()
        {
            foreach (string item in GetTestImages())
            {
                using FileStream stream = File.OpenRead(item);
                using SKBitmap bitmap = SKBitmap.Decode(stream);
                SKColor[] colors = bitmap.Pixels;

                SKColor[] a = initialQuantizer.Quantize(colors, 128);
                SKColor[] b = optimizedQuantizer.Quantize(colors, 128);
                SKColor[] c = optimizedQuantizer2.Quantize(colors, 128);
                SKColor[] d = optimizedQuantizer3.Quantize(colors, 128);

                Assert.Equal(a, b);
                Assert.Equal(a, c);
                Assert.Equal(a, d);
            }
        }

        [Fact]
        public void All_impls_same_swatch()
        {
            foreach (string item in GetTestImages())
            {
                using FileStream stream = File.OpenRead(item);
                using SKBitmap bitmap = SKBitmap.Decode(stream);
                SKColor[] colors = bitmap.Pixels;

                //quantize with one, check that both find the same final swatch.
                SKColor[] quantized = initialQuantizer.Quantize(colors, 128);

                Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true), optimizedQuantizer.FindAllColorVariations(quantized, true));
                Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true), optimizedQuantizer2.FindAllColorVariations(quantized, true));
                Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true), optimizedQuantizer3.FindAllColorVariations(quantized, true));
            }
        }

        [Fact]
        public void RangesEquals()
        {
            foreach (string item in GetTestImages())
            {
                using FileStream stream = File.OpenRead(item);
                using SKBitmap bitmap = SKBitmap.Decode(stream);
                SKColor[] colors = bitmap.Pixels;

                ColorRanges ranges1 = RangeBenchmark2(colors);
                ColorRanges ranges2 = RangeBenchmark3(colors);

                Assert.Equal(ranges1, ranges2);
            }
        }

        public ColorRanges RangeBenchmark2(Span<SKColor> colors)
        {
            byte redMin = byte.MaxValue;
            byte redMax = byte.MinValue;
            byte greenMin = byte.MaxValue;
            byte greenMax = byte.MinValue;
            byte blueMin = byte.MaxValue;
            byte blueMax = byte.MinValue;

            for (int i = 0; i < colors.Length; i++)
            {
                SKColor color = colors[i];
                if (color.Red < redMin) redMin = color.Red;
                if (color.Red > redMax) redMax = color.Red;
                if (color.Green < greenMin) greenMin = color.Green;
                if (color.Green > greenMax) greenMax = color.Green;
                if (color.Blue < blueMin) blueMin = color.Blue;
                if (color.Blue > blueMax) blueMax = color.Blue;
            }

            return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
        }
        
        public ColorRanges RangeBenchmark3(Span<SKColor> colors)
        {
            int elementsPerVector = Vector<byte>.Count / 3;
            int chunks = colors.Length / elementsPerVector;
            int missingElements = colors.Length - (chunks * elementsPerVector);

            Vector<byte> max = Vector<byte>.Zero;
            Vector<byte> min = new(byte.MaxValue);

            Span<byte> chunkData = stackalloc byte[Vector<byte>.Count];
            int dataIndex = 0;
            for (int i = 0; i < chunks; i++)
            {
                int chunkDataIndex = 0;
                for (int j = 0; j < elementsPerVector; j++)
                {
                    SKColor color = colors[dataIndex];
                    chunkData[chunkDataIndex] = color.Red;
                    ++chunkDataIndex;
                    chunkData[chunkDataIndex] = color.Green;
                    ++chunkDataIndex;
                    chunkData[chunkDataIndex] = color.Blue;
                    ++chunkDataIndex;
                    ++dataIndex;
                }

                Vector<byte> chunkVector = new(chunkData);
                max = Vector.Max(max, chunkVector);
                min = Vector.Min(min, chunkVector);
            }

            byte redMin = byte.MaxValue;
            byte redMax = byte.MinValue;
            byte greenMin = byte.MaxValue;
            byte greenMax = byte.MinValue;
            byte blueMin = byte.MaxValue;
            byte blueMax = byte.MinValue;

            int vectorEntries = elementsPerVector * 3;
            for (int i = 0; i < vectorEntries; i += 3)
            {
                if (min[i] < redMin) redMin = min[i];
                if (max[i] > redMax) redMax = max[i];
                if (min[i + 1] < greenMin) greenMin = min[i + 1];
                if (max[i + 1] > greenMax) greenMax = max[i + 1];
                if (min[i + 2] < blueMin) blueMin = min[i + 2];
                if (max[i + 2] > blueMax) blueMax = max[i + 2];
            }

            for (int i = 0; i < missingElements; i++)
            {
                SKColor color = colors[dataIndex];
                if (color.Red < redMin) redMin = color.Red;
                if (color.Red > redMax) redMax = color.Red;
                if (color.Green < greenMin) greenMin = color.Green;
                if (color.Green > greenMax) greenMax = color.Green;
                if (color.Blue < blueMin) blueMin = color.Blue;
                if (color.Blue > blueMax) blueMax = color.Blue;

                ++dataIndex;
            }

            return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
        }
    }
}
