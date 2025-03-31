using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Linq;
using llm_agent.Model;
using llm_agent.Models;
using llm_agent.DAL;

namespace llm_agent.API.Provider
{
    /// <summary>
    /// LLM提供商的基础实现
    /// </summary>
    public abstract class BaseLLMProvider
    {
        /// <summary>
        /// API密钥
        /// </summary>
        protected string ApiKey { get; set; } = string.Empty;
        
        /// <summary>
        /// API主机地址
        /// </summary>
        protected string ApiHost { get; set; } = string.Empty;
        
        /// <summary>
        /// 当前使用的模型
        /// </summary>
        protected string CurrentModel { get; set; } = string.Empty;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseLLMProvider(string apiKey = "", string apiHost = "")
        {
            ApiKey = apiKey ?? string.Empty;
            ApiHost = apiHost ?? string.Empty;
            CurrentModel = GetDefaultModel();
        }
        
        /// <summary>
        /// 获取默认模型
        /// </summary>
        protected virtual string GetDefaultModel()
        {
            var models = GetSupportedModels();
            return models.Count > 0 ? models[0] : string.Empty;
        }
        
        /// <summary>
        /// 获取支持的模型列表
        /// </summary>
        public abstract List<string> GetSupportedModels();
        
        /// <summary>
        /// 发送聊天消息并接收响应
        /// </summary>
        public abstract Task<string> ChatAsync(List<ChatMessage> messages, string modelId);
        
        /// <summary>
        /// 流式发送聊天消息并接收响应
        /// </summary>
        public virtual async IAsyncEnumerable<string> StreamChatAsync(List<ChatMessage> messages, string modelId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // 默认实现：非流式方式发送，然后一次性返回
            string response = await ChatAsync(messages, modelId);
            yield return response;
        }
        
        /// <summary>
        /// 获取API密钥
        /// </summary>
        public virtual string GetApiKey()
        {
            return ApiKey;
        }
        
        /// <summary>
        /// 更新API密钥
        /// </summary>
        public virtual void UpdateApiKey(string apiKey)
        {
            ApiKey = apiKey ?? string.Empty;
        }
        
        /// <summary>
        /// 获取API主机地址
        /// </summary>
        public virtual string GetApiHost()
        {
            return ApiHost;
        }
        
        /// <summary>
        /// 更新API主机地址
        /// </summary>
        public virtual void UpdateApiHost(string apiHost)
        {
            ApiHost = apiHost ?? string.Empty;
        }
        
        /// <summary>
        /// 获取提供商类型
        /// </summary>
        public abstract ProviderType GetProviderType();
        
        /// <summary>
        /// 获取可用的模型列表
        /// </summary>
        public virtual List<string> GetAvailableModels()
        {
            try
            {
                // 获取提供商类型字符串
                string providerStr = GetProviderType().ToString().ToLower();
                if (providerStr == "azureopenai")
                    providerStr = "openai"; // 特殊处理Azure OpenAI
                
                // 从数据库获取已启用的模型
                DatabaseManager dbManager = new DatabaseManager();
                var dbModels = dbManager.GetModels(providerStr);
                
                // 只返回已启用的模型ID
                if (dbModels.Count > 0)
                {
                    return dbModels.Where(m => m.Enabled).Select(m => m.Id).ToList();
                }
                
                // 如果数据库中没有，返回所有支持的模型
                return GetSupportedModels();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"从数据库获取可用模型时出错: {ex.Message}");
                return GetSupportedModels();
            }
        }
    }
} 