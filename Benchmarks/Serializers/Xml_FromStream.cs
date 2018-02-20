using BenchmarkDotNet.Attributes;
using System.IO;
using System.Xml.Serialization;

namespace Benchmarks.Serializers
{
    public class Xml_FromStream<T>
    {
        private readonly T value;
        private readonly XmlSerializer xmlSerializer;
        private readonly MemoryStream memoryStream;

        public Xml_FromStream()
        {
            value = DataGenerator.Generate<T>();
            xmlSerializer = new XmlSerializer(typeof(T));
            memoryStream = new MemoryStream(capacity: short.MaxValue);
        }

        [IterationSetup]
        public void Serialize()
        {
            memoryStream.Position = 0;
            xmlSerializer.Serialize(memoryStream, value);
        }

        [Benchmark]
        public T Deserialize()
        {
            memoryStream.Position = 0;
            return (T)xmlSerializer.Deserialize(memoryStream);
        }

        [GlobalCleanup]
        public void Cleanup() => memoryStream.Dispose();
    }
}
