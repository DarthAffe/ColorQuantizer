// Based on https://github.com/colgreen/Redzen/blob/78bf423c3512fa4e613dfa52c6f3b8c82708d22b/Redzen/Sorting/TimSort.cs

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SkiaSharp;

namespace ColorQuantizer.Sorting
{
    internal static class QuantizerTimsortBlue
    {
        #region Constants

        private const int MIN_MERGE = 32;
        private const int MIN_GALLOP = 7;

        #endregion

        #region Methods

        public static void Sort(in Span<SKColor> span, in SKColor[] tmpDataStorage)
        {
            if (span.Length < 2) return;

            int lo = 0;
            int hi = span.Length;

            if (span.Length < MIN_MERGE)
            {
                int initRunLen = CountRunAndMakeAscending(span[lo..hi]);
                InsertionSort(span, initRunLen);
                return;
            }

            int len = span.Length;
            int stackLen = (len < 120 ? 4 : len < 1542 ? 9 : len < 119151 ? 18 : 39);

            int stackSize = 0;
            int minGallop = MIN_GALLOP;

            Span<int> runBaseData = stackalloc int[stackLen];
            Span<int> runLenData = stackalloc int[stackLen];
            
            int minRun = MinRunLength(span.Length, MIN_MERGE);
            int nRemaining = span.Length;
            do
            {
                int runLen = CountRunAndMakeAscending(span[lo..hi]);

                if (runLen < minRun)
                {
                    int force = nRemaining <= minRun ? nRemaining : minRun;
                    InsertionSort(span.Slice(lo, force), runLen);
                    runLen = force;
                }

                PushRun(lo, runLen, runBaseData, runLenData, ref stackSize);
                MergeCollapse(span, runBaseData, runLenData, ref stackSize, ref minGallop, tmpDataStorage);

                lo += runLen;
                nRemaining -= runLen;
            }
            while (nRemaining != 0);

            MergeForceCollapse(span, runBaseData, runLenData, ref stackSize, ref minGallop, tmpDataStorage);
        }

        private static void PushRun(int runBase, int runLen, in Span<int> runBaseData, in Span<int> runLenData, ref int stackSize)
        {
            runBaseData[stackSize] = runBase;
            runLenData[stackSize] = runLen;
            stackSize++;
        }

        private static void MergeCollapse(in Span<SKColor> s, in Span<int> runBaseData, in Span<int> runLenData, ref int stackSize, ref int minGallop, in Span<SKColor> tmpDataStorage)
        {
            while (stackSize > 1)
            {
                int n = stackSize - 2;
                int runLenN = runLenData[n];
                int runLenNPlus1 = runLenData[n + 1];

                if (((n >= 1) && (runLenData[n - 1] <= (runLenN + runLenNPlus1)))
                 || ((n >= 2) && (runLenData[n - 2] <= (runLenN + runLenData[n - 1]))))
                {
                    if (runLenData[n - 1] < runLenNPlus1)
                        n--;
                }
                else if (runLenN > runLenNPlus1)
                    break;

                MergeAt(s, n, runBaseData, runLenData, ref stackSize, ref minGallop, tmpDataStorage);
            }
        }

        private static void MergeForceCollapse(in Span<SKColor> s, in Span<int> runBaseData, in Span<int> runLenData, ref int stackSize, ref int minGallop, in Span<SKColor> tmpDataStorage)
        {
            while (stackSize > 1)
            {
                int n = stackSize - 2;
                if ((n > 0) && (runLenData[n - 1] < runLenData[n + 1]))
                    n--;

                MergeAt(s, n, runBaseData, runLenData, ref stackSize, ref minGallop, tmpDataStorage);
            }
        }

        private static void MergeAt(in Span<SKColor> s, int i, in Span<int> runBaseData, in Span<int> runLenData, ref int stackSize, ref int minGallop, in Span<SKColor> tmpDataStorage)
        {
            int base1 = runBaseData[i];
            int len1 = runLenData[i];
            int base2 = runBaseData[i + 1];
            int len2 = runLenData[i + 1];

            runLenData[i] = len1 + len2;
            if (i == (stackSize - 3))
            {
                runBaseData[i + 1] = runBaseData[i + 2];
                runLenData[i + 1] = runLenData[i + 2];
            }
            stackSize--;

            int k = GallopRight(s[base2], s, base1, len1, 0);
            base1 += k;
            len1 -= k;
            if (len1 == 0) return;

            len2 = GallopLeft(s[(base1 + len1) - 1], s, base2, len2, len2 - 1);
            if (len2 == 0) return;

            if (len1 <= len2)
                MergeLo(s, base1, len1, base2, len2, ref minGallop, tmpDataStorage);
            else
                MergeHi(s, base1, len1, base2, len2, ref minGallop, tmpDataStorage);
        }

