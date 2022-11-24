using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

namespace ColorQuantizer.Benchmarks;

[SimpleJob(RuntimeMoniker.Net70)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class ByteAccessBenchmarks
{
    private const int SIZE = 1024 * 1024;
    private const int SIZE_BYTES = SIZE * 4;

    private readonly byte[] _data;
    private readonly ByteStruct[] _byteData;
    private readonly IntStruct[] _intData;

    public ByteAccessBenchmarks()
    {
        _data = new byte[SIZE_BYTES];

        Random random = new(1);
        random.NextBytes(_data);

        _byteData = new ByteStruct[SIZE];
        _intData = new IntStruct[SIZE];
        Span<byte> byteDataStruct = MemoryMarshal.Cast<ByteStruct, byte>(_byteData.AsSpan());
        Span<byte> intDataStruct = MemoryMarshal.Cast<IntStruct, byte>(_intData.AsSpan());

        _data.AsSpan().CopyTo(byteDataStruct);
        _data.AsSpan().CopyTo(intDataStruct);
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Marshal")]
    public int MarshalByteStruct()
    {
        Span<ByteStruct> byteDataStruct = MemoryMarshal.Cast<byte, ByteStruct>(_data.AsSpan());
        return byteDataStruct.Length;
    }

    [Benchmark]
    [BenchmarkCategory("Marshal")]
    public int MarshalIntStruct()
    {
        Span<IntStruct> intDataStruct = MemoryMarshal.Cast<byte, IntStruct>(_data.AsSpan());
        return intDataStruct.Length;
    }

    [Benchmark]
    [BenchmarkCategory("Marshal")]
    public unsafe int MarshalByteStructUnsafe()
    {
        fixed (void* colorPtr = &_data.AsSpan().GetPinnableReference())
        {
            Span<ByteStruct> byteDataStruct = new(colorPtr, SIZE);
            return byteDataStruct.Length;
        }
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Marshal with access")]
    public int MarshalByteStructWithAccess()
    {
        Span<ByteStruct> byteDataStruct = MemoryMarshal.Cast<byte, ByteStruct>(_data.AsSpan());
        return byteDataStruct[1000].R;
    }

    [Benchmark]
    [BenchmarkCategory("Marshal with access")]
    public int MarshalIntStructWithAccess()
    {
        Span<IntStruct> intDataStruct = MemoryMarshal.Cast<byte, IntStruct>(_data.AsSpan());
        return intDataStruct[1000].R;
    }

    [Benchmark]
    [BenchmarkCategory("Marshal with access")]
    public unsafe int MarshalByteStructWithAccessUnsafe()
    {
        fixed (void* colorPtr = &_data.AsSpan().GetPinnableReference())
        {
            Span<ByteStruct> byteDataStruct = new(colorPtr, SIZE);
            return byteDataStruct[1000].R;
        }
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Sum wo Marshal")]
    public int SumByteStruct()
    {
        int sum = 0;
        foreach (ByteStruct d in _byteData)
            sum += d.R;

        return sum;
    }

    [Benchmark]
    [BenchmarkCategory("Sum wo Marshal")]
    public int SumIntStruct()
    {
        int sum = 0;
        foreach (IntStruct d in _intData)
            sum += d.R;

        return sum;
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Sum with Marshal")]
    public int SumByteStructMarshal()
    {
        Span<ByteStruct> data = MemoryMarshal.Cast<byte, ByteStruct>(_data.AsSpan());

        int sum = 0;
        foreach (ByteStruct d in data)
            sum += d.R;

        return sum;
    }

    [Benchmark]
    [BenchmarkCategory("Sum with Marshal")]
    public int SumIntStructMarshal()
    {
        Span<IntStruct> data = MemoryMarshal.Cast<byte, IntStruct>(_data.AsSpan());

        int sum = 0;
        foreach (IntStruct d in data)
            sum += d.R;

        return sum;
    }

    [Benchmark]
    [BenchmarkCategory("Sum with Marshal")]
    public unsafe int SumByteStructMarshalUnsafe()
    {
        fixed (void* colorPtr = &_data.AsSpan().GetPinnableReference())
        {
            Span<ByteStruct> data = new(colorPtr, SIZE);

            int sum = 0;
            foreach (ByteStruct d in data)
                sum += d.R;

            return sum;
        }
    }

    public readonly struct ByteStruct
    {
        public readonly byte B;
        public readonly byte G;
        public readonly byte R;
        public readonly byte A;
    }

    public readonly struct IntStruct
    {
        private readonly uint _color;
        
        public byte A => (byte)((_color >> 24) & (uint)byte.MaxValue);
        public byte R => (byte)((_color >> 16) & (uint)byte.MaxValue);
        public byte G => (byte)((_color >> 8) & (uint)byte.MaxValue);
        public byte B => (byte)(_color & (uint)byte.MaxValue);
    }
}