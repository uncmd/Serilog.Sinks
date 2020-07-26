using Confluent.Kafka;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using Serilog.Formatting.Json;
using Serilog.Sinks.AliyunLog.Sinks.AliyunLog;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.AliyunLog.Tests
{
    /// <summary>
    /// Kafka接收器测试类
    /// </summary>
    public class KafkaSinkTest
    {
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "kafkasinktest";

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

        /// <summary>
        /// 写入错误日志后消费者应该要接收到这个消息
        /// </summary>
        [Fact]
        public async Task Error_Log_Message_Consumer_Should_Receive()
        {
            var messageTemplate = "Denominator cannot be zero in {numerator}/{denominator}";
            int p1 = 1;
            int p2 = 0;

            this.logger.Error(new DivideByZeroException(), messageTemplate, p1, p2);

            await Task.Delay(100);

            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                c.Subscribe(Topic);

                var cr = c.Consume(1000);

                var receivedMessage = JObject.Parse(cr.Message.Value);

                receivedMessage["Level"].ShouldBe("Error");
                receivedMessage["MessageTemplate"].ShouldBe(messageTemplate);
                receivedMessage["Properties"].ShouldNotBeNull();
                receivedMessage["Properties"]["numerator"].ShouldBe(p1);
                receivedMessage["Properties"]["denominator"].ShouldBe(p2);
                receivedMessage["Exception"].ShouldBe("System.DivideByZeroException: Attempted to divide by zero.");
            }
        }
    }
}
