﻿using BenchmarkDotNet.Attributes;
using Benchmarks.Serializers.Helpers;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Benchmarks.Serializers
{
    public class Binary_ToStream<T>
    {
        private readonly T value;
        private readonly MemoryStream memoryStream;
        private readonly BinaryFormatter binaryFormatter;

        public Binary_ToStream()
        {
            value = DataGenerator.Generate<T>();

            // the stream is pre-allocated, we don't want the benchmarks to include stream allocaton cost
            memoryStream = new MemoryStream(capacity: short.MaxValue);
            binaryFormatter = new BinaryFormatter();

            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate)); // https://stackoverflow.com/a/7046868
        }

        [Benchmark(Description = nameof(BinaryFormatter))]
        public void BinaryFormatter_()
        {
            memoryStream.Position = 0;
            binaryFormatter.Serialize(memoryStream, value);
        }

        [Benchmark(Description = "protobuf-net")]
        public void ProtoBuffNet()
        {
            memoryStream.Position = 0;
            ProtoBuf.Serializer.Serialize(memoryStream, value);
        }

        [Benchmark(Description = "ZeroFormatter")]
        public void ZeroFormatter_()
        {
            memoryStream.Position = 0;
            ZeroFormatter.ZeroFormatterSerializer.Serialize<T>(memoryStream, value);
        }
    }
}
