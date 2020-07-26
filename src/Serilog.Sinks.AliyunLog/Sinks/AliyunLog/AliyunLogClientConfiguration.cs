using Aliyun.Api.LogService;
using Aliyun.Api.LogService.Domain.Log;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serilog.Sinks.AliyunLog.Sinks.AliyunLog
{
    /// <summary>
    /// AliyunLogClient配置类
    /// </summary>
    public class AliyunLogClientConfiguration
    {
        /// <summary>
        /// 项目
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// 日志库
        /// </summary>
        public string LogstoreName { get; set; }

        /// <summary>
        /// 日志组信息（Topic、Tags、Source）
        /// </summary>
        public LogGroupInfo LogGroupInfo { get; set; }

        /// <summary>
        /// 日志服务客户端
        /// </summary>
        public ILogServiceClient LogServiceClient { get; set; }
    }
}
