using Aliyun.Api.LogService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.AliyunLog.Sinks.AliyunLog
{
    /// <summary>
    /// AliyunLog客户端，这个类是允许向AliyunLog发送消息的引擎
    /// </summary>
    public class AliyunLogClient
    {
        private readonly AliyunLogClientConfiguration _config;

        public AliyunLogClient(AliyunLogClientConfiguration clientConfiguration)
        {
            _config = clientConfiguration;
        }

        /// <summary>
        /// 发布一个消息到AliyunLog
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(DateTimeOffset time, Dictionary<string, string> contents)
        {
            _config.LogGroupInfo.Logs.Add(new Aliyun.Api.LogService.Domain.Log.LogInfo
            {
                Time = time,
                Contents = contents
            });

            var response = await _config.LogServiceClient.PostLogStoreLogsAsync(_config.LogstoreName, _config.LogGroupInfo, hashKey: Guid.NewGuid().ToString("N"), project: _config.Project);

            if (!response.IsSuccess)
            {
                // TODO: 失败处理？
            }
        }
    }
}