        private static void MergeLo(in Span<SKColor> s, int base1, int len1, int base2, int len2, ref int minGallop, in Span<SKColor> tmpDataStorage)
        {
            Span<SKColor> tmp = EnsureCapacity(len1, s.Length, tmpDataStorage);

            int cursor1 = 0;
            int cursor2 = base2;
            int dest = base1;
            s.Slice(base1, len1).CopyTo(tmp.Slice(cursor1));

            s[dest++] = s[cursor2++];
            if (--len2 == 0)
            {
                tmp.Slice(cursor1, len1).CopyTo(s.Slice(dest));
                return;
            }

            if (len1 == 1)
            {
                s.Slice(cursor2, len2).CopyTo(s.Slice(dest));
                s[dest + len2] = tmp[cursor1];
                return;
            }

            while (true)
            {
                int count1 = 0;
                int count2 = 0;

                do
                {
                    if (LessThan(s[cursor2], tmp[cursor1]))
                    {
                        s[dest++] = s[cursor2++];
                        count2++;
                        count1 = 0;
                        if (--len2 == 0)
                            goto outerExit;
                    }
                    else
                    {
                        s[dest++] = tmp[cursor1++];
                        count1++;
                        count2 = 0;
                        if (--len1 == 1)
                            goto outerExit;
                    }
                }
                while ((count1 | count2) < minGallop);

                do
                {
                    count1 = GallopRight(s[cursor2], tmp, cursor1, len1, 0);
                    if (count1 != 0)
                    {
                        tmp.Slice(cursor1, count1).CopyTo(s.Slice(dest));
                        dest += count1;
                        cursor1 += count1;
                        len1 -= count1;
                        if (len1 <= 1)
                            goto outerExit;
                    }
                    s[dest++] = s[cursor2++];
                    if (--len2 == 0)
                        goto outerExit;

                    count2 = GallopLeft(tmp[cursor1], s, cursor2, len2, 0);
                    if (count2 != 0)
                    {
                        s.Slice(cursor2, count2).CopyTo(s.Slice(dest));
                        dest += count2;
                        cursor2 += count2;
                        len2 -= count2;
                        if (len2 == 0)
                            goto outerExit;
                    }
                    s[dest++] = tmp[cursor1++];
                    if (--len1 == 1)
                        goto outerExit;

                    minGallop--;
                }
                while ((count1 >= MIN_GALLOP) | (count2 >= MIN_GALLOP));

                if (minGallop < 0)
                    minGallop = 0;

                minGallop += 2;
            }
            outerExit:

            if (minGallop < 1)
                minGallop = 1;

            if (len1 == 1)
            {
                s.Slice(cursor2, len2).CopyTo(s.Slice(dest));
                s[dest + len2] = tmp[cursor1];
            }
            else if (len1 == 0)
                throw new ArgumentException("Comparison method violates its general contract!");
            else
                tmp.Slice(cursor1, len1).CopyTo(s.Slice(dest));
        }

        private static void MergeHi(in Span<SKColor> s, int base1, int len1, int base2, int len2, ref int minGallop, in Span<SKColor> tmpDataStorage)
        {
            Span<SKColor> tmp = EnsureCapacity(len2, s.Length, tmpDataStorage);
            s.Slice(base2, len2).CopyTo(tmp);

            int cursor1 = (base1 + len1) - 1;
            int cursor2 = len2 - 1;
            int dest = (base2 + len2) - 1;

            s[dest--] = s[cursor1--];
            if (--len1 == 0)
            {
                tmp.Slice(0, len2).CopyTo(s.Slice(dest - (len2 - 1)));
                return;
            }

            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                s.Slice(cursor1 + 1, len1).CopyTo(s.Slice(dest + 1));
                s[dest] = tmp[cursor2];
                return;
            }
            
