using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
using System.Linq;
using llm_agent.Model;

namespace llm_agent.API.Provider
{
    /// <summary>
    /// Anthropic Claude API提供商实现
    /// </summary>
    public class AnthropicProvider : BaseLLMProvider
    {
        private readonly HttpClient _httpClient;
        private int _maxRetries = 3;
        private string _apiVersion = "2023-06-01"; // Anthropic API版本

        /// <summary>
        /// 构造函数
        /// </summary>
        public AnthropicProvider(HttpClient httpClient, string apiKey = "", string apiHost = "")
            : base(apiKey, apiHost ?? "https://api.anthropic.com")
        {
            _httpClient = httpClient;
            // 确保HTTP客户端超时设置足够长，以处理长时间运行的请求
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// 获取支持的模型列表
        /// </summary>
        public override List<string> GetSupportedModels()
        {
            return new List<string> 
            { 
                "claude-3-opus",
                "claude-3-sonnet",
                "claude-3-haiku",
                "claude-2.1",
                "claude-2.0"
            };
        }

        /// <summary>
        /// 构建Anthropic格式的消息列表
        /// </summary>
        private List<object> BuildMessages(List<ChatMessage> messages)
        {
            var result = new List<object>();
            
            // 提取系统提示
            string systemPrompt = "";
            foreach (var msg in messages)
            {
                if (msg.Role == ChatRole.System)
                {
                    systemPrompt = msg.Content;
                    break;
                }
            }
            
            // 添加用户和助手消息
            foreach (var msg in messages)
            {
                if (msg.Role == ChatRole.System)
                    continue; // 跳过系统消息，已单独处理

                // Anthropic API角色映射
                string role = msg.Role switch
                {
                    ChatRole.User => "user",
                    ChatRole.Assistant => "assistant",
                    _ => "user"
                };

                result.Add(new
                {
                    role = role,
                    content = msg.Content
                });
            }
            
            return result;
        }

        /// <summary>
        /// 发送聊天消息并接收响应
        /// </summary>
        public override async Task<string> ChatAsync(List<ChatMessage> messages, string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                modelId = "claude-3-sonnet"; // 默认使用claude-3-sonnet模型
            }

            int retries = 0;
            Exception? lastException = null;

            while (retries < _maxRetries)
            {
                try
                {
                    string apiUrl = $"{ApiHost}/v1/messages";
                    
                    // 提取系统提示
                    string systemPrompt = "";
                    foreach (var msg in messages)
                    {
                        if (msg.Role == ChatRole.System)
                        {
                            systemPrompt = msg.Content;
                            break;
                        }
                    }
                    
                    // 构建消息数组 (Anthropic格式)
                    var anthropicMessages = BuildMessages(messages);

                    // 构建请求体
                    var requestBody = new
                    {
                        model = modelId,
                        messages = anthropicMessages,
                        system = string.IsNullOrEmpty(systemPrompt) ? null : systemPrompt,
                        temperature = 0.7,
                        max_tokens = 4096
                    };

                    // 转换为JSON
                    string requestJson = JsonSerializer.Serialize(requestBody,
                        new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                    
                    // 创建请求
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    
                    // 添加Anthropic API头
                    request.Headers.Add("x-api-key", ApiKey);
                    request.Headers.Add("anthropic-version", _apiVersion);
                    
                    // 发送请求
                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    
                    // 解析响应
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    
                    // Anthropic响应格式解析
                    if (doc.RootElement.TryGetProperty("content", out var content) &&
                        content.GetArrayLength() > 0 &&
                        content[0].TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? string.Empty;
                    }
                    
                    throw new Exception("无法解析Anthropic响应: " + responseBody);
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    retries++;
                    
                    if (retries < _maxRetries)
                    {
                        // 指数退避策略
                        int delayMs = (int)Math.Pow(2, retries) * 1000;
                        await Task.Delay(delayMs);
                    }
                }
            }
            
            // 所有重试失败后
            Console.Error.WriteLine($"Anthropic API调用错误: {lastException?.Message}");
            return $"Anthropic API调用失败: {lastException?.Message}";
        }

        /// <summary>
        /// 流式发送聊天消息并接收响应
        /// </summary>
        public override async IAsyncEnumerable<string> StreamChatAsync(
            List<ChatMessage> messages, 
            string modelId,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                modelId = "claude-3-sonnet"; // 默认使用claude-3-sonnet模型
            }

            // 收集的完整内容
            var contentChunks = new List<string>();
            bool hasError = false;
            string errorMessage = string.Empty;
            
            // 在此块中处理请求和收集数据，不使用yield return
            await Task.Run(async () => 
            {
                HttpResponseMessage response = null;
                Stream responseStream = null;
                StreamReader reader = null;
                
                try
                {
                    string apiUrl = $"{ApiHost}/v1/messages";
                    
                    // 提取系统提示
                    string systemPrompt = "";
                    foreach (var msg in messages)
                    {
                        if (msg.Role == ChatRole.System)
                        {
                            systemPrompt = msg.Content;
                            break;
                        }
                    }
                    
                    // 构建消息数组 (Anthropic格式)
                    var anthropicMessages = BuildMessages(messages);
    
                    // 构建请求体
                    var requestBody = new
                    {
                        model = modelId,
                        messages = anthropicMessages,
                        system = string.IsNullOrEmpty(systemPrompt) ? null : systemPrompt,
                        temperature = 0.7,
                        max_tokens = 4096,
                        stream = true
                    };
    
                    // 转换为JSON
                    string requestJson = JsonSerializer.Serialize(requestBody,
                        new JsonSerializerOptions { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull });
                    
                    // 创建请求
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    
                    // 添加Anthropic API头
                    request.Headers.Add("x-api-key", ApiKey);
                    request.Headers.Add("anthropic-version", _apiVersion);
                    
                    // 发送请求并获取流式响应
                    response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    
                    responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    reader = new StreamReader(responseStream);
                    
                    // 处理流式响应
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null && !cancellationToken.IsCancellationRequested)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                            
                        if (line.StartsWith("data: "))
                        {
                            string data = line.Substring(6);
                            
                            // 检查是否是结束标记
                            if (data == "[DONE]")
                                break;
                            
                            // Anthropic流式响应格式可能与OpenAI不同
                            try
                            {
                                using JsonDocument doc = JsonDocument.Parse(data);
                                
                                if (doc.RootElement.TryGetProperty("type", out var type) && 
                                    type.GetString() == "content_block_delta" &&
                                    doc.RootElement.TryGetProperty("delta", out var delta) &&
                                    delta.TryGetProperty("text", out var text))
                                {
                                    string chunk = text.GetString() ?? string.Empty;
                                    if (!string.IsNullOrEmpty(chunk))
                                    {
                                        contentChunks.Add(chunk);
                                    }
                                }
                            }
                            catch (JsonException)
                            {
                                // 可能是不完整的JSON，忽略
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Anthropic流式API调用错误: {ex.Message}");
                    hasError = true;
                    errorMessage = $"Anthropic流式API调用失败: {ex.Message}";
                }
                finally
                {
                    reader?.Dispose();
                    responseStream?.Dispose();
                    response?.Dispose();
                }
            }, cancellationToken);
            
            // 在收集所有内容后，再使用yield return返回
            foreach (var chunk in contentChunks)
            {
                yield return chunk;
            }
            
            // 如果有错误，在最后返回错误信息
            if (hasError)
            {
                yield return errorMessage;
            }
        }

        /// <summary>
        /// 获取提供商类型
        /// </summary>
        public override ProviderType GetProviderType()
        {
            return ProviderType.Anthropic;
        }
        
        /// <summary>
        /// 设置API版本
        /// </summary>
        public void SetApiVersion(string apiVersion)
        {
            _apiVersion = apiVersion;
        }
        
        /// <summary>
        /// 从API获取Anthropic支持的模型列表
        /// </summary>
        /// <returns>可用模型ID列表</returns>
        public async Task<List<string>> GetModelsFromApiAsync()
        {
            try
            {
                // Anthropic API端点
                string apiUrl = $"{ApiHost}/v1/models";
                
                // 创建请求
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                
                // 添加Anthropic授权和版本头
                request.Headers.Add("x-api-key", ApiKey);
                request.Headers.Add("anthropic-version", _apiVersion);
                
                // 发送请求
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                // 解析响应
                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                
                var models = new List<string>();
                
                // 解析Anthropic API返回的模型列表
                if (doc.RootElement.TryGetProperty("data", out var data))
                {
                    foreach (var model in data.EnumerateArray())
                    {
                        if (model.TryGetProperty("id", out var id))
                        {
                            string modelId = id.GetString() ?? string.Empty;
                            if (!string.IsNullOrEmpty(modelId))
                            {
                                models.Add(modelId);
                            }
                        }
                    }
                }
                
                // 如果models为空，说明解析失败或没有找到model字段
                if (models.Count == 0)
                {
                    Console.Error.WriteLine("Anthropic API返回的模型列表为空或格式不符合预期");
                }
                
                return models;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"获取Anthropic模型列表失败: {ex.Message}");
                // 返回空列表，而不是默认的模型列表
                return new List<string>();
            }
        }
        
        /// <summary>
        /// 获取可用的模型列表
        /// </summary>
        public override List<string> GetAvailableModels()
        {
            // 使用父类方法从数据库获取已启用的模型
            return base.GetAvailableModels();
        }
    }
} 