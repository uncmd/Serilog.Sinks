using BenchmarkDotNet.Running;
using System;

namespace KafkaSinkBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<KafkaTest>();
        }
    }
}
