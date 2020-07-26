using Aliyun.Api.LogService;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Formatting.Json;
using Serilog.Sinks.AliyunLog.Sinks.AliyunLog;
using System;
using System.Collections.Generic;

namespace Serilog.Sinks.AliyunLog
{
    public static class LoggerConfigurationAliyunExtension
    {
        private const string DefaultOutputTemplate = "{Message}";
        private const int DefaultBatchPostingLimit = 50;
        private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);
        private const string DefaultTopic = "serilog-topic";

        public static LoggerConfiguration AliyunLog(
            this LoggerSinkConfiguration sinkConfiguration,
            ILogServiceClient logServiceClient,
            string logstoreName = null,
            string project = null,
            string source = null,
            string topic = null,
            IDictionary<string, string> logTags = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null) 
                throw new ArgumentNullException(nameof(sinkConfiguration));
            if (logstoreName == null) 
                throw new ArgumentNullException(nameof(logstoreName));
            if (logServiceClient == null) 
                throw new ArgumentNullException(nameof(logServiceClient));

            return sinkConfiguration.AliyunLog(logServiceClient, null, logstoreName, project, source, topic, logTags, restrictedToMinimumLevel, levelSwitch);
        }

        public static LoggerConfiguration AliyunLog(
            this LoggerSinkConfiguration sinkConfiguration,
            ILogServiceClient logServiceClient,
            ITextFormatter formatter = null,
            string logstoreName = null,
            string project = null,
            string source = null,
            string topic = null,
            IDictionary<string, string> logTags = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            LoggingLevelSwitch levelSwitch = null)
        {
            if (sinkConfiguration == null) 
                throw new ArgumentNullException(nameof(sinkConfiguration));
            if (logstoreName == null) 
                throw new ArgumentNullException(nameof(logstoreName));
            if (logServiceClient == null) 
                throw new ArgumentNullException(nameof(logServiceClient));

            AliyunLogClientConfiguration clientConfiguration = new AliyunLogClientConfiguration
            {
                Project = project,
                LogstoreName = logstoreName,
                LogGroupInfo = new Aliyun.Api.LogService.Domain.Log.LogGroupInfo
                {
                    Topic = topic,
                    LogTags = logTags,
                    Source = source,
                },
                LogServiceClient = logServiceClient
            };

            AliyunLogSinkConfiguration logSinkConfiguration = new AliyunLogSinkConfiguration
            {
                BatchPostingLimit = DefaultBatchPostingLimit,
                Period = DefaultPeriod,
                RestrictedToMinimumLevel = LogEventLevel.Verbose,
            };

            if (formatter != null)
                logSinkConfiguration.TextFormatter = formatter;

            var sink = new AliyunLogSink(clientConfiguration, logSinkConfiguration);

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
