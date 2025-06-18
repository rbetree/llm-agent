using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using llm_agent.Model;

namespace llm_agent.API.Provider
{
    /// <summary>
    /// 管理LLM提供商的工厂类
    /// </summary>
    public class ProviderFactory
    {
        /// <summary>
        /// HTTP客户端
        /// </summary>
        private readonly HttpClient _httpClient;
        
        /// <summary>
        /// 提供商字典
        /// </summary>
        private readonly Dictionary<ProviderType, BaseLLMProvider> _providers = new Dictionary<ProviderType, BaseLLMProvider>();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProviderFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
            InitializeProviders();
        }
        
        /// <summary>
        /// 初始化提供商
        /// </summary>
        private void InitializeProviders()
        {
            // 在这里初始化所有支持的提供商
            _providers[ProviderType.OpenAI] = new OpenAIProvider(_httpClient, "", GetDefaultApiHost(ProviderType.OpenAI));
            _providers[ProviderType.AzureOpenAI] = new AzureOpenAIProvider(_httpClient, "", GetDefaultApiHost(ProviderType.AzureOpenAI));
            _providers[ProviderType.Google] = new GeminiProvider(_httpClient, "", GetDefaultApiHost(ProviderType.Google));
            _providers[ProviderType.Anthropic] = new AnthropicProvider(_httpClient, "", GetDefaultApiHost(ProviderType.Anthropic));
        }
        
        /// <summary>
        /// 获取指定类型的提供商
        /// </summary>
        public BaseLLMProvider GetProvider(ProviderType providerType)
        {
            if (_providers.TryGetValue(providerType, out var provider))
            {
                return provider;
            }
            
            return null;
        }
        
        /// <summary>
        /// 获取支持的提供商类型列表
        /// </summary>
        public List<ProviderType> GetSupportedProviderTypes()
        {
            return new List<ProviderType>(_providers.Keys);
        }
        
        /// <summary>
        /// 获取提供商名称
        /// </summary>
        public static string GetProviderDisplayName(ProviderType providerType)
        {
            return providerType switch
            {
                ProviderType.OpenAI => "OpenAI",
                ProviderType.AzureOpenAI => "Azure OpenAI",
                ProviderType.Anthropic => "Anthropic Claude",
                ProviderType.Google => "Google Gemini",
                _ => "未知提供商"
            };
        }
        
        /// <summary>
        /// 获取提供商默认API主机地址
        /// </summary>
        public static string GetDefaultApiHost(ProviderType providerType)
        {
            return providerType switch
            {
                ProviderType.OpenAI => "https://api.openai.com/v1",
                ProviderType.AzureOpenAI => "https://your-resource-name.openai.azure.com",
                ProviderType.Anthropic => "https://api.anthropic.com",
                ProviderType.Google => "https://generativelanguage.googleapis.com/v1",
                _ => ""
            };
        }
        
        /// <summary>
        /// 从显示名称获取提供商类型
        /// </summary>
        public ProviderType GetProviderTypeFromDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return ProviderType.OpenAI;
                
            if (displayName.Contains("OpenAI", StringComparison.OrdinalIgnoreCase))
            {
                if (displayName.Contains("Azure", StringComparison.OrdinalIgnoreCase))
                    return ProviderType.AzureOpenAI;
                else
                    return ProviderType.OpenAI;
            }
            else if (displayName.Contains("Anthropic", StringComparison.OrdinalIgnoreCase) || 
                     displayName.Contains("Claude", StringComparison.OrdinalIgnoreCase))
                return ProviderType.Anthropic;
            else if (displayName.Contains("Google", StringComparison.OrdinalIgnoreCase) ||
                     displayName.Contains("Gemini", StringComparison.OrdinalIgnoreCase))
                return ProviderType.Google;
            
            // 默认返回OpenAI
            return ProviderType.OpenAI;
        }
        
        /// <summary>
        /// 创建提供商实例
        /// </summary>
        public static BaseLLMProvider CreateProvider(string providerType, string apiKey, string apiHost = "")
        {
            if (Enum.TryParse<ProviderType>(providerType, out var type))
            {
                return CreateProvider(type, apiKey, apiHost);
            }
            return new MockProvider(apiKey, apiHost);
        }
        
        /// <summary>
        /// 创建提供商实例
        /// </summary>
        public static BaseLLMProvider CreateProvider(ProviderType providerType, string apiKey, string apiHost = "")
        {
            // 如果没有提供API主机地址，则使用默认值
            if (string.IsNullOrEmpty(apiHost))
            {
                apiHost = GetDefaultApiHost(providerType);
            }
            
            HttpClient httpClient = new HttpClient();
            // 设置适当的超时
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            
            return providerType switch
            {
                ProviderType.OpenAI => new OpenAIProvider(httpClient, apiKey, apiHost),
                ProviderType.AzureOpenAI => new AzureOpenAIProvider(httpClient, apiKey, apiHost),
                ProviderType.Anthropic => new AnthropicProvider(httpClient, apiKey, apiHost),
                ProviderType.Google => new GeminiProvider(httpClient, apiKey, apiHost),
                _ => new MockProvider(apiKey, apiHost)
            };
        }
    }
    
    /// <summary>
    /// 模拟提供商实现（用于测试）
    /// </summary>
    public class MockProvider : BaseLLMProvider
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MockProvider(string apiKey = "mock-api-key", string apiHost = "https://api.mock-provider.com") : base(apiKey, apiHost)
        {
        }

        /// <summary>
        /// 获取支持的模型列表
        /// </summary>
        public override List<string> GetSupportedModels()
        {
            return new List<string> { "gpt-3.5-turbo", "gpt-4" };
        }
        
        /// <summary>
        /// 发送聊天消息并接收响应
        /// </summary>
        public override async Task<string> ChatAsync(List<ChatMessage> messages, string modelId)
        {
            await Task.Delay(1000); // 模拟网络延迟
            
            string lastUserMessage = "没有消息";
            foreach (var message in messages)
            {
                if (message.Role == ChatRole.User)
                {
                    lastUserMessage = message.Content;
                }
            }
            
            return $"这是来自模拟提供商的响应，使用模型：{modelId}。您的消息是：{lastUserMessage}";
        }
        
        /// <summary>
        /// 获取提供商类型
        /// </summary>
        public override ProviderType GetProviderType()
        {
            return ProviderType.OpenAI;
        }
    }
} 