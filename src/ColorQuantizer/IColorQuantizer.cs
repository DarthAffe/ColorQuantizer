using SkiaSharp;
using System;
using System.Collections.Generic;

namespace ColorQuantizer
{
    /// <summary>
    /// A service providing a pallette of colors in a bitmap based on vibrant.js
    /// </summary>
    public interface IColorQuantizer
    {
        /// <summary>
        /// Reduces an <see cref="IEnumerable{SKColor}"/> to a given amount of relevant colors. Based on the Median Cut algorithm
        /// </summary>
        /// <param name="colors">The colors to quantize.</param>
        /// <param name="amount">The number of colors that should be calculated. Must be a power of two.</param>
        /// <returns>The quantized colors.</returns>
        public SKColor[] Quantize(SKColor[] colors, int amount);

        /// <summary>
        /// Finds all the color variations available and returns a struct containing them all.
        /// </summary>
        /// <param name="colors">The colors to find the variations in</param>
        /// <param name="ignoreLimits">Ignore hard limits on whether a color is considered for each category. Some colors may be <see cref="SKColor.Empty"/> if this is false</param>
        /// <returns>A swatch containing all color variations</returns>
        public ColorSwatch FindAllColorVariations(SKColor[] colors, bool ignoreLimits = false);
    }
}
