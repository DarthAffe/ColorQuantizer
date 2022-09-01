using System;
using System.Buffers;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace ColorQuantizer.Optimized8
{
    // DarthAffe 01.09.2022: It's a bit slower than the 3 split sorters but would prevent the code duplication
    internal class RadixLikeSort
    {
        #region Methods

        public static unsafe void Sort(in Span<SKColor> span, int offset)
        {
            ReadOnlySpan<byte> colorBytes = MemoryMarshal.AsBytes(span);
            fixed (byte* colorPtr = &MemoryMarshal.GetReference(colorBytes))
            {
                byte* current = colorPtr + offset;
                Span<int> counts = stackalloc int[256];
                for (int i = 0; i < span.Length; i++)
                {
                    counts[*current]++;
                    current += 4;
                }

                SKColor[][] bucketsArray = ArrayPool<SKColor[]>.Shared.Rent(256);
                Span<SKColor[]> buckets = bucketsArray.AsSpan(0, 256);
                for (int i = 0; i < counts.Length; i++)
                    buckets[i] = ArrayPool<SKColor>.Shared.Rent(counts[i]);

                Span<int> currentBucketIndex = stackalloc int[256];
                current = colorPtr + offset;
                for (int i = 0; i < span.Length; i++)
                {
                    int index = *current;
                    SKColor[] bucket = buckets[index];
                    int bucketIndex = currentBucketIndex[index];
                    currentBucketIndex[index]++;
                    bucket[bucketIndex] = span[i];
                    current += 4;
                }

                int newIndex = 0;
                for (int i = 0; i < buckets.Length; i++)
                {
                    Span<SKColor> bucket = buckets[i].AsSpan(0, counts[i]);
                    bucket.CopyTo(span[newIndex..]);
                    newIndex += bucket.Length;

                    ArrayPool<SKColor>.Shared.Return(buckets[i]);
                }

                ArrayPool<SKColor[]>.Shared.Return(bucketsArray);
            }
        }

        #endregion
    }
}
