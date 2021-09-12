using BenchmarkDotNet.Running;
using System;

namespace ColorQuantizer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<QuantizerBenchmarks>();
        }
    }
}
