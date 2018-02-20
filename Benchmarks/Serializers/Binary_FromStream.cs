using BenchmarkDotNet.Attributes;
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
        }

        [IterationSetup(Target = nameof(BinaryFormatter_))]
        public void SetupBinaryFormatter_()
        {
            memoryStream.Position = 0;
            binaryFormatter.Serialize(memoryStream, value);
        }

        [Benchmark(Description = nameof(BinaryFormatter))]
        public T BinaryFormatter_()
        {
            memoryStream.Position = 0;
            return (T)binaryFormatter.Deserialize(memoryStream);
        }
    }
}
