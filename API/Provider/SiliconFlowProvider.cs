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
    /// 硅基流动 API提供商实现（使用OpenAI格式）
    /// </summary>
    public class SiliconFlowProvider : BaseLLMProvider
    {
        private readonly HttpClient _httpClient;
        private int _maxRetries = 3;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SiliconFlowProvider(HttpClient httpClient, string apiKey = "", string apiHost = "")
            : base(apiKey, apiHost ?? "https://api.siliconflow.cn/v1")
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
                "claude-3-opus-20240229",
                "claude-3-sonnet-20240229",
                "claude-3-haiku-20240307",
                "yi-large",
                "yi-large-chat",
                "mistral-large-latest",
                "mistral-medium-latest"
            };
        }

        /// <summary>
        /// 发送聊天消息并接收响应
        /// </summary>
        public override async Task<string> ChatAsync(List<ChatMessage> messages, string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                modelId = "claude-3-sonnet-20240229"; // 默认使用claude-3-sonnet模型
            }

            int retries = 0;
            Exception? lastException = null;

            while (retries < _maxRetries)
            {
                try
                {
                    // 确保ApiHost是完整的URL
                    if (string.IsNullOrEmpty(ApiHost))
                    {
                        ApiHost = "https://api.siliconflow.cn/v1";
                    }
                    
                    // 构建完整的绝对URI
                    Uri baseUri = new Uri(ApiHost);
                    Uri absoluteUri = new Uri(baseUri, "chat/completions");
                    string apiUrl = absoluteUri.ToString();
                    
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
                    
                    // OpenAI格式响应解析
                    if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                        choices.GetArrayLength() > 0 &&
                        choices[0].TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var content))
                    {
                        return content.GetString() ?? string.Empty;
                    }
                    
                    throw new Exception("无法解析硅基流动响应: " + responseBody);
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
            Console.Error.WriteLine($"硅基流动 API调用错误: {lastException?.Message}");
            return $"硅基流动 API调用失败: {lastException?.Message}";
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
                modelId = "claude-3-sonnet-20240229"; // 默认模型
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
                    // 确保ApiHost是完整的URL
                    if (string.IsNullOrEmpty(ApiHost))
                    {
                        ApiHost = "https://api.siliconflow.cn/v1";
                    }
                    
                    // 构建完整的绝对URI
                    Uri baseUri = new Uri(ApiHost);
                    Uri absoluteUri = new Uri(baseUri, "chat/completions");
                    string apiUrl = absoluteUri.ToString();
                    
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
                                    string contentChunk = content.GetString() ?? string.Empty;
                                    contentChunks.Add(contentChunk);
                                }
                            }
                            catch (JsonException ex)
                            {
                                Console.Error.WriteLine($"解析流式JSON错误: {ex.Message}, 数据: {data}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    hasError = true;
                    errorMessage = ex.Message;
                    Console.Error.WriteLine($"硅基流动 API流式调用错误: {ex.Message}");
                }
                finally
                {
                    reader?.Dispose();
                    responseStream?.Dispose();
                    response?.Dispose();
                }
            }, cancellationToken);
            
            // 处理错误
            if (hasError)
            {
                yield return $"硅基流动 API调用失败: {errorMessage}";
                yield break;
            }
            
            // 返回收集的内容块
            foreach (var chunk in contentChunks)
            {
                yield return chunk;
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
        /// 从API获取可用模型列表
        /// </summary>
        public async Task<List<string>> GetModelsFromApiAsync()
        {
            try
            {
                // 确保ApiHost是完整的URL
                if (string.IsNullOrEmpty(ApiHost))
                {
                    ApiHost = "https://api.siliconflow.cn/v1";
                }
                
                // 构建完整的绝对URI
                Uri baseUri = new Uri(ApiHost);
                Uri absoluteUri = new Uri(baseUri, "models");
                string apiUrl = absoluteUri.ToString();
                
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
                
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("data", out var data))
                {
                    var modelIds = new List<string>();
                    
                    foreach (var model in data.EnumerateArray())
                    {
                        if (model.TryGetProperty("id", out var id))
                        {
                            modelIds.Add(id.GetString());
                        }
                    }
                    
                    return modelIds;
                }
                
                // 如果代码执行到这里，说明API返回格式与预期不符
                Console.Error.WriteLine("硅基流动API返回的格式不符合预期，没有找到'data'字段");
                return new List<string>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"从硅基流动API获取模型列表失败: {ex.Message}");
                // 返回空列表，而不是默认支持的模型列表
                return new List<string>();
            }
        }
        
        /// <summary>
        /// 获取可用的模型列表
        /// </summary>
        public override List<string> GetAvailableModels()
        {
            try
            {
                // 首先尝试从数据库获取
                var dbModels = base.GetAvailableModels();
                
                // 如果数据库已有记录，直接返回
                if (dbModels.Count > 0 && dbModels != GetSupportedModels())
                {
                    return dbModels;
                }
                
                // 否则从API获取
                var task = GetModelsFromApiAsync();
                task.Wait();
                return task.Result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"获取可用模型列表错误: {ex.Message}");
                return GetSupportedModels();
            }
        }
    }
} 