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
    /// Google Gemini API提供商实现
    /// </summary>
    public class GeminiProvider : BaseLLMProvider
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GeminiProvider(HttpClient httpClient, string apiKey = "", string apiHost = "")
            : base(apiKey, apiHost ?? "https://generativelanguage.googleapis.com/v1")
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
                "gemini-1.5-pro",
                "gemini-1.5-flash",
                "gemini-1.0-pro"
            };
        }

        /// <summary>
        /// 根据消息列表构造Gemini API请求内容
        /// </summary>
        private List<object> BuildContentsFromMessages(List<ChatMessage> messages)
        {
            var contents = new List<object>();
            string systemPrompt = "";

            // 首先找出系统消息
            foreach (var msg in messages)
            {
                if (msg.Role == ChatRole.System)
                {
                    systemPrompt = msg.Content;
                    break;
                }
            }

            // 然后添加用户和助手消息
            foreach (var msg in messages)
            {
                if (msg.Role == ChatRole.System)
                    continue; // 跳过系统消息，我们会在第一个用户消息中处理

                // Gemini API 角色映射
                string role = msg.Role switch
                {
                    ChatRole.User => "user",
                    ChatRole.Assistant => "model",
                    _ => "user"
                };

                // 对于第一个用户消息，添加系统提示作为前缀（如果存在）
                string content = msg.Content;
                if (!string.IsNullOrEmpty(systemPrompt) && msg.Role == ChatRole.User && 
                    !contents.Any(c => c.GetType().GetProperty("role")?.GetValue(c)?.ToString() == "user"))
                {
                    content = $"[System Instructions: {systemPrompt}]\n\n{content}";
                }

                contents.Add(new
                {
                    role = role,
                    parts = new[]
                    {
                        new { text = content }
                    }
                });
            }

            return contents;
        }

        /// <summary>
        /// 发送聊天消息并接收响应
        /// </summary>
        public override async Task<string> ChatAsync(List<ChatMessage> messages, string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                modelId = "gemini-1.5-pro"; // 默认使用gemini-1.5-pro模型
            }

            try
            {
                string apiUrl = $"{ApiHost}/models/{modelId}:generateContent?key={ApiKey}";
                
                // 构建消息数组
                var contents = BuildContentsFromMessages(messages);

                // 构建请求体
                var requestBody = new
                {
                    contents = contents,
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topP = 0.8,
                        topK = 40,
                        maxOutputTokens = 2048
                    }
                };

                // 转换为JSON
                string requestJson = JsonSerializer.Serialize(requestBody);
                
                // 创建请求
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                
                // 发送请求
                var response = await _httpClient.SendAsync(request);
                
                // 检查响应状态码
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"状态码: {response.StatusCode}, 错误: {errorContent}");
                }
                
                // 解析响应
                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                
                // Gemini API响应格式解析
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                    candidates.GetArrayLength() > 0 &&
                    candidates[0].TryGetProperty("content", out var content) &&
                    content.TryGetProperty("parts", out var parts) &&
                    parts.GetArrayLength() > 0 &&
                    parts[0].TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? string.Empty;
                }
                
                throw new Exception("无法解析Gemini API响应: " + responseBody);
            }
            catch (Exception ex)
            {
                // 记录异常并返回错误消息
                Console.Error.WriteLine($"Gemini API调用错误: {ex.Message}");
                return $"Gemini API调用失败: {ex.Message}";
            }
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
                modelId = "gemini-1.5-pro"; // 默认使用gemini-1.5-pro模型
            }

            string errorMessage = null;
            var responseChunks = new List<string>();

            try
            {
                // 构建消息数组
                var contents = BuildContentsFromMessages(messages);

                // 构建请求体 - 不用stream=true标志
                var requestBody = new
                {
                    contents = contents,
                    generationConfig = new
                    {
                        temperature = 0.7,
                        topP = 0.8,
                        topK = 40,
                        maxOutputTokens = 2048
                    }
                };

                // 转换为JSON
                string requestJson = JsonSerializer.Serialize(requestBody);

                // 创建请求 - 流式端点
                string apiUrl = $"{ApiHost}/models/{modelId}:streamGenerateContent?key={ApiKey}";
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                
                // 设置Accept头
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // 发送请求
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                // 获取响应流
                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var reader = new StreamReader(responseStream);

                // 读取响应并按行解析
                string? line;
                var jsonBuffer = new StringBuilder();
                
                while (!cancellationToken.IsCancellationRequested && 
                       (line = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        // 空行表示一个完整的JSON对象结束
                        if (jsonBuffer.Length > 0)
                        {
                            string jsonData = jsonBuffer.ToString();
                            jsonBuffer.Clear();
                            
                            try 
                            {
                                using JsonDocument doc = JsonDocument.Parse(jsonData);
                                
                                // 解析Gemini特定的响应格式
                                if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                                    candidates.GetArrayLength() > 0)
                                {
                                    // 尝试获取增量内容
                                    if (candidates[0].TryGetProperty("content", out var content) &&
                                        content.TryGetProperty("parts", out var parts) &&
                                        parts.GetArrayLength() > 0 &&
                                        parts[0].TryGetProperty("text", out var text))
                                    {
                                        string chunk = text.GetString() ?? string.Empty;
                                        if (!string.IsNullOrEmpty(chunk))
                                        {
                                            responseChunks.Add(chunk);
                                        }
                                    }
                                    else if (candidates[0].TryGetProperty("delta", out var delta) &&
                                             delta.TryGetProperty("content", out var deltaContent) &&
                                             deltaContent.TryGetProperty("parts", out var deltaParts) &&
                                             deltaParts.GetArrayLength() > 0 &&
                                             deltaParts[0].TryGetProperty("text", out var deltaText))
                                    {
                                        string chunk = deltaText.GetString() ?? string.Empty;
                                        if (!string.IsNullOrEmpty(chunk))
                                        {
                                            responseChunks.Add(chunk);
                                        }
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                Console.Error.WriteLine($"JSON解析错误: {ex.Message} - 数据: {jsonData}");
                                continue;
                            }
                        }
                    }
                    else
                    {
                        // 添加行到JSON缓冲区
                        jsonBuffer.AppendLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Gemini流式调用错误: {ex.Message}");
                errorMessage = $"Gemini流式调用失败: {ex.Message}";
            }

            // 在try/catch之外返回结果
            foreach (var chunk in responseChunks)
            {
                yield return chunk;
            }

            // 如果有错误，最后返回错误信息
            if (!string.IsNullOrEmpty(errorMessage))
            {
                yield return errorMessage;
            }
        }

        /// <summary>
        /// 获取提供商类型
        /// </summary>
        public override ProviderType GetProviderType()
        {
            return ProviderType.Google; // 返回Google类型
        }
        
        /// <summary>
        /// 从API获取Google Gemini支持的模型列表
        /// </summary>
        public async Task<List<string>> GetModelsFromApiAsync()
        {
            try
            {
                // Gemini API端点获取模型列表
                string apiUrl = $"{ApiHost}/models?key={ApiKey}";
                
                // 创建请求
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                
                // 发送请求
                var response = await _httpClient.SendAsync(request);
                
                // 检查响应状态码
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"获取Gemini模型列表失败，状态码: {response.StatusCode}, 错误: {errorContent}");
                }
                
                // 解析响应
                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                
                var models = new List<string>();
                
                // 解析Google API返回的模型列表
                if (doc.RootElement.TryGetProperty("models", out var modelArray))
                {
                    foreach (var model in modelArray.EnumerateArray())
                    {
                        if (model.TryGetProperty("name", out var nameElement))
                        {
                            string fullName = nameElement.GetString() ?? string.Empty;
                            
                            // 提取模型ID - name格式通常是"models/gemini-1.5-pro"
                            if (!string.IsNullOrEmpty(fullName) && fullName.Contains("/"))
                            {
                                string[] parts = fullName.Split('/');
                                string modelId = parts[parts.Length - 1];
                                
                                // 确保是Gemini模型
                                if (modelId.StartsWith("gemini-"))
                                {
                                    models.Add(modelId);
                                }
                            }
                        }
                    }
                }
                
                // 如果models为空，说明解析失败或没有找到model字段
                if (models.Count == 0)
                {
                    Console.Error.WriteLine("Google Gemini API返回的模型列表为空或格式不符合预期");
                }
                
                return models;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"获取Google Gemini模型列表失败: {ex.Message}");
                // 出错时返回空列表，而不是默认的模型列表
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