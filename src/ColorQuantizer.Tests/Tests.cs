using ColorQuantizer.Initial;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using ColorQuantizer.Optimized10;
using ColorQuantizer.Optimized11;
using ColorQuantizer.Optimized12;
using ColorQuantizer.Optimized13;
using ColorQuantizer.Optimized3;
using ColorQuantizer.Optimized4;
using ColorQuantizer.Optimized5;
using ColorQuantizer.Optimized6;
using ColorQuantizer.Optimized7;
using ColorQuantizer.Optimized8;
using ColorQuantizer.Optimized9;
using ColorQuantizer.Opzimized2;
using ColorQuantizer.Shared;
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
        private readonly OptimizedColorQuantizer3 optimizedQuantize3 = new();
        private readonly OptimizedColorQuantizer4 optimizedQuantizer4 = new();
        private readonly OptimizedColorQuantizer5 optimizedQuantizer5 = new();
        private readonly OptimizedColorQuantizer6 optimizedQuantizer6 = new();
        private readonly OptimizedColorQuantizer7 optimizedQuantizer7 = new();
        private readonly OptimizedColorQuantizer8 optimizedQuantizer8 = new();

        private static IEnumerable<string> GetTestImages() =>
            Directory.EnumerateFiles(@"..\..\..\..\sample_data", "*.jpg", SearchOption.AllDirectories);

        [Fact]
        public void All_impls_same_quantization()
        {
            foreach (string item in GetTestImages())
            {
                using FileStream stream = File.OpenRead(item);
                using SKBitmap bitmap = SKBitmap.Decode(stream);
                SKColor[] colors = bitmap.Pixels;

                //SKColor[] a = initialQuantizer.Quantize(colors, 128);
                //SKColor[] b = optimizedQuantize3.Quantize(colors, 128);
                //SKColor[] c = optimizedQuantizer4.Quantize(colors, 128);
                //SKColor[] d = optimizedQuantizer5.Quantize(colors, 128);
                //SKColor[] e = optimizedQuantizer6.Quantize(colors, 128);
                //SKColor[] f = optimizedQuantizer7.Quantize(colors, 128);
                //SKColor[] g = optimizedQuantizer8.Quantize(colors, 128);
                //SKColor[] h = OptimizedColorQuantizer9.Quantize(colors, 128);
                //SKColor[] i = OptimizedColorQuantizer11.Quantize(colors, 128);
                SKColor[] i = OptimizedColorQuantizer12.Quantize(colors, 128);
                //SKColor[] j = OptimizedColorQuantizer13Pointer.Quantize(colors, 128);
                SKColor[] k = OptimizedColorQuantizer13AllPointer.Quantize(colors, 128);
                //SKColor[] l = OptimizedColorQuantizer13Foreach.Quantize(colors, 128);

                //Assert.Equal(a, b);
                //Assert.Equal(a, c);
                //Assert.Equal(a, d);
                //Assert.Equal(a, e);
                //Assert.Equal(a, f);
                //Assert.Equal(a, g);

                //Assert.Equal(j, i);
                //Assert.Equal(l, i);
                Assert.Equal(k, i);
                //Assert.Equal(a, h);
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
                SKColor[] quantized = initialQuantizer.Quantize(colors, 256);

                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             optimizedQuantize3.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             optimizedQuantizer4.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             optimizedQuantizer5.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             optimizedQuantizer6.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             optimizedQuantizer7.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             optimizedQuantizer8.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             OptimizedColorQuantizer9.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             OptimizedColorQuantizer10.FindAllColorVariations(quantized, true));
                //Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                //             OptimizedColorQuantizer11.FindAllColorVariations(quantized, true));
                Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                             OptimizedColorQuantizer13Pointer.FindAllColorVariations(quantized, true));
                Assert.Equal(initialQuantizer.FindAllColorVariations(quantized, true),
                             OptimizedColorQuantizer13AllPointer.FindAllColorVariations(quantized, true));
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

                //SKColor[] colors = new[]
                //                   {
                //                       new SKColor(1, 10, 110),
                //                       new SKColor(2, 20, 120),
                //                       new SKColor(3, 30, 130),
                //                       new SKColor(4, 40, 140),
                //                       new SKColor(5, 50, 150),
                //                       new SKColor(6, 60, 160),
                //                       new SKColor(7, 70, 170),
                //                       new SKColor(8, 80, 180),
                //                       new SKColor(5, 50, 150),
                //                       new SKColor(5, 50, 150),
                //                       new SKColor(5, 50, 150),
                //                       new SKColor(5, 50, 150),
                //                   };

                ColorRanges ranges1 = RangeBenchmark2(colors);
                ColorRanges ranges2 = RangeBenchmark3(colors);
                ColorRanges ranges3 = RangeBenchmark4(colors.Select(x => new Color(x.Red, x.Green, x.Blue)).ToArray());
                ColorRanges ranges4 = RangeBenchmark5(colors);

                Assert.Equal(ranges1, ranges2);
                Assert.Equal(ranges1, ranges3);
                Assert.Equal(ranges1, ranges4);
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

        public unsafe ColorRanges RangeBenchmark4(Span<Color> colors)
        {
            int ELEMENTS_PER_VECTOR = Vector<byte>.Count / 3;
            int BYTES_PER_VECTOR = ELEMENTS_PER_VECTOR * 3;

            int chunks = colors.Length / ELEMENTS_PER_VECTOR;
            int missingElements = colors.Length - (chunks * ELEMENTS_PER_VECTOR);

            Vector<byte> max = Vector<byte>.Zero;
            Vector<byte> min = new(byte.MaxValue);

            ReadOnlySpan<byte> colorBytes = MemoryMarshal.AsBytes(colors);
            fixed (byte* colorPtr = &MemoryMarshal.GetReference(colorBytes))
            {
                byte* current = colorPtr;

                for (int i = 0; i < chunks; i++)
                {
                    Vector<byte> currentVector = *(Vector<byte>*)current;

                    max = Vector.Max(max, currentVector);
                    min = Vector.Min(min, currentVector);

                    current += BYTES_PER_VECTOR;
                }

                byte redMin = byte.MaxValue;
                byte redMax = byte.MinValue;
                byte greenMin = byte.MaxValue;
                byte greenMax = byte.MinValue;
                byte blueMin = byte.MaxValue;
                byte blueMax = byte.MinValue;

                int vectorEntries = ELEMENTS_PER_VECTOR * 3;
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
                    Color color = colors[^(i + 1)];

                    if (color.Red < redMin) redMin = color.Red;
                    if (color.Red > redMax) redMax = color.Red;
                    if (color.Green < greenMin) greenMin = color.Green;
                    if (color.Green > greenMax) greenMax = color.Green;
                    if (color.Blue < blueMin) blueMin = color.Blue;
                    if (color.Blue > blueMax) blueMax = color.Blue;
                }

                return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
            }
        }

        public unsafe ColorRanges RangeBenchmark5(Span<SKColor> colors)
        {
            int ELEMENTS_PER_VECTOR = Vector<byte>.Count / 4;
            int BYTES_PER_VECTOR = ELEMENTS_PER_VECTOR * 4;

            int chunks = colors.Length / ELEMENTS_PER_VECTOR;
            int missingElements = colors.Length - (chunks * ELEMENTS_PER_VECTOR);

            Vector<byte> max = Vector<byte>.Zero;
            Vector<byte> min = new(byte.MaxValue);

            ReadOnlySpan<byte> colorBytes = MemoryMarshal.AsBytes(colors);
            fixed (byte* colorPtr = &MemoryMarshal.GetReference(colorBytes))
            {
                byte* current = colorPtr;

                for (int i = 0; i < chunks; i++)
                {
                    Vector<byte> currentVector = *(Vector<byte>*)current;

                    max = Vector.Max(max, currentVector);
                    min = Vector.Min(min, currentVector);

                    current += BYTES_PER_VECTOR;
                }

                byte redMin = byte.MaxValue;
                byte redMax = byte.MinValue;
                byte greenMin = byte.MaxValue;
                byte greenMax = byte.MinValue;
                byte blueMin = byte.MaxValue;
                byte blueMax = byte.MinValue;

                for (int i = 0; i < BYTES_PER_VECTOR; i += 4)
                {
                    if (min[i + 2] < redMin) redMin = min[i + 2];
                    if (max[i + 2] > redMax) redMax = max[i + 2];
                    if (min[i + 1] < greenMin) greenMin = min[i + 1];
                    if (max[i + 1] > greenMax) greenMax = max[i + 1];
                    if (min[i] < blueMin) blueMin = min[i];
                    if (max[i] > blueMax) blueMax = max[i];
                }

                for (int i = 0; i < missingElements; i++)
                {
                    SKColor color = colors[^(i + 1)];

                    if (color.Red < redMin) redMin = color.Red;
                    if (color.Red > redMax) redMax = color.Red;
                    if (color.Green < greenMin) greenMin = color.Green;
                    if (color.Green > greenMax) greenMax = color.Green;
                    if (color.Blue < blueMin) blueMin = color.Blue;
                    if (color.Blue > blueMax) blueMax = color.Blue;
                }

                return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
            }
        }
    }
}
