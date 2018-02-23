using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using Benchmarks.Serializers;
using System;
using System.Linq;
using System.Runtime.InteropServices;

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
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(Job.Default.With(Runtime.Core).With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp20)).AsBaseline().WithId("2.0"));
            Add(Job.Default.With(Runtime.Core).With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp21)).WithId("2.1"));

#if NET46
            if (Type.GetType("Mono.Runtime") == null) // not a Mono
                Add(Job.Default.With(Runtime.Clr).WithId("4.x"));
#endif
            Add(Job.Default.With(Runtime.Mono).WithId("Mono"));

            Add(MemoryDiagnoser.Default);

            Add(DefaultConfig.Instance.GetValidators().ToArray());
            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());

            Add(new CsvMeasurementsExporter(CsvSeparator.Semicolon));
            //Add(RPlotExporter.Default); // it produces nice plots but requires R to be installed
            Add(MarkdownExporter.GitHub);
            Add(HtmlExporter.Default);
            //Add(StatisticColumn.AllStatistics);

            Set(new BenchmarkDotNet.Reports.SummaryStyle
            {
                PrintUnitsInHeader = true,
                PrintUnitsInContent = false,
                TimeUnit = BenchmarkDotNet.Horology.TimeUnit.Microsecond,
                SizeUnit = BenchmarkDotNet.Columns.SizeUnit.B
            });
        }
    }

    public class CustomPathsBenchmarkConfig : ManualConfig
    {
        public CustomPathsBenchmarkConfig(string dotnetCliPath, string monoPath)
        {
            Add(Job.Default.With(Runtime.Core).With(CsProjCoreToolchain.From(new NetCoreAppSettings("netcoreapp2.0", null, "2.0", dotnetCliPath))).AsBaseline().WithId("2.0"));
            Add(Job.Default.With(Runtime.Core).With(CsProjCoreToolchain.From(new NetCoreAppSettings("netcoreapp2.1", null, "2.1", dotnetCliPath))).WithId("2.1"));

#if NET46
            if (Type.GetType("Mono.Runtime") == null) // not a Mono
                Add(Job.Default.With(Runtime.Clr).WithId("4.x"));
#endif

            Add(Job.Default.With(new MonoRuntime("Mono", monoPath)).WithId("Mono"));

            Add(MemoryDiagnoser.Default);

            Add(DefaultConfig.Instance.GetValidators().ToArray());
            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());

            Add(new CsvMeasurementsExporter(CsvSeparator.Semicolon));
            Add(MarkdownExporter.GitHub);
            Add(HtmlExporter.Default);
            Add(StatisticColumn.AllStatistics);

            Set(new BenchmarkDotNet.Reports.SummaryStyle
            {
                PrintUnitsInHeader = true,
                PrintUnitsInContent = false,
                TimeUnit = BenchmarkDotNet.Horology.TimeUnit.Microsecond,
                SizeUnit = BenchmarkDotNet.Columns.SizeUnit.B
            });
        }
    }
}