            while (true)
            {
                int count1 = 0;
                int count2 = 0;

                do
                {
                    if (LessThan(tmp[cursor2], s[cursor1]))
                    {
                        s[dest--] = s[cursor1--];
                        count1++;
                        count2 = 0;
                        if (--len1 == 0)
                            goto outerExit;
                    }
                    else
                    {
                        s[dest--] = tmp[cursor2--];
                        count2++;
                        count1 = 0;
                        if (--len2 == 1)
                            goto outerExit;
                    }
                }
                while ((count1 | count2) < minGallop);

                do
                {
                    count1 = len1 - GallopRight(tmp[cursor2], s, base1, len1, len1 - 1);
                    if (count1 != 0)
                    {
                        dest -= count1;
                        cursor1 -= count1;
                        len1 -= count1;
                        s.Slice(cursor1 + 1, count1).CopyTo(s.Slice(dest + 1));
                        if (len1 == 0)
                            goto outerExit;
                    }
                    s[dest--] = tmp[cursor2--];
                    if (--len2 == 1)
                        goto outerExit;

                    count2 = len2 - GallopLeft(s[cursor1], tmp, 0, len2, len2 - 1);
                    if (count2 != 0)
                    {
                        dest -= count2;
                        cursor2 -= count2;
                        len2 -= count2;
                        tmp.Slice(cursor2 + 1, count2).CopyTo(s.Slice(dest + 1));
                        if (len2 <= 1)
                            goto outerExit;
                    }
                    s[dest--] = s[cursor1--];
                    if (--len1 == 0)
                        goto outerExit;

                    minGallop--;
                }
                while ((count1 >= MIN_GALLOP) | (count2 >= MIN_GALLOP));

                if (minGallop < 0)
                    minGallop = 0;

                minGallop += 2;
            }
            outerExit:

            if (minGallop < 1)
                minGallop = 1;

            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                s.Slice(cursor1 + 1, len1).CopyTo(s.Slice(dest + 1));
                s[dest] = tmp[cursor2];
            }
            else if (len2 == 0)
                throw new ArgumentException("Comparison method violates its general contract!");
            else
                tmp.Slice(0, len2).CopyTo(s.Slice(dest - (len2 - 1)));
        }

        private static Span<SKColor> EnsureCapacity(int minCapacity, int spanLen, Span<SKColor> tmpDataStorage)
        {
            if (tmpDataStorage.Length < minCapacity)
            {
                int newSize = 1 << (32 - BitOperations.LeadingZeroCount((uint)minCapacity));

                if (newSize < 0)
                    newSize = minCapacity;
                else
                    newSize = Math.Min(newSize, spanLen >> 1);

                SKColor[] newArray = new SKColor[newSize];
                tmpDataStorage = new Span<SKColor>(newArray);
            }
            return tmpDataStorage;
        }

        private static void InsertionSort(in Span<SKColor> s, int start)
        {
            if (start > 0) start--;

            for (int i = start; i < (s.Length - 1); i++)
            {
                SKColor k = s[i + 1];

                int j = i;
                while ((j >= 0) && LessThan(k, s[j]))
                {
                    s[j + 1] = s[j];
                    j--;
                }

                s[j + 1] = k;
            }
        }

        private static int CountRunAndMakeAscending(in Span<SKColor> s)
        {
            int runHi = 1;
            if (runHi == s.Length) return 1;

            if (LessThan(s[runHi++], s[0]))
            {
                while ((runHi < s.Length) && LessThan(s[runHi], s[runHi - 1]))
                    runHi++;

                s.Slice(0, runHi).Reverse();
            }
            else
            {
                while ((runHi < s.Length) && !LessThan(s[runHi], s[runHi - 1]))
                    runHi++;
            }

            return runHi;
        }

        public static int MinRunLength(int n, int minMerge)
        {
            int r = 0;
            while (n >= minMerge)
            {
                r |= (n & 1);
                n >>= 1;
            }
            return n + r;
        }

        private static int GallopLeft(SKColor key, Span<SKColor> s, int baseIdx, int len, int hint)
        {
            int lastOfs = 0;
            int ofs = 1;
            if (LessThan(s[baseIdx + hint], key))
            {
                int maxOfs = len - hint;
                while ((ofs < maxOfs) && LessThan(s[baseIdx + hint + ofs], key))
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                lastOfs += hint;
                ofs += hint;
            }
            else
            {
                int maxOfs = hint + 1;
                while ((ofs < maxOfs) && !LessThan(s[(baseIdx + hint) - ofs], key))
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }

            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (LessThan(s[baseIdx + m], key))
                    lastOfs = m + 1;
                else
                    ofs = m;
            }

            return ofs;
        }

        private static int GallopRight(SKColor key, Span<SKColor> s, int baseIdx, int len, int hint)
        {
            int ofs = 1;
            int lastOfs = 0;
            if (LessThan(key, s[baseIdx + hint]))
            {
                int maxOfs = hint + 1;
                while ((ofs < maxOfs) && LessThan(key, s[(baseIdx + hint) - ofs]))
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            else
            {
                int maxOfs = len - hint;
                while ((ofs < maxOfs) && !LessThan(key, s[baseIdx + hint + ofs]))
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                lastOfs += hint;
                ofs += hint;
            }

            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (LessThan(key, s[baseIdx + m]))
                    ofs = m;
                else
                    lastOfs = m + 1;
            }

            return ofs;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool LessThan(in SKColor color1, in SKColor color2) => color1.Blue < color2.Blue;

        #endregion
    }
}
