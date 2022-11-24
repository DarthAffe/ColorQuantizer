// ReSharper disable NotAccessedField.Global - This has to look exactly like the SKColor is stored!

using System.Runtime.InteropServices;

namespace ColorQuantizer.Optimized11;

internal readonly struct QuantizerColor
{
    public readonly byte Blue;
    public readonly byte Green;
    public readonly byte Red;
    public readonly byte Alpha;


    //private readonly uint color;
    //public byte Alpha => (byte)(this.color >> 24 & (uint)byte.MaxValue);
    //public byte Red => (byte)(this.color >> 16 & (uint)byte.MaxValue);
    //public byte Green => (byte)(this.color >> 8 & (uint)byte.MaxValue);
    //public byte Blue => (byte)(this.color & (uint)byte.MaxValue);
}