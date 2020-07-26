using Confluent.Kafka;
using Serilog.Configuration;
using Serilog.Formatting;
using Serilog.Sinks.AliyunLog.Sinks.AliyunLog;
using System;

namespace Serilog.Sinks.AliyunLog
{
    /// <summary>
    /// 为Kafka配置Sink的Serilog扩展方法
    /// </summary>
    public static class LoggerConfigurationKafkaExtension
    {
        private const int DefaultBatchPostingLimit = 50;
        private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(2);
        private const string DefaultTopic = "serilog-topic";

        /// <summary>
        /// 添加一个接收器，允许您将日志消息推送到Kafka
        /// </summary>
        public static LoggerConfiguration Kafka(
            this LoggerSinkConfiguration loggerConfiguration,
            Action<KafkaClientConfiguration, KafkaSinkConfiguration> configure)
        {
            KafkaClientConfiguration clientConfiguration = new KafkaClientConfiguration();
            KafkaSinkConfiguration sinkConfiguration = new KafkaSinkConfiguration();
            configure(clientConfiguration, sinkConfiguration);

            return RegisterSink(loggerConfiguration, clientConfiguration, sinkConfiguration);
        }

        /// <summary>
        /// 添加一个接收器，允许您将日志消息推送到Kafka
        /// </summary>
        public static LoggerConfiguration Kafka(
            this LoggerSinkConfiguration loggerConfiguration,
            KafkaClientConfiguration clientConfiguration, KafkaSinkConfiguration sinkConfiguration, ITextFormatter textFormatter = null)
        {
            if (textFormatter != null) sinkConfiguration.TextFormatter = textFormatter;
            return RegisterSink(loggerConfiguration, clientConfiguration, sinkConfiguration);
        }

        /// <summary>
        /// 添加一个接收器，允许您将日志消息推送到Kafka
        /// </summary>
        public static LoggerConfiguration Kafka(
            this LoggerSinkConfiguration loggerConfiguration,
            string bootstrapServer, string topic, int batchPostingLimit = 0,
            TimeSpan period = default, ITextFormatter formatter = null)
        {
            return loggerConfiguration.Kafka(new ProducerConfig() { BootstrapServers = bootstrapServer }, topic, batchPostingLimit, period, formatter);
        }

        /// <summary>
        /// 添加一个接收器，允许您将日志消息推送到Kafka
        /// </summary>
        public static LoggerConfiguration Kafka(
            this LoggerSinkConfiguration loggerConfiguration,
            ProducerConfig producerConfig, string topic, int batchPostingLimit = 0,
            TimeSpan period = default, ITextFormatter formatter = null
        )
        {
            KafkaClientConfiguration clientConfiguration = new KafkaClientConfiguration
            {
                Topic = topic,
                ProducerConfig = producerConfig
            };

            KafkaSinkConfiguration sinkConfiguration = new KafkaSinkConfiguration
            {
                BatchPostingLimit = batchPostingLimit,
                Period = period,
            };

            if (formatter != null)
                sinkConfiguration.TextFormatter = formatter;

            return RegisterSink(loggerConfiguration, clientConfiguration, sinkConfiguration);
        }

        static LoggerConfiguration RegisterSink(LoggerSinkConfiguration loggerConfiguration, KafkaClientConfiguration clientConfiguration, KafkaSinkConfiguration sinkConfiguration)
        {
            if (loggerConfiguration == null) 
                throw new ArgumentNullException("loggerConfiguration");
            if (string.IsNullOrEmpty(clientConfiguration.ProducerConfig.BootstrapServers)) 
                throw new ArgumentException("BootstrapServers不能为空, 至少指定一个主机名", "BootstrapServers");

            sinkConfiguration.BatchPostingLimit = (sinkConfiguration.BatchPostingLimit == default) ? DefaultBatchPostingLimit : sinkConfiguration.BatchPostingLimit;
            sinkConfiguration.Period = (sinkConfiguration.Period == default) ? DefaultPeriod : sinkConfiguration.Period;
            if (string.IsNullOrEmpty(clientConfiguration.Topic))
                clientConfiguration.Topic = DefaultTopic;

            return
                loggerConfiguration
                    .Sink(new KafkaSink(clientConfiguration, sinkConfiguration), sinkConfiguration.RestrictedToMinimumLevel);
        }
    }
}
