using System;
using System.IO;
using System.Numerics;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Running;
using ColorQuantizer.Optimized3;
using ColorQuantizer.Optimized4;
using ColorQuantizer.Opzimized2;
using SkiaSharp;

namespace ColorQuantizer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<QuantizerBenchmarks>(); 

            //Profile();
        }

        private static SKBitmap _bitmap;
        private static SKColor[] _colors;
        private static OptimizedColorQuantizer4 _optimizedColorQuantizer2;

        private static void Profile()
        {
            InitializeProfile();

            const int RUNS = 5;
            for (int i = 0; i < RUNS; i++)
            {
                Console.WriteLine($"{i + 1}/{RUNS}");
                ProfileRun();
            }
        }

        private static void InitializeProfile()
        {
            _optimizedColorQuantizer2 = new OptimizedColorQuantizer4();

            using FileStream stream = File.OpenRead(@"..\..\..\..\sample_data\splash\Aatrox_0.jpg");
            _bitmap = SKBitmap.Decode(stream);
            _colors = _bitmap.Pixels;
        }

        private static ColorSwatch ProfileRun()
        {
            SKColor[] skClrs = _optimizedColorQuantizer2.Quantize(_colors, 128);
            return _optimizedColorQuantizer2.FindAllColorVariations(skClrs, true);
        }
    }
}
