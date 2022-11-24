using System;
using System.Buffers;
using SkiaSharp;

namespace ColorQuantizer.Optimized13
{
    internal static unsafe class QuantizerSortAllPointer
    {
        #region Methods

        public static void SortRed(in Span<SKColor> colors)
        {
            fixed (SKColor* colorsPtr = &colors.GetPinnableReference())
            {
                SKColor* colorsEnd = colorsPtr + colors.Length;

                Span<int> counts = stackalloc int[256];
                fixed (int* countsPtr = &counts.GetPinnableReference())
                {
                    int* countsEnd = countsPtr + counts.Length;

                    for (SKColor* color = colorsPtr; color < colorsEnd; ++color)
                        ++(*(countsPtr + (*color).Red));

                    SKColor[] bucketsArray = ArrayPool<SKColor>.Shared.Rent(colors.Length);

                    try
                    {
                        Span<SKColor> buckets = bucketsArray.AsSpan().Slice(0, colors.Length);
                        fixed (SKColor* bucketsPtr = &buckets.GetPinnableReference())
                        {
                            Span<int> currentBucketIndex = stackalloc int[256];
                            fixed (int* currentBucketIndexPtr = &currentBucketIndex.GetPinnableReference())
                            {
                                int offset = 0;
                                int* currentBucketIndx = currentBucketIndexPtr;
                                for (int* count = countsPtr; count < countsEnd; ++count, ++currentBucketIndx)
                                {
                                    (*currentBucketIndx) = offset;
                                    offset += (*count);
                                }

                                for (SKColor* color = colorsPtr; color < colorsEnd; ++color)
                                {
                                    int index = (*color).Red;
                                    int* bucketIndexPtr = currentBucketIndexPtr + index;
                                    SKColor* bucketPtr = bucketsPtr + (*bucketIndexPtr);
                                    ++(*bucketIndexPtr);
                                    *bucketPtr = *color;
                                }

                                buckets.CopyTo(colors);
                            }
                        }
                    }
                    finally
                    {
                        ArrayPool<SKColor>.Shared.Return(bucketsArray);
                    }
                }
            }
        }

        public static void SortGreen(in Span<SKColor> colors)
        {
            fixed (SKColor* colorsPtr = &colors.GetPinnableReference())
            {
                SKColor* colorsEnd = colorsPtr + colors.Length;

                Span<int> counts = stackalloc int[256];
                fixed (int* countsPtr = &counts.GetPinnableReference())
                {
                    int* countsEnd = countsPtr + counts.Length;

                    for (SKColor* color = colorsPtr; color < colorsEnd; ++color)
                        ++(*(countsPtr + (*color).Green));

                    SKColor[] bucketsArray = ArrayPool<SKColor>.Shared.Rent(colors.Length);

                    try
                    {
                        Span<SKColor> buckets = bucketsArray.AsSpan().Slice(0, colors.Length);
                        fixed (SKColor* bucketsPtr = &buckets.GetPinnableReference())
                        {
                            Span<int> currentBucketIndex = stackalloc int[256];
                            fixed (int* currentBucketIndexPtr = &currentBucketIndex.GetPinnableReference())
                            {
                                int offset = 0;
                                int* currentBucketIndx = currentBucketIndexPtr;
                                for (int* count = countsPtr; count < countsEnd; ++count, ++currentBucketIndx)
                                {
                                    (*currentBucketIndx) = offset;
                                    offset += (*count);
                                }

                                for (SKColor* color = colorsPtr; color < colorsEnd; ++color)
                                {
                                    int index = (*color).Green;
                                    int* bucketIndexPtr = currentBucketIndexPtr + index;
                                    SKColor* bucketPtr = bucketsPtr + (*bucketIndexPtr);
                                    ++(*bucketIndexPtr);
                                    *bucketPtr = *color;
                                }

                                buckets.CopyTo(colors);
                            }
                        }
                    }
                    finally
                    {
                        ArrayPool<SKColor>.Shared.Return(bucketsArray);
                    }
                }
            }
        }

        public static void SortBlue(in Span<SKColor> colors)
        {
            fixed (SKColor* colorsPtr = &colors.GetPinnableReference())
            {
                SKColor* colorsEnd = colorsPtr + colors.Length;

                Span<int> counts = stackalloc int[256];
                fixed (int* countsPtr = &counts.GetPinnableReference())
                {
                    int* countsEnd = countsPtr + counts.Length;

                    for (SKColor* color = colorsPtr; color < colorsEnd; ++color)
                        ++(*(countsPtr + (*color).Blue));

                    SKColor[] bucketsArray = ArrayPool<SKColor>.Shared.Rent(colors.Length);

                    try
                    {
                        Span<SKColor> buckets = bucketsArray.AsSpan().Slice(0, colors.Length);
                        fixed (SKColor* bucketsPtr = &buckets.GetPinnableReference())
                        {
                            Span<int> currentBucketIndex = stackalloc int[256];
                            fixed (int* currentBucketIndexPtr = &currentBucketIndex.GetPinnableReference())
                            {
                                int offset = 0;
                                int* currentBucketIndx = currentBucketIndexPtr;
                                for (int* count = countsPtr; count < countsEnd; ++count, ++currentBucketIndx)
                                {
                                    (*currentBucketIndx) = offset;
                                    offset += (*count);
                                }

                                for (SKColor* color = colorsPtr; color < colorsEnd; ++color)
                                {
                                    int index = (*color).Blue;
                                    int* bucketIndexPtr = currentBucketIndexPtr + index;
                                    SKColor* bucketPtr = bucketsPtr + (*bucketIndexPtr);
                                    ++(*bucketIndexPtr);
                                    *bucketPtr = *color;
                                }

                                buckets.CopyTo(colors);
                            }
                        }
                    }
                    finally
                    {
                        ArrayPool<SKColor>.Shared.Return(bucketsArray);
                    }
                }
            }
        }

        #endregion
    }
}
