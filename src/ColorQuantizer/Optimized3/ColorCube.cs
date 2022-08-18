using System;
using System.Diagnostics.CodeAnalysis;
using SkiaSharp;

namespace ColorQuantizer.Optimized3
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

    internal class ColorCube
    {
        private readonly int _from;
        private readonly int _length;
        private SortTarget _currentOrder = SortTarget.None;

        public ColorCube(in Span<SKColor> fullColorList, int from, int length, SortTarget preOrdered)
        {
            this._from = from;
            this._length = length;

            OrderColors(fullColorList.Slice(from, length), preOrdered);
        }

        private void OrderColors(in Span<SKColor> colors, SortTarget preOrdered)
        {
            if (colors.Length < 2) return;
            ColorRanges colorRanges = GetColorRanges(colors);

            if ((colorRanges.RedRange > colorRanges.GreenRange) && (colorRanges.RedRange > colorRanges.BlueRange))
            {
                if (preOrdered != SortTarget.Red)
                    RadixLikeSortRed.Sort(colors);

                _currentOrder = SortTarget.Red;
            }
            else if (colorRanges.GreenRange > colorRanges.BlueRange)
            {
                if (preOrdered != SortTarget.Green)
                    RadixLikeSortGreen.Sort(colors);

                _currentOrder = SortTarget.Green;
            }
            else
            {
                if (preOrdered != SortTarget.Blue)
                    RadixLikeSortBlue.Sort(colors);

                _currentOrder = SortTarget.Blue;
            }
        }

        private ColorRanges GetColorRanges(in Span<SKColor> colors)
        {
            if (colors.Length < 512)
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
            else
            {
                Span<bool> redBuckets = stackalloc bool[256];
                Span<bool> greenBuckets = stackalloc bool[256];
                Span<bool> blueBuckets = stackalloc bool[256];

                for (int i = 0; i < colors.Length; i++)
                {
                    SKColor color = colors[i];
                    redBuckets[color.Red] = true;
                    greenBuckets[color.Green] = true;
                    blueBuckets[color.Blue] = true;
                }

                byte redMin = 0;
                byte redMax = 0;
                byte greenMin = 0;
                byte greenMax = 0;
                byte blueMin = 0;
                byte blueMax = 0;

                for (byte i = 0; i < redBuckets.Length; i++)
                    if (redBuckets[i])
                    {
                        redMin = i;
                        break;
                    }

                for (int i = redBuckets.Length - 1; i >= 0; i--)
                    if (redBuckets[i])
                    {
                        redMax = (byte)i;
                        break;
                    }

                for (byte i = 0; i < greenBuckets.Length; i++)
                    if (greenBuckets[i])
                    {
                        greenMin = i;
                        break;
                    }

                for (int i = greenBuckets.Length - 1; i >= 0; i--)
                    if (greenBuckets[i])
                    {
                        greenMax = (byte)i;
                        break;
                    }

                for (byte i = 0; i < blueBuckets.Length; i++)
                    if (blueBuckets[i])
                    {
                        blueMin = i;
                        break;
                    }

                for (int i = blueBuckets.Length - 1; i >= 0; i--)
                    if (blueBuckets[i])
                    {
                        blueMax = (byte)i;
                        break;
                    }

                return new ColorRanges((byte)(redMax - redMin), (byte)(greenMax - greenMin), (byte)(blueMax - blueMin));
            }
        }

        internal bool TrySplit(in Span<SKColor> fullColorList, [NotNullWhen(returnValue: true)] out ColorCube? a, [NotNullWhen(returnValue: true)] out ColorCube? b)
        {
            Span<SKColor> colors = fullColorList.Slice(_from, _length);

            if (colors.Length < 2)
            {
                a = null;
                b = null;
                return false;
            }

            int median = colors.Length / 2;

            a = new ColorCube(fullColorList, _from, median, _currentOrder);
            b = new ColorCube(fullColorList, _from + median, colors.Length - median, _currentOrder);

            return true;
        }

        internal SKColor GetAverageColor(in Span<SKColor> fullColorList)
        {
            Span<SKColor> colors = fullColorList.Slice(_from, _length);

            int r = 0, g = 0, b = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                SKColor color = colors[i];
                r += color.Red;
                g += color.Green;
                b += color.Blue;
            }

            return new SKColor(
                (byte)(r / colors.Length),
                (byte)(g / colors.Length),
                (byte)(b / colors.Length)
            );
        }
    }
}
