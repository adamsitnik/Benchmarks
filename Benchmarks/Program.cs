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
                typeof(Xml_FromStream<>)
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
