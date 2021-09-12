using SkiaSharp;
using System;

namespace ColorQuantizer
{
    public struct ColorSwatch : IEquatable<ColorSwatch>
    {
        /// <summary>
        /// The <see cref="ColorType.Vibrant"/> component.
        /// </summary>
        public SKColor Vibrant { get; init; }

        /// <summary>
        /// The <see cref="ColorType.LightVibrant"/> component.
        /// </summary>
        public SKColor LightVibrant { get; init; }

        /// <summary>
        /// The <see cref="ColorType.DarkVibrant"/> component.
        /// </summary>
        public SKColor DarkVibrant { get; init; }

        /// <summary>
        /// The <see cref="ColorType.Muted"/> component.
        /// </summary>
        public SKColor Muted { get; init; }

        /// <summary>
        /// The <see cref="ColorType.LightMuted"/> component.
        /// </summary>
        public SKColor LightMuted { get; init; }

        /// <summary>
        /// The <see cref="ColorType.DarkMuted"/> component.
        /// </summary>
        public SKColor DarkMuted { get; init; }

        public override bool Equals(object obj) => obj is ColorSwatch other && this.Equals(other);

        public bool Equals(ColorSwatch p) => Vibrant == p.Vibrant &&
                                            LightVibrant == p.LightVibrant &&
                                            DarkVibrant == p.DarkVibrant &&
                                            Muted == p.Muted &&
                                            LightMuted == p.LightMuted &&
                                            DarkMuted == p.DarkMuted;

        public override int GetHashCode() => (Vibrant, LightVibrant, DarkVibrant, Muted, LightMuted, DarkMuted).GetHashCode();

        public static bool operator ==(ColorSwatch lhs, ColorSwatch rhs) => lhs.Equals(rhs);

        public static bool operator !=(ColorSwatch lhs, ColorSwatch rhs) => !(lhs == rhs);

        public override string ToString()
        {
            return $"Vibrant {Vibrant} LightVibrant {LightVibrant} DarkVibrant {DarkVibrant} Muted {Muted} LightMuted {LightMuted} DarkMuted {DarkMuted}";
        }
    }
}
