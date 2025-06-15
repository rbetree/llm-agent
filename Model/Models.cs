using System;
using System.Collections.Generic;

namespace llm_agent.Models
{
    // 模型定义类
    public class ModelInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProviderType { get; set; }
        public int? ContextLength { get; set; }
        public double? TokenPrice { get; set; }
        public bool Enabled { get; set; } = true;

        public ModelInfo(string id, string name, string providerType, int? contextLength = null, double? tokenPrice = null)
        {
            Id = id;
            Name = name;
            ProviderType = providerType;
            ContextLength = contextLength;
            TokenPrice = tokenPrice;
        }
    }

    // 模型配置静态类
    public static class ModelsConfig
    {
        // OpenAI模型
        public static readonly List<ModelInfo> OpenAIModels = new List<ModelInfo>
        {
            new ModelInfo("gpt-4o", "GPT-4o", "openai", 128000, 0.005),
            new ModelInfo("gpt-4-turbo", "GPT-4 Turbo", "openai", 128000, 0.01),
            new ModelInfo("gpt-4", "GPT-4", "openai", 8192, 0.03),
            new ModelInfo("gpt-3.5-turbo", "GPT-3.5 Turbo", "openai", 16385, 0.0015),
            new ModelInfo("dall-e-3", "DALL-E 3", "openai"),
            new ModelInfo("text-embedding-ada-002", "Embeddings v2", "openai", null, 0.0001)
        };

        // Anthropic模型
        public static readonly List<ModelInfo> AnthropicModels = new List<ModelInfo>
        {
            new ModelInfo("claude-3-opus-20240229", "Claude 3 Opus", "anthropic", 200000, 0.015),
            new ModelInfo("claude-3-sonnet-20240229", "Claude 3 Sonnet", "anthropic", 200000, 0.003),
            new ModelInfo("claude-3-haiku-20240307", "Claude 3 Haiku", "anthropic", 200000, 0.00025),
            new ModelInfo("claude-2.1", "Claude 2.1", "anthropic", 100000, 0.008),
            new ModelInfo("claude-2.0", "Claude 2.0", "anthropic", 100000, 0.008)
        };

        // Google Gemini模型
        public static readonly List<ModelInfo> GeminiModels = new List<ModelInfo>
        {
            new ModelInfo("gemini-1.5-pro", "Gemini 1.5 Pro", "gemini", 1000000, 0.0025),
            new ModelInfo("gemini-1.5-flash", "Gemini 1.5 Flash", "gemini", 1000000, 0.0007),
            new ModelInfo("gemini-pro", "Gemini Pro", "gemini", 32760, 0.0005)
        };

        // 百度文心一言模型
        public static readonly List<ModelInfo> BaiduModels = new List<ModelInfo>
        {
            new ModelInfo("ernie-bot-4", "文心一言 4.0", "baidu", 16000),
            new ModelInfo("ernie-bot", "文心一言", "baidu", 8000)
        };

        // 智谱AI模型
        public static readonly List<ModelInfo> ZhipuModels = new List<ModelInfo>
        {
            new ModelInfo("glm-4", "智谱 GLM-4", "zhipu", 128000),
            new ModelInfo("glm-3-turbo", "智谱 GLM-3-Turbo", "zhipu", 32000)
        };

        // 硅基流动模型
        public static readonly List<ModelInfo> SiliconFlowModels = new List<ModelInfo>
        {
            new ModelInfo("moonshot-v1-8k", "月之暗面 Moonshot-v1-8k", "siliconflow", 8192),
            new ModelInfo("moonshot-v1-32k", "月之暗面 Moonshot-v1-32k", "siliconflow", 32768),
            new ModelInfo("moonshot-v1-128k", "月之暗面 Moonshot-v1-128k", "siliconflow", 131072),
            new ModelInfo("deepseek-chat", "DeepSeek-Chat", "siliconflow", 16384),
            new ModelInfo("deepseek-coder", "DeepSeek-Coder", "siliconflow", 16384),
            new ModelInfo("yi-large", "Yi-Large", "siliconflow", 32768),
            new ModelInfo("yi-1.5-9b-chat", "Yi-1.5-9B-Chat", "siliconflow", 4096),
            new ModelInfo("yi-1.5-34b-chat", "Yi-1.5-34B-Chat", "siliconflow", 16384),
            new ModelInfo("qwen-max", "Qwen-Max", "siliconflow", 8192),
            new ModelInfo("qwen-1.5-72b-chat", "Qwen-1.5-72B-Chat", "siliconflow", 8192)
        };

        // 获取指定提供商的模型列表
        public static List<ModelInfo> GetModelsByProvider(string providerType)
        {
            switch (providerType.ToLower())
            {
                case "openai":
                    return OpenAIModels;
                case "anthropic":
                    return AnthropicModels;
                case "gemini":
                    return GeminiModels;
                case "baidu":
                    return BaiduModels;
                case "zhipu":
                    return ZhipuModels;
                case "siliconflow":
                    return SiliconFlowModels;
                default:
                    return new List<ModelInfo>();
            }
        }
    }
} 