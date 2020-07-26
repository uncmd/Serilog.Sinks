using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.AliyunLog.Sinks.AliyunLog
{
    /// <summary>
    /// Kafka客户端，这个类是允许向Kafka发送消息的引擎
    /// </summary>
    public class KafkaClient
    {
        private readonly KafkaClientConfiguration _config;

        public KafkaClient(KafkaClientConfiguration clientConfiguration)
        {
            _config = clientConfiguration;
        }

        /// <summary>
        /// 发布一个消息到Kafka
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(string message)
        {
            using (var p = new ProducerBuilder<Null, string>(_config.ProducerConfig).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync(_config.Topic, new Message<Null, string> { Value = message });
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    // TODO: 发送失败后处理？
                }
            }
        }
    }
}
