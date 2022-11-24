using System;
using System.Buffers;
using SkiaSharp;

namespace ColorQuantizer.Optimized13
{
    internal static unsafe class QuantizerSortPointer
    {
        #region Methods

        public static void SortRed(in Span<SKColor> colors)
        {
            fixed (SKColor* ptr = &colors.GetPinnableReference())
            {
                SKColor* end = ptr + colors.Length;

                Span<int> counts = stackalloc int[256];

                for (SKColor* color = ptr; color < end; ++color)
                    counts[(*color).Red]++;

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

                    for (SKColor* color = ptr; color < end; ++color)
                    {
                        int index = (*color).Red;
                        int bucketIndex = currentBucketIndex[index];
                        currentBucketIndex[index]++;
                        buckets[bucketIndex] = (*color);
                    }

                    buckets.CopyTo(colors);
                }
                finally
                {
                    ArrayPool<SKColor>.Shared.Return(bucketsArray);
                }
            }
        }

        public static void SortGreen(in Span<SKColor> colors)
        {
            fixed (SKColor* ptr = &colors.GetPinnableReference())
            {
                SKColor* end = ptr + colors.Length;

                Span<int> counts = stackalloc int[256];

                for (SKColor* color = ptr; color < end; ++color)
                    counts[(*color).Green]++;

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

                    for (SKColor* color = ptr; color < end; ++color)
                    {
                        int index = (*color).Green;
                        int bucketIndex = currentBucketIndex[index];
                        currentBucketIndex[index]++;
                        buckets[bucketIndex] = (*color);
                    }

                    buckets.CopyTo(colors);
                }
                finally
                {
                    ArrayPool<SKColor>.Shared.Return(bucketsArray);
                }
            }
        }

        public static void SortBlue(in Span<SKColor> colors)
        {
            fixed (SKColor* ptr = &colors.GetPinnableReference())
            {
                SKColor* end = ptr + colors.Length;

                Span<int> counts = stackalloc int[256];

                for (SKColor* color = ptr; color < end; ++color)
                    counts[(*color).Blue]++;

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

                    for (SKColor* color = ptr; color < end; ++color)
                    {
                        int index = (*color).Blue;
                        int bucketIndex = currentBucketIndex[index];
                        currentBucketIndex[index]++;
                        buckets[bucketIndex] = (*color);
                    }

                    buckets.CopyTo(colors);
                }
                finally
                {
                    ArrayPool<SKColor>.Shared.Return(bucketsArray);
                }
            }
        }

        #endregion
    }
}
