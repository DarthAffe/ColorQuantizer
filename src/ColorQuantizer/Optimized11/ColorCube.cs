﻿using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;
using SkiaSharp;

namespace ColorQuantizer.Optimized11
{
    internal readonly struct ColorRanges
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

    internal readonly struct ColorCube
    {
        private const int BYTES_PER_COLOR = 4;
        private static readonly int ELEMENTS_PER_VECTOR = Vector<byte>.Count / BYTES_PER_COLOR;
        private static readonly int BYTES_PER_VECTOR = ELEMENTS_PER_VECTOR * BYTES_PER_COLOR;

        private readonly int _from;
        private readonly int _length;
        private readonly SortTarget _currentOrder = SortTarget.None;

        public ColorCube(in Span<uint> fullColorList, int from, int length, SortTarget preOrdered)
        {
            this._from = from;
            this._length = length;

            if (length < 2) return;

            Span<uint> colors = fullColorList.Slice(from, length);
            ColorRanges colorRanges = GetColorRanges(colors);

            if ((colorRanges.RedRange > colorRanges.GreenRange) && (colorRanges.RedRange > colorRanges.BlueRange))
            {
                if (preOrdered != SortTarget.Red)
                    QuantizerSort.Sort(colors, 16);

                _currentOrder = SortTarget.Red;
            }
            else if (colorRanges.GreenRange > colorRanges.BlueRange)
            {
                if (preOrdered != SortTarget.Green)
                    QuantizerSort.Sort(colors, 8);

                _currentOrder = SortTarget.Green;
            }
            else
            {
                if (preOrdered != SortTarget.Blue)
                    QuantizerSort.Sort(colors, 0);

                _currentOrder = SortTarget.Blue;
            }
        }

        private ColorRanges GetColorRanges(in ReadOnlySpan<uint> colors)
        {
            if (Vector.IsHardwareAccelerated && (colors.Length >= Vector<byte>.Count))
            {
                int chunks = colors.Length / ELEMENTS_PER_VECTOR;
                int vectorElements = (chunks * ELEMENTS_PER_VECTOR);
                int missingElements = colors.Length - vectorElements;

                Vector<byte> max = Vector<byte>.Zero;
                Vector<byte> min = new(byte.MaxValue);
                foreach (Vector<byte> currentVector in colors[..vectorElements].Cast<uint, Vector<byte>>())
                {
                    max = Vector.Max(max, currentVector);
                    min = Vector.Min(min, currentVector);
                }

                byte redMin = byte.MaxValue;
                byte redMax = byte.MinValue;
                byte greenMin = byte.MaxValue;
                byte greenMax = byte.MinValue;
                byte blueMin = byte.MaxValue;
                byte blueMax = byte.MinValue;

                for (int i = 0; i < BYTES_PER_VECTOR; i += BYTES_PER_COLOR)
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
                    uint color = colors[^(i + 1)];

                    if (color.Red() < redMin) redMin = color.Red();
                    if (color.Red() > redMax) redMax = color.Red();
                    if (color.Green() < greenMin) greenMin = color.Green();
                    if (color.Green() > greenMax) greenMax = color.Green();
                    if (color.Blue() < blueMin) blueMin = color.Blue();
                    if (color.Blue() > blueMax) blueMax = color.Blue();
                }

                return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
            }
            else
            {
                byte redMin = byte.MaxValue;
                byte redMax = byte.MinValue;
                byte greenMin = byte.MaxValue;
                byte greenMax = byte.MinValue;
                byte blueMin = byte.MaxValue;
                byte blueMax = byte.MinValue;

                foreach (uint color in colors)
                {
                    if (color.Red() < redMin) redMin = color.Red();
                    if (color.Red() > redMax) redMax = color.Red();
                    if (color.Green() < greenMin) greenMin = color.Green();
                    if (color.Green() > greenMax) greenMax = color.Green();
                    if (color.Blue() < blueMin) blueMin = color.Blue();
                    if (color.Blue() > blueMax) blueMax = color.Blue();
                }

                return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
            }
        }

        internal void Split(in Span<uint> fullColorList, out ColorCube a, out ColorCube b)
        {
            Span<uint> colors = fullColorList.Slice(_from, _length);

            int median = colors.Length / 2;

            a = new ColorCube(fullColorList, _from, median, _currentOrder);
            b = new ColorCube(fullColorList, _from + median, colors.Length - median, _currentOrder);
        }

        internal SKColor GetAverageColor(in ReadOnlySpan<uint> fullColorList)
        {
            ReadOnlySpan<uint> colors = fullColorList.Slice(_from, _length);

            uint r = 0, g = 0, b = 0;
            foreach (uint color in colors)
            {
                r += color.Red();
                g += color.Green();
                b += color.Blue();
            }

            return new SKColor(
                (byte)(r / colors.Length),
                (byte)(g / colors.Length),
                (byte)(b / colors.Length)
            );
        }
    }

    internal static class UIntColorExteions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Alpha(this uint color) => (byte)((color >> 24) & (uint)byte.MaxValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Red(this uint color) => (byte)((color >> 16) & (uint)byte.MaxValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Green(this uint color) => (byte)((color >> 8) & (uint)byte.MaxValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Blue(this uint color) => (byte)(color & (uint)byte.MaxValue);
    }
}
