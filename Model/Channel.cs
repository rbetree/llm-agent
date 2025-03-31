using System;
using System.Collections.Generic;

namespace llm_agent.Model
{
    /// <summary>
    /// 表示一个LLM渠道
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// 渠道ID
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 渠道类型
        /// </summary>
        public ProviderType ProviderType { get; set; } = ProviderType.OpenAI;

        /// <summary>
        /// API密钥
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// API主机地址
        /// </summary>
        public string ApiHost { get; set; } = string.Empty;

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 是否使用流式响应
        /// </summary>
        public bool UseStreamResponse { get; set; } = true;

        /// <summary>
        /// 支持的模型列表
        /// </summary>
        public List<string> SupportedModels { get; set; } = new List<string>();

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
} 