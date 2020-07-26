using Aliyun.Api.LogService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using Xunit;

namespace Serilog.Sinks.AliyunLog.Tests
{
    public class AliyunLogSinkTest
    {
        public const string accessKeyId = "<accessKeyId>";
        public const string accessKey = "<accessKey>";
        public const string project = "serilog-aliyunlog-test";
        public const string logStore = "sink-log-store";

        [Fact]
        public void EventsAreWrittenToTheAliyunLog()
        {
            var targetFramework = typeof(ILogServiceClient)
                .Assembly
                .GetCustomAttribute<TargetFrameworkAttribute>()
                .FrameworkName;

            var stats = new
            {
                DateTime.Now,
                RuntimeInformation.OSDescription,
                RuntimeInformation.OSArchitecture,
                RuntimeInformation.ProcessArchitecture,
                TargetFramework = targetFramework
            };

            var client = LogServiceClientBuilders
                .HttpBuilder
                .Endpoint("https://cn-qingdao.log.aliyuncs.com", project)
                .Credential(accessKeyId, accessKey)
                .Build();

            var log = new LoggerConfiguration()
                .WriteTo.AliyunLog(client, logStore, logTags: new Dictionary<string, string> {
                    { "Env", "UnitTest" },
                    { "App", "Serilog.Sinks.AliyunLog" }
                })
                .MinimumLevel.Verbose()
                .CreateLogger();


            log.Information("this is info message {@stats}", stats);
            log.ForContext<AliyunLogSinkTest>().Error(new NotImplementedException(), "this is error message with context {@stats}", stats);
            log.Debug("{@stats}", stats);

            if (Debugger.IsAttached)
            {
                SpinWait.SpinUntil(() => false);
            }
            else
            {
                SpinWait.SpinUntil(() => false, 1000 * 60 * 1);
            }
        }
    }
}
