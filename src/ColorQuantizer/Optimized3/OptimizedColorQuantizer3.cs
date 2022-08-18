using System;
using System.Collections.Generic;
using SkiaSharp;

namespace ColorQuantizer.Optimized3
{
    public class OptimizedColorQuantizer3
    {
        public SKColor[] Quantize(in Span<SKColor> colors, int amount)
        {
            if ((amount & (amount - 1)) != 0)
                throw new ArgumentException("Must be power of two", nameof(amount));
            
            Queue<ColorCube> cubes = new(amount);
            cubes.Enqueue(new ColorCube(colors, 0, colors.Length, SortTarget.None));

            while (cubes.Count < amount)
            {
                ColorCube cube = cubes.Dequeue();

                if (cube.TrySplit(colors, out ColorCube? a, out ColorCube? b))
                {
                    cubes.Enqueue(a);
                    cubes.Enqueue(b);
                }
            }

            SKColor[] result = new SKColor[cubes.Count];
            int i = 0;
            foreach (ColorCube colorCube in cubes)
                result[i++] = colorCube.GetAverageColor(colors);

            return result;
        }

        public ColorSwatch FindAllColorVariations(IEnumerable<SKColor> colors, bool ignoreLimits = false)
        {
            SKColor bestVibrantColor = SKColor.Empty;
            SKColor bestLightVibrantColor = SKColor.Empty;
            SKColor bestDarkVibrantColor = SKColor.Empty;
            SKColor bestMutedColor = SKColor.Empty;
            SKColor bestLightMutedColor = SKColor.Empty;
            SKColor bestDarkMutedColor = SKColor.Empty;
            float bestVibrantScore = float.MinValue;
            float bestLightVibrantScore = float.MinValue;
            float bestDarkVibrantScore = float.MinValue;
            float bestMutedScore = float.MinValue;
            float bestLightMutedScore = float.MinValue;
            float bestDarkMutedScore = float.MinValue;

            //ugly but at least we only loop through the enumerable once ¯\_(ツ)_/¯
            foreach (SKColor color in colors)
            {
                static void SetIfBetterScore(ref float bestScore, ref SKColor bestColor, SKColor newColor, ColorType type, bool ignoreLimits)
                {
                    float newScore = GetScore(newColor, type, ignoreLimits);
                    if (newScore > bestScore)
                    {
                        bestScore = newScore;
                        bestColor = newColor;
                    }
                }

                SetIfBetterScore(ref bestVibrantScore, ref bestVibrantColor, color, ColorType.Vibrant, ignoreLimits);
                SetIfBetterScore(ref bestLightVibrantScore, ref bestLightVibrantColor, color, ColorType.LightVibrant, ignoreLimits);
                SetIfBetterScore(ref bestDarkVibrantScore, ref bestDarkVibrantColor, color, ColorType.DarkVibrant, ignoreLimits);
                SetIfBetterScore(ref bestMutedScore, ref bestMutedColor, color, ColorType.Muted, ignoreLimits);
                SetIfBetterScore(ref bestLightMutedScore, ref bestLightMutedColor, color, ColorType.LightMuted, ignoreLimits);
                SetIfBetterScore(ref bestDarkMutedScore, ref bestDarkMutedColor, color, ColorType.DarkMuted, ignoreLimits);
            }

            return new ColorSwatch
            {
                Vibrant = bestVibrantColor,
                LightVibrant = bestLightVibrantColor,
                DarkVibrant = bestDarkVibrantColor,
                Muted = bestMutedColor,
                LightMuted = bestLightMutedColor,
                DarkMuted = bestDarkMutedColor,
            };
        }

        private static float GetScore(SKColor color, ColorType type, bool ignoreLimits = false)
        {
            static float InvertDiff(float value, float target)
            {
                return 1 - Math.Abs(value - target);
            }

            color.ToHsl(out float _, out float saturation, out float luma);
            saturation /= 100f;
            luma /= 100f;

            if (!ignoreLimits && ((saturation <= GetMinSaturation(type)) || (saturation >= GetMaxSaturation(type)) || (luma <= GetMinLuma(type)) || (luma >= GetMaxLuma(type))))
            {
                //if either saturation or luma falls outside the min-max, return the
                //lowest score possible unless we're ignoring these limits.
                return float.MinValue;
            }

            float totalValue = (InvertDiff(saturation, GetTargetSaturation(type)) * WEIGHT_SATURATION) + (InvertDiff(luma, GetTargetLuma(type)) * WEIGHT_LUMA);

            const float TOTAL_WEIGHT = WEIGHT_SATURATION + WEIGHT_LUMA;

            return totalValue / TOTAL_WEIGHT;
        }

