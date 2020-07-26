using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.AliyunLog.Sinks.AliyunLog
{
    /// <summary>
    /// KafkaClient配置类
    /// </summary>
    public class KafkaClientConfiguration
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }

        /// <summary>
        /// 生产者配置
        /// </summary>
        public ProducerConfig ProducerConfig { get; set; }
    }
}
