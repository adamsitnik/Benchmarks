using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;
using Benchmarks.Serializers;
using System.Linq;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
            => BenchmarkSwitcher.FromTypes
                (
                    new[]
                    {
                        typeof(Json_ToString<LoginViewModel>),
                        typeof(Json_ToString<Location>),
                        typeof(Json_ToString<IndexViewModel>),

                        typeof(Json_ToStream<LoginViewModel>),
                        typeof(Json_ToStream<Location>),
                        typeof(Json_ToStream<IndexViewModel>),

                        typeof(Json_FromString<LoginViewModel>),
                        typeof(Json_FromString<Location>),
                        typeof(Json_FromString<IndexViewModel>),

                        typeof(Json_FromStream<LoginViewModel>),
                        typeof(Json_FromStream<Location>),
                        typeof(Json_FromStream<IndexViewModel>),
                    }
                )
                .Run(args, new BenchmarkConfig());
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(Job.ShortRun.With(CsProjCoreToolchain.From(NetCoreAppSettings.NetCoreApp21)));

            Add(MemoryDiagnoser.Default);

            Add(DefaultConfig.Instance.GetValidators().ToArray());
            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            Add(DefaultConfig.Instance.GetExporters().ToArray());
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
            //Add(StatisticColumn.AllStatistics);
        }
    }
}
