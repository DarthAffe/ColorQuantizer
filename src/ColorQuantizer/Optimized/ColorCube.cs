using SkiaSharp;
using System;

namespace ColorQuantizer.Optimized
{
    internal struct ColorCube
    {
        private readonly Memory<SKColor> _colors;

        internal ColorCube(Memory<SKColor> colors)
        {
            _colors = colors;

            var span = _colors.Span;

            byte minRed = byte.MaxValue, minGreen = byte.MaxValue, minBlue = byte.MaxValue;
            byte maxRed = byte.MinValue, maxGreen = byte.MinValue, maxBlue = byte.MinValue;

            for (int i = 0; i < span.Length; i++)
            {
                minRed = Math.Min(minRed, span[i].Red);
                maxRed = Math.Max(maxRed, span[i].Red);

                minGreen = Math.Min(minGreen, span[i].Green);
                maxGreen = Math.Max(maxGreen, span[i].Green);

                minBlue = Math.Min(minBlue, span[i].Blue);
                maxBlue = Math.Max(maxBlue, span[i].Blue);
            }

            int redRange = maxRed - minRed;
            int greenRange = maxGreen - minGreen;
            int blueRange = maxBlue - minBlue;

            //if (redRange > greenRange && redRange > blueRange)
            //    _colors = arr.OrderBy(a => a.Red).ToArray();
            //else if (greenRange > blueRange)
            //    _colors = arr.OrderBy(a => a.Green).ToArray();
            //else
            //    _colors = arr.OrderBy(a => a.Blue).ToArray();

            if (redRange > greenRange && redRange > blueRange)
                span.Sort((a, b) => a.Red.CompareTo(b.Red));
            else if (greenRange > blueRange)
                span.Sort((a, b) => a.Green.CompareTo(b.Green));
            else
                span.Sort((a, b) => a.Blue.CompareTo(b.Blue));
        }

        internal bool TrySplit(out ColorCube a, out ColorCube b)
        {
            if (_colors.Length < 2)
            {
                a = default;
                b = default;
                return false;
            }

            int median = _colors.Length / 2;

            a = new ColorCube(_colors[..median]);
            b = new ColorCube(_colors[median..]);

            return true;
        }

        internal SKColor GetAverageColor()
        {
            int r = 0, g = 0, b = 0;
            var span = _colors.Span;

            for (int i = 0; i < _colors.Length; i++)
            {
                r += span[i].Red;
                g += span[i].Green;
                b += span[i].Blue;
            }

            return new SKColor(
                (byte)(r / _colors.Length),
                (byte)(g / _colors.Length),
                (byte)(b / _colors.Length)
            );
        }
    }
}
