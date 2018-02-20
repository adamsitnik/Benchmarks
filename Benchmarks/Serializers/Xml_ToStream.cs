using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Benchmarks.Serializers
{
    //[Config(typeof(SgenConfig))] https://github.com/dotnet/corefx/issues/27281 it does not work when project targets multiple frameworks
    public class Xml_ToStream<T>
    {
        private readonly T value;
        private readonly XmlSerializer xmlSerializer;
        private readonly DataContractSerializer dataContractSerializer;
        private readonly MemoryStream memoryStream;

        public Xml_ToStream()
        {
            value = DataGenerator.Generate<T>();
            xmlSerializer = new XmlSerializer(typeof(T));
            dataContractSerializer = new DataContractSerializer(typeof(T));
            memoryStream = new MemoryStream(capacity: short.MaxValue);

#if SGEN
            // we need to give some hints to the SGEN tool ;)
            if (typeof(T) == typeof(LoginViewModel))
                xmlSerializer = new XmlSerializer(typeof(LoginViewModel));
            if (typeof(T) == typeof(Location))
                xmlSerializer = new XmlSerializer(typeof(Location));
            if (typeof(T) == typeof(IndexViewModel))
                xmlSerializer = new XmlSerializer(typeof(IndexViewModel));
            if (typeof(T) == typeof(MyEventsListerViewModel))
                xmlSerializer = new XmlSerializer(typeof(MyEventsListerViewModel));
#endif
        }

        [Benchmark(Description = nameof(XmlSerializer))]
        public void XmlSerializer_()
        {
            memoryStream.Position = 0;
            xmlSerializer.Serialize(memoryStream, value);
        }

        [Benchmark(Description = nameof(DataContractSerializer))]
        public void DataContractSerializer_()
        {
            memoryStream.Position = 0;
            dataContractSerializer.WriteObject(memoryStream, value);
        }

        [GlobalCleanup]
        public void Dispose() => memoryStream.Dispose();
    }

    public class SgenConfig : ManualConfig
    {
        public SgenConfig()
        {
            Add(Job.ShortRun
                .WithCustomBuildConfiguration("SGEN") // this is going to use Microsoft.XmlSerializer.Generator
                .With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp21)).WithId("SGEN"));

            Add(Job.ShortRun
                .With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp21)).WithId("NO_SGEN"));

            // make sure that Benchmarks.XmlSerializers.dll file exists (https://github.com/dotnet/core/blob/master/samples/xmlserializergenerator-instructions.md)
            KeepBenchmarkFiles = true;
        }
    }
}
