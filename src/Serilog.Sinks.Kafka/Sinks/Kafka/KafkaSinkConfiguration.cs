using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.AliyunLog.Sinks.AliyunLog
{
    /// <summary>
    /// KafkaSink配置类
    /// </summary>
    public class KafkaSinkConfiguration
    {
        /// <summary>
        /// 单个批处理中包含的最大事件数
        /// </summary>
        public int BatchPostingLimit { get; set; }

        /// <summary>
        /// 检查事件批之间的等待时间
        /// </summary>
        public TimeSpan Period { get; set; }

        /// <summary>
        /// 输出文本格式
        /// </summary>
        public ITextFormatter TextFormatter { get; set; } = new CompactJsonFormatter();

        /// <summary>
        /// 限制的最小日志级别
        /// </summary>
        public LogEventLevel RestrictedToMinimumLevel { get; set; } = LogEventLevel.Verbose;
    }
}
