using System;
using System.Buffers;
using ColorQuantizer.Shared;

namespace ColorQuantizer.Optimized7
{
    internal class RadixLikeSortBlue
    {
        #region Methods

        public static void Sort(in Span<Color> span)
        {
            Span<int> counts = stackalloc int[256];
            foreach (Color t in span)
                counts[t.Blue]++;

            Color[][] bucketsArray = ArrayPool<Color[]>.Shared.Rent(256);
            Span<Color[]> buckets = bucketsArray.AsSpan(0, 256);
            for (int i = 0; i < counts.Length; i++)
                buckets[i] = ArrayPool<Color>.Shared.Rent(counts[i]);

            Span<int> currentBucketIndex = stackalloc int[256];
            foreach (Color color in span)
            {
                int index = color.Blue;
                Color[] bucket = buckets[index];
                int bucketIndex = currentBucketIndex[index];
                currentBucketIndex[index]++;
                bucket[bucketIndex] = color;
            }

            int newIndex = 0;
            for (int i = 0; i < buckets.Length; i++)
            {
                Span<Color> bucket = buckets[i].AsSpan(0, counts[i]);
                bucket.CopyTo(span.Slice(newIndex));
                newIndex += bucket.Length;

                ArrayPool<Color>.Shared.Return(buckets[i]);
            }

            ArrayPool<Color[]>.Shared.Return(bucketsArray);
        }

        #endregion
    }
}
