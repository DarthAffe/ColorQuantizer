using System;
using System.Buffers;
using SkiaSharp;

namespace ColorQuantizer.Optimized13
{
    internal static class QuantizerSortFor
    {
        #region Methods

        public static void SortRed(in Span<SKColor> colors)
        {
            Span<int> counts = stackalloc int[256];
            for (int i = 0; i < colors.Length; i++)
            {
                SKColor color = colors[i];
                counts[color.Red]++;
            }

            SKColor[] bucketsArray = ArrayPool<SKColor>.Shared.Rent(colors.Length);

            try
            {
                Span<SKColor> buckets = bucketsArray.AsSpan().Slice(0, colors.Length);
                Span<int> currentBucketIndex = stackalloc int[256];

                int offset = 0;
                for (int i = 0; i < counts.Length; i++)
                {
                    currentBucketIndex[i] = offset;
                    offset += counts[i];
                }

                for (int i = 0; i < colors.Length; i++)
                {
                    SKColor color = colors[i];
                    int index = color.Red;
                    int bucketIndex = currentBucketIndex[index];
                    currentBucketIndex[index]++;
                    buckets[bucketIndex] = color;
                }

                buckets.CopyTo(colors);
            }
            finally
            {
                ArrayPool<SKColor>.Shared.Return(bucketsArray);
            }
        }

        public static void SortGreen(in Span<SKColor> colors)
        {
            Span<int> counts = stackalloc int[256];
            for (int i = 0; i < colors.Length; i++)
            {
                SKColor color = colors[i];
                counts[color.Green]++;
            }

            SKColor[] bucketsArray = ArrayPool<SKColor>.Shared.Rent(colors.Length);

            try
            {
                Span<SKColor> buckets = bucketsArray.AsSpan().Slice(0, colors.Length);
                Span<int> currentBucketIndex = stackalloc int[256];

                int offset = 0;
                for (int i = 0; i < counts.Length; i++)
                {
                    currentBucketIndex[i] = offset;
                    offset += counts[i];
                }

                for (int i = 0; i < colors.Length; i++)
                {
                    SKColor color = colors[i];
                    int index = color.Green;
                    int bucketIndex = currentBucketIndex[index];
                    currentBucketIndex[index]++;
                    buckets[bucketIndex] = color;
                }

                buckets.CopyTo(colors);
            }
            finally
            {
                ArrayPool<SKColor>.Shared.Return(bucketsArray);
            }
        }

        public static void SortBlue(in Span<SKColor> colors)
        {
            Span<int> counts = stackalloc int[256];
            for (int i = 0; i < colors.Length; i++)
            {
                SKColor color = colors[i];
                counts[color.Blue]++;
            }

            SKColor[] bucketsArray = ArrayPool<SKColor>.Shared.Rent(colors.Length);

            try
            {
                Span<SKColor> buckets = bucketsArray.AsSpan().Slice(0, colors.Length);
                Span<int> currentBucketIndex = stackalloc int[256];

                int offset = 0;
                for (int i = 0; i < counts.Length; i++)
                {
                    currentBucketIndex[i] = offset;
                    offset += counts[i];
                }

                for (int i = 0; i < colors.Length; i++)
                {
                    SKColor color = colors[i];
                    int index = color.Blue;
                    int bucketIndex = currentBucketIndex[index];
                    currentBucketIndex[index]++;
                    buckets[bucketIndex] = color;
                }

                buckets.CopyTo(colors);
            }
            finally
            {
                ArrayPool<SKColor>.Shared.Return(bucketsArray);
            }
        }

        #endregion
    }
}
