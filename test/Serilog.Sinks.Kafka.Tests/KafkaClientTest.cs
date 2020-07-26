using Confluent.Kafka;
using Serilog.Sinks.AliyunLog.Sinks.AliyunLog;
using Shouldly;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Serilog.Sinks.AliyunLog.Tests
{
    /// <summary>
    /// Kafka客户端测试类
    /// </summary>
    public class KafkaClientTest
    {
        private const string BootstrapServers = "localhost:9092";
        private const string Topic = "localhost-kafka";

        /// <summary>
        /// 发布消息后消费者应该要接收到这个消息
        /// </summary>
        [Fact]
        public async Task Publish_Message_Consumer_Should_Receive()
        {
            KafkaClient kafkaClient = new KafkaClient(new KafkaClientConfiguration
            {
                ProducerConfig = new ProducerConfig
                {
                    BootstrapServers = BootstrapServers
                },
                Topic = Topic
            });

            string message = $"hello kafka";

            await kafkaClient.PublishAsync(message);

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

                try
                {
                    var cr = c.Consume(1000);

                    cr.Message.Value.ShouldBe(message);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
    }
}
