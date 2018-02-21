using BenchmarkDotNet.Attributes;
using Benchmarks.Serializers.Helpers;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Benchmarks.Serializers
{
    public class Binary_FromStream<T>
    {
        private readonly T value;
        private readonly MemoryStream memoryStream;
        private readonly BinaryFormatter binaryFormatter;

        public Binary_FromStream()
        {
            value = DataGenerator.Generate<T>();

            // the stream is pre-allocated, we don't want the benchmarks to include stream allocaton cost
            memoryStream = new MemoryStream(capacity: short.MaxValue);
            binaryFormatter = new BinaryFormatter();

            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate)); // https://stackoverflow.com/a/7046868
        }

        [IterationSetup(Target = nameof(BinaryFormatter_))]
        public void SetupBinaryFormatter()
        {
            memoryStream.Position = 0;
            binaryFormatter.Serialize(memoryStream, value);
        }

        [IterationSetup(Target = nameof(ProtoBuffNet))]
        public void SetupProtoBuffNet()
        {
            memoryStream.Position = 0;
            ProtoBuf.Serializer.Serialize(memoryStream, value);
        }

        [IterationSetup(Target = nameof(ZeroFormatter_))]
        public void SetupZeroFormatter_()
        {
            memoryStream.Position = 0;
            ZeroFormatter.ZeroFormatterSerializer.Serialize<T>(memoryStream, value);
        }

        [Benchmark(Description = nameof(BinaryFormatter))]
        public T BinaryFormatter_()
        {
            memoryStream.Position = 0;
            return (T)binaryFormatter.Deserialize(memoryStream);
        }

        [Benchmark(Description = "protobuf-net")]
        public T ProtoBuffNet()
        {
            memoryStream.Position = 0;
            return ProtoBuf.Serializer.Deserialize<T>(memoryStream);
        }

        [Benchmark(Description = "ZeroFormatter")]
        public T ZeroFormatter_()
        {
            memoryStream.Position = 0;
            return ZeroFormatter.ZeroFormatterSerializer.Deserialize<T>(memoryStream);
        }
    }
}
