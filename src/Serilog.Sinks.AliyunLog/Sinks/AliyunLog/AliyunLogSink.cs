using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Domain.Log;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.AliyunLog.Sinks.AliyunLog
{
    class AliyunLogSink : PeriodicBatchingSink
    {
        private readonly ITextFormatter _formatter;
        private readonly AliyunLogClient _client;

        public AliyunLogSink(AliyunLogClientConfiguration configuration, AliyunLogSinkConfiguration sinkConfiguration) 
            : base(sinkConfiguration.BatchPostingLimit, sinkConfiguration.Period)
        {
            _formatter = sinkConfiguration.TextFormatter;
            _client = new AliyunLogClient(configuration);
        }

        async protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (events == null) 
                throw new ArgumentNullException(nameof(events));

            foreach (var logEvent in events)
            {
                var contents = new Dictionary<string, string>(logEvent.Properties.ToDictionary(k => k.Key, v => v.Value.ToString()))
                {
                    { "Level", logEvent.Level.ToString() }
                };

                if (logEvent.Exception != null)
                {
                    contents.Add("Exception", logEvent.Exception.ToString());
                }

                var stringBuilder = new StringBuilder();
                using (var stringWriter = new StringWriter(stringBuilder))
                {
                    _formatter.Format(logEvent, stringWriter);
                }
                contents.Add("Message", stringBuilder.ToString());

                await _client.PublishAsync(logEvent.Timestamp, contents);
            }
        }
    }
}
