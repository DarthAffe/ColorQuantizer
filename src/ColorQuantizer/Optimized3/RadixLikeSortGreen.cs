﻿using System;
using System.Buffers;
using SkiaSharp;

namespace ColorQuantizer.Optimized3
{
    internal class RadixLikeSortGreen
    {
        #region Methods

        public static void Sort(in Span<SKColor> span)
        {
            Span<int> counts = stackalloc int[256];
            for (int i = 0; i < span.Length; i++)
                counts[span[i].Green]++;

            SKColor[][] bucketsArray = ArrayPool<SKColor[]>.Shared.Rent(256);
            Span<SKColor[]> buckets = bucketsArray.AsSpan(0, 256);
            for (int i = 0; i < counts.Length; i++)
                buckets[i] = ArrayPool<SKColor>.Shared.Rent(counts[i]);

            Span<int> currentBucketIndex = stackalloc int[256];
            for (int i = 0; i < span.Length; i++)
            {
                SKColor color = span[i];
                int index = color.Green;
                SKColor[] bucket = buckets[index];
                int bucketIndex = currentBucketIndex[index];
                currentBucketIndex[index]++;
                bucket[bucketIndex] = color;
            }

            int newIndex = 0;
            for (int i = 0; i < buckets.Length; i++)
            {
                Span<SKColor> bucket = buckets[i].AsSpan(0, counts[i]);
                for (int j = 0; j < bucket.Length; j++)
                    span[newIndex++] = bucket[j];

                ArrayPool<SKColor>.Shared.Return(buckets[i]);
            }

            ArrayPool<SKColor[]>.Shared.Return(bucketsArray);
        }

        #endregion
    }
}
