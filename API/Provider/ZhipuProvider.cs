using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;
using llm_agent.Model;

namespace llm_agent.API.Provider
{
    /// <summary>
    /// 智谱AI提供商实现
    /// </summary>
    public class ZhipuProvider : BaseLLMProvider
    {
        private readonly HttpClient _httpClient;
        private string _apiSecret = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ZhipuProvider(HttpClient httpClient, string apiKey = "", string apiHost = "")
            : base(apiKey, apiHost ?? "https://open.bigmodel.cn/api/paas/v4")
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
                "GLM-4", 
                "GLM-4-Flash",
                "GLM-4-Air",
                "GLM-3-Turbo",
                "GLM-4V"
            };
        }

        /// <summary>
        /// 发送聊天消息并接收响应
        /// </summary>
        public override async Task<string> ChatAsync(List<ChatMessage> messages, string modelId)
        {
            if (string.IsNullOrEmpty(modelId))
            {
                modelId = "GLM-4"; // 默认使用GLM-4模型
            }

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
                    model = modelId.ToLower(), // 智谱AI的API要求模型ID小写
                    messages = chatMessages,
                    temperature = 0.7, // 可以从设置中获取这些参数
                    top_p = 0.8,
                    max_tokens = 4096
                };

                // 转换为JSON
                string requestJson = JsonSerializer.Serialize(requestBody);
                
                // 创建请求
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                
                // 添加授权头
                string jwtToken = GenerateJwtToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                
                // 发送请求
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                // 解析响应
                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                
                // 智谱AI响应格式解析
                if (doc.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content))
                {
                    return content.GetString() ?? string.Empty;
                }
                
                throw new Exception("无法解析智谱AI响应");
            }
            catch (Exception ex)
            {
                // 记录异常并返回错误消息
                Console.Error.WriteLine($"智谱AI API调用错误: {ex.Message}");
                return $"智谱AI调用失败: {ex.Message}";
            }
        }

        /// <summary>
        /// 获取提供商类型
        /// </summary>
        public override ProviderType GetProviderType()
        {
            return ProviderType.ZhipuAI; // 返回智谱AI类型
        }

        /// <summary>
        /// 重写获取API密钥方法，增加设置_apiSecret逻辑
        /// </summary>
        public override void UpdateApiKey(string apiKey)
        {
            _apiSecret = apiKey ?? string.Empty;
            base.UpdateApiKey(apiKey);
        }

        /// <summary>
        /// 从API获取智谱AI支持的模型列表
        /// </summary>
        public async Task<List<string>> GetModelsFromApiAsync()
        {
            try
            {
                // 智谱API端点获取模型列表
                string apiUrl = $"{ApiHost}/models";
                
                // 创建请求
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                
                // 添加JWT授权头
                string jwtToken = GenerateJwtToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                
                // 发送请求
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                // 解析响应
                string responseBody = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseBody);
                
                var models = new List<string>();
                
                // 解析智谱AI API返回的模型列表
                if (doc.RootElement.TryGetProperty("data", out var data) && 
                    data.TryGetProperty("models", out var modelArray))
                {
                    foreach (var model in modelArray.EnumerateArray())
                    {
                        if (model.TryGetProperty("id", out var id))
                        {
                            string modelId = id.GetString() ?? string.Empty;
                            if (!string.IsNullOrEmpty(modelId))
                            {
                                // 统一使用大写形式的模型ID
                                models.Add(modelId.ToUpper());
                            }
                        }
                    }
                }
                
                // 如果models为空，说明解析失败或没有找到model字段
                if (models.Count == 0)
                {
                    Console.Error.WriteLine("智谱AI API返回的模型列表为空或格式不符合预期");
                }
                
                return models;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"获取智谱AI模型列表失败: {ex.Message}");
                // 出错时返回空列表，而不是默认的模型列表
                return new List<string>();
            }
        }

        /// <summary>
        /// 生成JWT令牌用于API认证
        /// </summary>
        private string GenerateJwtToken()
        {
            try
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string expireTime = (timestamp + 3600).ToString(); // 令牌有效期1小时

                // JWT Header
                var header = new
                {
                    alg = "HS256",
                    sign_type = "SIGN"
                };
                string headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(header)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');

                // JWT Payload
                var payload = new
                {
                    api_key = ApiKey,
                    exp = expireTime,
                    timestamp = timestamp.ToString()
                };
                string payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)))
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');

                // 签名
                string signatureBase = $"{headerBase64}.{payloadBase64}";
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_apiSecret));
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signatureBase));
                string signature = Convert.ToBase64String(signatureBytes)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');

                // 完整JWT
                return $"{headerBase64}.{payloadBase64}.{signature}";
            }
            catch (Exception ex)
            {
                throw new Exception($"生成JWT令牌失败: {ex.Message}");
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
                modelId = "GLM-4"; // 默认使用GLM-4模型
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
                        model = modelId.ToLower(), // 智谱AI的API要求模型ID小写
                        messages = chatMessages,
                        temperature = 0.7,
                        top_p = 0.8,
                        max_tokens = 4096,
                        stream = true // 启用流式响应
                    };
    
                    // 转换为JSON
                    string requestJson = JsonSerializer.Serialize(requestBody);
                    
                    // 创建请求
                    string apiUrl = $"{ApiHost}/chat/completions";
                    var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
                    request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    
                    // 添加授权头
                    string jwtToken = GenerateJwtToken();
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    
                    // 发送请求
                    response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    
                    // 获取响应流
                    responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    reader = new StreamReader(responseStream);
                    
                    // 处理SSE流
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null && !cancellationToken.IsCancellationRequested)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                            continue;
                            
                        if (line.StartsWith("data: "))
                        {
                            string data = line.Substring(6);
                            
                            // 检查是否是结束标记
                            if (data.Contains("[DONE]"))
                                break;
                                
                            try
                            {
                                var jsonData = JsonDocument.Parse(data);
                                
                                if (jsonData.RootElement.TryGetProperty("choices", out var choices) &&
                                    choices.GetArrayLength() > 0 &&
                                    choices[0].TryGetProperty("delta", out var delta) &&
                                    delta.TryGetProperty("content", out var content))
                                {
                                    string chunk = content.GetString();
                                    if (!string.IsNullOrEmpty(chunk))
                                    {
                                        contentChunks.Add(chunk);
                                    }
                                }
                            }
                            catch (JsonException)
                            {
                                // 忽略解析错误，继续下一行
                                continue;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 错误处理
                    Console.Error.WriteLine($"智谱AI流式调用失败: {ex.Message}");
                    hasError = true;
                    errorMessage = $"智谱AI流式调用失败: {ex.Message}";
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
    }
} 