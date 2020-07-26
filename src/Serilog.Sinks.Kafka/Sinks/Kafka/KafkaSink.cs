using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.AliyunLog.Sinks.AliyunLog
{
    /// <summary>
    /// Serilog Kafka Sink - 允许您使用Serilog将日志发送到Kafka
    /// </summary>
    public class KafkaSink : PeriodicBatchingSink
    {
        private readonly ITextFormatter _formatter;
        private readonly KafkaClient _client;

        public KafkaSink(KafkaClientConfiguration configuration,
            KafkaSinkConfiguration sinkConfiguration) 
            : base(sinkConfiguration.BatchPostingLimit, sinkConfiguration.Period)
        {
            _formatter = sinkConfiguration.TextFormatter;
            _client = new KafkaClient(configuration);
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            foreach (var logEvent in events)
            {
                var sw = new StringWriter();
                _formatter.Format(logEvent, sw);
                await _client.PublishAsync(sw.ToString());
            }
        }
    }
}
