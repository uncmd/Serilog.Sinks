using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Confluent.Kafka;
using Serilog;
using Serilog.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.AliyunLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaSinkBenchmark
{
    [MemoryDiagnoser]
    [SimpleJob(warmupCount: 10, targetCount: 10)]
    public class KafkaTest
    {
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "kafkabenchmark";

        private readonly Logger logger = new LoggerConfiguration()
            .WriteTo.Kafka((clientConfiguration, sinkConfiguration) =>
            {
                clientConfiguration.Topic = Topic;
                clientConfiguration.ProducerConfig = new ProducerConfig
                {
                    BootstrapServers = BootstrapServers,
                };
                sinkConfiguration.TextFormatter = new JsonFormatter();
            })
            .MinimumLevel.Verbose()
            .CreateLogger();

        private readonly Logger fileLogger = new LoggerConfiguration()
            .WriteTo.File("log.txt")
            .MinimumLevel.Verbose()
            .CreateLogger();

        [Benchmark(Baseline = true)]
        public void WriteToFile()
        {
            fileLogger.Information("hello kafka");
        }

        [Benchmark]
        public void WriteToKafka()
        {
            logger.Information("hello kafka");
        }

        // * Detailed results *
        // KafkaTest.WriteToFile: Job-XNHECZ(IterationCount=10, WarmupCount=10)
        // Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
        // Mean = 5.384 μs, StdErr = 0.011 μs (0.21%), N = 9, StdDev = 0.034 μs
        // Min = 5.329 μs, Q1 = 5.375 μs, Median = 5.379 μs, Q3 = 5.390 μs, Max = 5.440 μs
        // IQR = 0.015 μs, LowerFence = 5.353 μs, UpperFence = 5.412 μs
        // ConfidenceInterval = [5.327 μs; 5.441 μs] (CI 99.9%), Margin = 0.057 μs (1.06% of Mean)
        // Skewness = 0.12, Kurtosis = 1.94, MValue = 2
        // -------------------- Histogram --------------------
        // [5.315 μs ; 5.461 μs) | @@@@@@@@@
        // ---------------------------------------------------
        // 
        // KafkaTest.WriteToKafka: Job-XNHECZ(IterationCount=10, WarmupCount=10)
        // Runtime = .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT; GC = Concurrent Workstation
        // Mean = 442.309 ns, StdErr = 1.488 ns (0.34%), N = 10, StdDev = 4.707 ns
        // Min = 434.234 ns, Q1 = 440.312 ns, Median = 442.369 ns, Q3 = 445.149 ns, Max = 449.622 ns
        // IQR = 4.836 ns, LowerFence = 433.057 ns, UpperFence = 452.403 ns
        // ConfidenceInterval = [435.193 ns; 449.425 ns] (CI 99.9%), Margin = 7.116 ns (1.61% of Mean)
        // Skewness = -0.15, Kurtosis = 1.86, MValue = 2
        // -------------------- Histogram --------------------
        // [431.367 ns ; 452.490 ns) | @@@@@@@@@@
        // ---------------------------------------------------
        // 
        // // * Summary *
        // 
        // BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.959 (1909/November2018Update/19H2)
        // Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
        // .NET Core SDK=3.1.302
        //   [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
        //   Job-XNHECZ : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
        // 
        // IterationCount=10  WarmupCount=10  
        // 
        // |       Method |       Mean |    Error |   StdDev | Ratio |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
        // |------------- |-----------:|---------:|---------:|------:|-------:|-------:|------:|----------:|
        // |  WriteToFile | 5,383.5 ns | 56.99 ns | 33.91 ns |  1.00 | 0.0687 |      - |     - |     336 B |
        // | WriteToKafka |   442.3 ns |  7.12 ns |  4.71 ns |  0.08 | 0.0215 | 0.0072 |     - |     152 B |
    }
}