        #region Constants

        private const float TARGET_DARK_LUMA = 0.26f;
        private const float MAX_DARK_LUMA = 0.45f;
        private const float MIN_LIGHT_LUMA = 0.55f;
        private const float TARGET_LIGHT_LUMA = 0.74f;
        private const float MIN_NORMAL_LUMA = 0.3f;
        private const float TARGET_NORMAL_LUMA = 0.5f;
        private const float MAX_NORMAL_LUMA = 0.7f;
        private const float TARGET_MUTES_SATURATION = 0.3f;
        private const float MAX_MUTES_SATURATION = 0.3f;
        private const float TARGET_VIBRANT_SATURATION = 1.0f;
        private const float MIN_VIBRANT_SATURATION = 0.35f;
        private const float WEIGHT_SATURATION = 3f;
        private const float WEIGHT_LUMA = 5f;

        private static float GetTargetLuma(ColorType colorType) => colorType switch
        {
            ColorType.Vibrant => TARGET_NORMAL_LUMA,
            ColorType.LightVibrant => TARGET_LIGHT_LUMA,
            ColorType.DarkVibrant => TARGET_DARK_LUMA,
            ColorType.Muted => TARGET_NORMAL_LUMA,
            ColorType.LightMuted => TARGET_LIGHT_LUMA,
            ColorType.DarkMuted => TARGET_DARK_LUMA,
            _ => throw new ArgumentException(nameof(colorType))
        };

        private static float GetMinLuma(ColorType colorType) => colorType switch
        {
            ColorType.Vibrant => MIN_NORMAL_LUMA,
            ColorType.LightVibrant => MIN_LIGHT_LUMA,
            ColorType.DarkVibrant => 0f,
            ColorType.Muted => MIN_NORMAL_LUMA,
            ColorType.LightMuted => MIN_LIGHT_LUMA,
            ColorType.DarkMuted => 0,
            _ => throw new ArgumentException(nameof(colorType))
        };

        private static float GetMaxLuma(ColorType colorType) => colorType switch
        {
            ColorType.Vibrant => MAX_NORMAL_LUMA,
            ColorType.LightVibrant => 1f,
            ColorType.DarkVibrant => MAX_DARK_LUMA,
            ColorType.Muted => MAX_NORMAL_LUMA,
            ColorType.LightMuted => 1f,
            ColorType.DarkMuted => MAX_DARK_LUMA,
            _ => throw new ArgumentException(nameof(colorType))
        };

        private static float GetTargetSaturation(ColorType colorType) => colorType switch
        {
            ColorType.Vibrant => TARGET_VIBRANT_SATURATION,
            ColorType.LightVibrant => TARGET_VIBRANT_SATURATION,
            ColorType.DarkVibrant => TARGET_VIBRANT_SATURATION,
            ColorType.Muted => TARGET_MUTES_SATURATION,
            ColorType.LightMuted => TARGET_MUTES_SATURATION,
            ColorType.DarkMuted => TARGET_MUTES_SATURATION,
            _ => throw new ArgumentException(nameof(colorType))
        };

        private static float GetMinSaturation(ColorType colorType) => colorType switch
        {
            ColorType.Vibrant => MIN_VIBRANT_SATURATION,
            ColorType.LightVibrant => MIN_VIBRANT_SATURATION,
            ColorType.DarkVibrant => MIN_VIBRANT_SATURATION,
            ColorType.Muted => 0,
            ColorType.LightMuted => 0,
            ColorType.DarkMuted => 0,
            _ => throw new ArgumentException(nameof(colorType))
        };

        private static float GetMaxSaturation(ColorType colorType) => colorType switch
        {
            ColorType.Vibrant => 1f,
            ColorType.LightVibrant => 1f,
            ColorType.DarkVibrant => 1f,
            ColorType.Muted => MAX_MUTES_SATURATION,
            ColorType.LightMuted => MAX_MUTES_SATURATION,
            ColorType.DarkMuted => MAX_MUTES_SATURATION,
            _ => throw new ArgumentException(nameof(colorType))
        };

        #endregion
    }
}
