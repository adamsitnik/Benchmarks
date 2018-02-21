using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using Benchmarks.Serializers;
using System;
using System.Linq;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
            => BenchmarkSwitcher.FromTypes
                (
                    GetOpenGenericBenchmarks()
                        .SelectMany(openGeneric => GetViewModels().Select(viewModel => openGeneric.MakeGenericType(viewModel)))
                        .ToArray()
                )
                .Run(args, new BenchmarkConfig());

        static Type[] GetOpenGenericBenchmarks()
            => new Type[]
            {
                typeof(Json_ToString<>),
                typeof(Json_ToStream<>),
                typeof(Json_FromString<>),
                typeof(Json_FromStream<>),
                typeof(Xml_ToStream<>),
                typeof(Xml_FromStream<>),
                typeof(Binary_ToStream<>),
                typeof(Binary_FromStream<>)
            };

        static Type[] GetViewModels()
            => new Type[]
            {
                typeof(LoginViewModel),
                typeof(Location),
                typeof(IndexViewModel),
                typeof(MyEventsListerViewModel)
            };

        /// <summary>
        /// ZeroFormatter is so fast that I needed to verify if it's not cheating in any way ;)
        /// </summary>
        static void EnsureZeroFormatterIsNotCheating()
        {
            var sut = new Binary_FromStream<Location>();

            sut.SetupZeroFormatter_();

            Location[] deserialized = Enumerable.Range(0, 10).Select(_ => sut.ZeroFormatter_()).ToArray();

            for (int i = 1; i < deserialized.Length; i++)
            {
                if (object.ReferenceEquals(deserialized[i - 1], deserialized[i]))
                    throw new InvalidOperationException();
                if (object.ReferenceEquals(deserialized[i - 1].Address1, deserialized[i].Address1))
                    throw new InvalidOperationException();
                if (object.ReferenceEquals(deserialized[i - 1].Address2, deserialized[i].Address2))
                    throw new InvalidOperationException();
            }
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(Job.Default.With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp20)).AsBaseline().WithId("2.0"));
            Add(Job.Default.With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp21)).WithId("2.1"));
            Add(Job.Default.With(CsProjClassicNetToolchain.Net47).WithId("4.7"));

            Add(MemoryDiagnoser.Default);

            Add(DefaultConfig.Instance.GetValidators().ToArray());
            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            Add(DefaultConfig.Instance.GetExporters().ToArray());
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            //Add(StatisticColumn.AllStatistics);
        }
    }
}
