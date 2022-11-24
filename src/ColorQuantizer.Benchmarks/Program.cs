using System;
using System.IO;
using System.Numerics;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Running;
using ColorQuantizer.Optimized11;
using ColorQuantizer.Optimized3;
using ColorQuantizer.Optimized4;
using ColorQuantizer.Optimized8;
using ColorQuantizer.Opzimized2;
using SkiaSharp;

namespace ColorQuantizer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<ByteAccessBenchmarks>(); 
            BenchmarkRunner.Run<QuantizerBenchmarks>();

            //Profile();
        }

        private static SKBitmap _bitmap;
        private static SKColor[] _colors;

        private static void Profile()
        {
            InitializeProfile();

            const int RUNS = 20;
            for (int i = 0; i < RUNS; i++)
            {
                Console.WriteLine($"{i + 1}/{RUNS}");
                ProfileRun();
            }
        }

        private static void InitializeProfile()
        {
            using FileStream stream = File.OpenRead(@"..\..\..\..\sample_data\splash\Aatrox_0.jpg");
            _bitmap = SKBitmap.Decode(stream);
            _colors = _bitmap.Pixels;
        }

        private static ColorSwatch ProfileRun()
        {
            SKColor[] skClrs = OptimizedColorQuantizer11.Quantize(_colors, 128);
            return OptimizedColorQuantizer11.FindAllColorVariations(skClrs, true);
        }
    }
}
