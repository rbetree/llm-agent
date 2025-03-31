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
using System.Text.Json.Serialization;

namespace llm_agent.API.Provider
{
    /// <summary>
    /// OpenAI API提供商实现
    /// </summary>
    public class OpenAIProvider : BaseLLMProvider
    {
        private readonly HttpClient _httpClient;
        private int _maxRetries = 3;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OpenAIProvider(HttpClient httpClient, string apiKey = "", string apiHost = "")
            : base(apiKey, apiHost ?? "https://api.openai.com/v1")
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
                "gpt-4",
                "gpt-4-turbo",
                "gpt-4o",
                "gpt-3.5-turbo",
                "gpt-4-vision-preview"
            };
        }

        /// <summary>
        /// 发送聊天消息并接收响应
        /// </summary>
        public override async Task<string> ChatAsync(List<ChatMessage> messages, string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                modelId = "gpt-3.5-turbo"; // 默认使用gpt-3.5-turbo模型
            }

            int retries = 0;
            Exception? lastException = null;

            while (retries < _maxRetries)
            {
                try
                {
                    string apiUrl = $"{ApiHost}/chat/completions";
                    
                    // 构建消息数组
                    var chatMessages = new List<object>();
                    foreach (var msg in messages)
                    {
                        string role = msg.Role switch
                        {
                            ChatRole.User => "user",
                            ChatRole.Assistant => "assistant",
                            ChatRole.System => "system",
                            _ => "user"
                        };

                        chatMessages.Add(new
                        {
                            role = role,
                            content = msg.Content
                        });
                    }

                    // 构建请求体
                    var requestBody = new
                    {
                        model = modelId,
                        messages = chatMessages,
                        temperature = 0.7,
                        top_p = 0.9,
                        max_tokens = 4096,
                        frequency_penalty = 0.0,
                        presence_penalty = 0.0
                    };

                    // 转换为JSON
                    string requestJson = JsonSerializer.Serialize(requestBody);
                    
                    // 创建请求
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    
                    // 添加授权头
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
                    
                    // 发送请求
                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    
                    // 解析响应
                    string responseBody = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    
                    // OpenAI响应格式解析
                    if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                        choices.GetArrayLength() > 0 &&
                        choices[0].TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var content))
                    {
                        return content.GetString() ?? string.Empty;
                    }
                    
                    throw new Exception("无法解析OpenAI响应: " + responseBody);
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
            Console.Error.WriteLine($"OpenAI API调用错误: {lastException?.Message}");
            return $"OpenAI API调用失败: {lastException?.Message}";
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
                modelId = "gpt-3.5-turbo"; // 默认使用gpt-3.5-turbo模型
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
                    string apiUrl = $"{ApiHost}/chat/completions";
                    
                    // 构建消息数组
                    var chatMessages = new List<object>();
                    foreach (var msg in messages)
                    {
                        string role = msg.Role switch
                        {
                            ChatRole.User => "user",
                            ChatRole.Assistant => "assistant",
                            ChatRole.System => "system",
                            _ => "user"
                        };
    
                        chatMessages.Add(new
                        {
                            role = role,
                            content = msg.Content
                        });
                    }
    
                    // 构建请求体
                    var requestBody = new
                    {
                        model = modelId,
                        messages = chatMessages,
                        temperature = 0.7,
                        top_p = 0.9,
                        max_tokens = 4096,
                        frequency_penalty = 0.0,
                        presence_penalty = 0.0,
                        stream = true // 启用流式响应
                    };
    
                    // 转换为JSON
                    string requestJson = JsonSerializer.Serialize(requestBody);
                    
                    // 创建请求
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    
                    // 添加授权头
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
                    
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
                                
                            try
                            {
                                using JsonDocument doc = JsonDocument.Parse(data);
                                
                                if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                                    choices.GetArrayLength() > 0 &&
                                    choices[0].TryGetProperty("delta", out var delta) &&
                                    delta.TryGetProperty("content", out var content))
                                {
                                    string chunk = content.GetString() ?? string.Empty;
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
                    Console.Error.WriteLine($"OpenAI流式API调用错误: {ex.Message}");
                    hasError = true;
                    errorMessage = $"OpenAI流式API调用失败: {ex.Message}";
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
            return ProviderType.OpenAI;
        }
        
        /// <summary>
        /// 从API获取OpenAI支持的模型列表
        /// </summary>
        public async Task<List<string>> GetModelsFromApiAsync()
        {
            try
            {
                string apiUrl = $"{ApiHost}/models";
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
                
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                
                var models = new List<string>();
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
                
                // 如果models为空，说明解析失败或没有找到数据
                if (models.Count == 0)
                {
                    Console.Error.WriteLine("OpenAI API返回的模型列表为空或格式不符合预期");
                }
                
                return models;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"获取OpenAI模型列表失败: {ex.Message}");
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