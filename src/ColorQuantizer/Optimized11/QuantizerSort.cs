using System;
using System.Buffers;

namespace ColorQuantizer.Optimized11
{
    internal static class QuantizerSort
    {
        #region Methods

        public static void Sort(in Span<uint> colors, int shift)
        {
            Span<int> counts = stackalloc int[256];
            foreach (uint c in colors)
                counts[(byte)((c >> shift) & (uint)byte.MaxValue)]++;

            uint[] bucketsArray = ArrayPool<uint>.Shared.Rent(colors.Length);
            try
            {
                Span<uint> buckets = bucketsArray.AsSpan(0, colors.Length);
                Span<int> currentBucketIndex = stackalloc int[256];

                int offset = 0;
                for (int i = 0; i < counts.Length; i++)
                {
                    currentBucketIndex[i] = offset;
                    offset += counts[i];
                }

                foreach (uint color in colors)
                {
                    int index = (byte)((color >> shift) & (uint)byte.MaxValue);
                    int bucketIndex = currentBucketIndex[index];
                    currentBucketIndex[index]++;
                    buckets[bucketIndex] = color;
                }

                buckets.CopyTo(colors);
            }
            finally
            {
                ArrayPool<uint>.Shared.Return(bucketsArray);
            }
        }

        //public static void SortRed(in Span<QuantizerColor> span)
        //{
        //    Span<int> counts = stackalloc int[256];
        //    foreach (QuantizerColor t in span)
        //        counts[t.Red]++;

        //    QuantizerColor[] bucketsArray = ArrayPool<QuantizerColor>.Shared.Rent(span.Length);
        //    Span<QuantizerColor> buckets = bucketsArray.AsSpan(0, span.Length);
        //    Span<int> currentBucketIndex = stackalloc int[256];

        //    int offset = 0;
        //    for (int i = 0; i < counts.Length; i++)
        //    {
        //        currentBucketIndex[i] = offset;
        //        offset += counts[i];
        //    }

        //    foreach (QuantizerColor color in span)
        //    {
        //        int index = color.Red;
        //        int bucketIndex = currentBucketIndex[index];
        //        currentBucketIndex[index]++;
        //        buckets[bucketIndex] = color;
        //    }

        //    buckets.CopyTo(span);

        //    ArrayPool<QuantizerColor>.Shared.Return(bucketsArray);
        //}

        //public static void SortGreen(in Span<QuantizerColor> span)
        //{
        //    Span<int> counts = stackalloc int[256];
        //    foreach (QuantizerColor t in span)
        //        counts[t.Green]++;

        //    QuantizerColor[] bucketsArray = ArrayPool<QuantizerColor>.Shared.Rent(span.Length);
        //    Span<QuantizerColor> buckets = bucketsArray.AsSpan(0, span.Length);
        //    Span<int> currentBucketIndex = stackalloc int[256];

        //    int offset = 0;
        //    for (int i = 0; i < counts.Length; i++)
        //    {
        //        currentBucketIndex[i] = offset;
        //        offset += counts[i];
        //    }

        //    foreach (QuantizerColor color in span)
        //    {
        //        int index = color.Green;
        //        int bucketIndex = currentBucketIndex[index];
        //        currentBucketIndex[index]++;
        //        buckets[bucketIndex] = color;
        //    }

        //    buckets.CopyTo(span);

        //    ArrayPool<QuantizerColor>.Shared.Return(bucketsArray);
        //}

        //public static void SortBlue(in Span<QuantizerColor> span)
        //{
        //    Span<int> counts = stackalloc int[256];
        //    foreach (QuantizerColor t in span)
        //        counts[t.Blue]++;

        //    QuantizerColor[] bucketsArray = ArrayPool<QuantizerColor>.Shared.Rent(span.Length);
        //    Span<QuantizerColor> buckets = bucketsArray.AsSpan(0, span.Length);
        //    Span<int> currentBucketIndex = stackalloc int[256];

        //    int offset = 0;
        //    for (int i = 0; i < counts.Length; i++)
        //    {
        //        currentBucketIndex[i] = offset;
        //        offset += counts[i];
        //    }

        //    foreach (QuantizerColor color in span)
        //    {
        //        int index = color.Blue;
        //        int bucketIndex = currentBucketIndex[index];
        //        currentBucketIndex[index]++;
        //        buckets[bucketIndex] = color;
        //    }

        //    buckets.CopyTo(span);

        //    ArrayPool<QuantizerColor>.Shared.Return(bucketsArray);
        //}

        #endregion
    }
}
