using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using llm_agent.Model;
using llm_agent.API.Provider;
using llm_agent.Common.Exceptions;

namespace llm_agent.BLL
{
    /// <summary>
    /// 渠道服务，封装与渠道API相关的操作
    /// </summary>
    public class ChannelService
    {
        private readonly HttpClient _httpClient;
        private readonly ProviderFactory _providerFactory;
        
        public ChannelService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _providerFactory = new ProviderFactory(_httpClient);
        }
        
        /// <summary>
        /// 从API获取渠道支持的模型列表
        /// </summary>
        /// <param name="channel">渠道信息</param>
        /// <returns>模型列表</returns>
        public List<string> GetChannelModelsAsync(Channel channel)
        {
            try
            {
                if (channel == null)
                    throw new ArgumentNullException(nameof(channel));

                // 获取提供商实例
                var provider = _providerFactory.GetProvider(channel.ProviderType);
                if (provider == null)
                    throw new InvalidOperationException($"无法创建类型为 {channel.ProviderType} 的提供商");

                // 设置API密钥和主机地址
                provider.UpdateApiKey(channel.ApiKey);
                provider.UpdateApiHost(channel.ApiHost);

                // 尝试调用API获取模型列表
                List<string> models = new List<string>();

                // 对于支持的渠道类型，尝试获取模型列表
                if (channel.ProviderType == ProviderType.OpenAI || 
                    channel.ProviderType == ProviderType.AzureOpenAI)
                {
                    // 暂时使用内置模型列表
                    models = provider.GetSupportedModels();
                }
                else
                {
                    // 对于其他提供商，使用内置模型列表
                    models = provider.GetSupportedModels();
                }

                if (models == null || models.Count == 0)
                {
                    throw new ApiException($"未能从 {channel.ProviderType} API获取模型列表");
                }

                return models;
            }
            catch (Exception ex)
            {
                throw new ApiException($"获取模型列表失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 测试渠道连接
        /// </summary>
        /// <param name="channel">渠道信息</param>
        /// <param name="modelName">模型名称</param>
        /// <returns>测试结果和响应文本</returns>
        public async Task<(bool success, string response)> TestChannelConnectionAsync(Channel channel, string modelName)
        {
            try
            {
                if (channel == null)
                    throw new ArgumentNullException(nameof(channel));
                
                if (string.IsNullOrEmpty(modelName))
                    throw new ArgumentException("模型名称不能为空", nameof(modelName));

                // 获取提供商实例
                var provider = _providerFactory.GetProvider(channel.ProviderType);
                if (provider == null)
                    throw new InvalidOperationException($"无法创建类型为 {channel.ProviderType} 的提供商");

                // 设置API密钥和主机地址
                provider.UpdateApiKey(channel.ApiKey);
                provider.UpdateApiHost(channel.ApiHost);

                // 构建测试消息
                var messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Role = ChatRole.System,
                        Content = "您是一个有帮助的助手，这是一个连接测试。"
                    },
                    new ChatMessage
                    {
                        Role = ChatRole.User,
                        Content = "请回复'连接测试成功'以验证连接正常工作。"
                    }
                };

                // 发送测试请求
                string response = await provider.ChatAsync(messages, modelName);

                // 检查响应
                bool success = !string.IsNullOrEmpty(response) && 
                              (response.Contains("连接测试成功") || 
                               response.Contains("测试") || 
                               response.Contains("成功"));

                return (success, response);
            }
            catch (Exception ex)
            {
                return (false, $"测试连接失败: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 发送消息并获取非流式响应
        /// </summary>
        /// <param name="channel">渠道信息</param>
        /// <param name="modelName">模型名称</param>
        /// <param name="messages">要发送的消息列表</param>
        /// <returns>模型的响应文本</returns>
        public async Task<string> SendMessageAsync(Channel channel, string modelName, List<ChatMessage> messages)
        {
            try
            {
                if (channel == null)
                    throw new ArgumentNullException(nameof(channel));
                
                if (string.IsNullOrEmpty(modelName))
                    throw new ArgumentException("模型名称不能为空", nameof(modelName));
                
                if (messages == null || !messages.Any())
                    throw new ArgumentException("消息列表不能为空", nameof(messages));

                // 获取提供商实例
                var provider = _providerFactory.GetProvider(channel.ProviderType);
                if (provider == null)
                    throw new InvalidOperationException($"无法创建类型为 {channel.ProviderType} 的提供商");

                // 设置API密钥和主机地址
                provider.UpdateApiKey(channel.ApiKey);
                provider.UpdateApiHost(channel.ApiHost);

                // 发送请求并获取响应
                string response = await provider.ChatAsync(messages, modelName);
                return response;
            }
            catch (Exception ex)
            {
                throw new ApiException($"发送消息失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 发送消息并获取流式响应
        /// </summary>
        /// <param name="channel">渠道信息</param>
        /// <param name="modelName">模型名称</param>
        /// <param name="messages">要发送的消息列表</param>
        /// <param name="chunkHandler">处理每个响应块的委托</param>
        /// <returns></returns>
        public async Task SendStreamMessageAsync(Channel channel, string modelName, List<ChatMessage> messages, Action<string> chunkHandler)
        {
            try
            {
                if (channel == null)
                    throw new ArgumentNullException(nameof(channel));
                
                if (string.IsNullOrEmpty(modelName))
                    throw new ArgumentException("模型名称不能为空", nameof(modelName));
                
                if (messages == null || !messages.Any())
                    throw new ArgumentException("消息列表不能为空", nameof(messages));
                
                if (chunkHandler == null)
                    throw new ArgumentNullException(nameof(chunkHandler));

                // 获取提供商实例
                var provider = _providerFactory.GetProvider(channel.ProviderType);
                if (provider == null)
                    throw new InvalidOperationException($"无法创建类型为 {channel.ProviderType} 的提供商");

                // 设置API密钥和主机地址
                provider.UpdateApiKey(channel.ApiKey);
                provider.UpdateApiHost(channel.ApiHost);

                // 发送流式请求并处理响应块
                await foreach (var chunk in provider.StreamChatAsync(messages, modelName))
                {
                    if (!string.IsNullOrEmpty(chunk))
                    {
                        chunkHandler(chunk);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApiException($"发送流式消息失败: {ex.Message}", ex);
            }
        }
    }
} 