namespace ColorQuantizer.Shared
{
    public struct Color
    {
        public byte Red;
        public byte Green;
        public byte Blue;

        public Color(byte r, byte g, byte b)
        {
            this.Red = r;
            this.Green = g;
            this.Blue = b;
        }
    }
}
