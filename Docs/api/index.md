# API参考

本页面提供LLM多模型客户端的API参考文档，帮助开发者了解应用的API结构和扩展方法。

## API概述

LLM多模型客户端采用模块化设计，通过统一的API接口与各种大语言模型服务进行交互。核心API位于`API`命名空间下，主要包含以下几个部分：

- **Provider**: 模型提供商实现
- **Models**: 模型定义和参数
- **Interfaces**: 接口定义
- **Utils**: API工具类

## 核心接口

### ILLMProvider

`ILLMProvider`是所有模型提供商实现的基础接口，定义了与LLM服务交互的基本方法：

```csharp
public interface ILLMProvider
{
    // 获取提供商名称
    string GetProviderName();
    
    // 获取支持的模型列表
    List<ModelInfo> GetSupportedModels();
    
    // 发送聊天请求（非流式）
    Task<ChatResponse> SendChatRequestAsync(ChatRequest request, CancellationToken cancellationToken = default);
    
    // 发送聊天请求（流式）
    IAsyncEnumerable<ChatResponseChunk> SendChatRequestStreamAsync(ChatRequest request, CancellationToken cancellationToken = default);
    
    // 验证API密钥
    Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
}
```

### BaseLLMProvider

`BaseLLMProvider`是一个抽象基类，实现了`ILLMProvider`接口的通用功能，简化了新提供商的实现：

```csharp
public abstract class BaseLLMProvider : ILLMProvider
{
    // 提供商配置
    protected ProviderConfig Config { get; }
    
    // HTTP客户端工厂
    protected IHttpClientFactory HttpClientFactory { get; }
    
    // 构造函数
    protected BaseLLMProvider(ProviderConfig config, IHttpClientFactory httpClientFactory);
    
    // 获取提供商名称（抽象方法）
    public abstract string GetProviderName();
    
    // 获取支持的模型列表（抽象方法）
    public abstract List<ModelInfo> GetSupportedModels();
    
    // 发送聊天请求（抽象方法）
    public abstract Task<ChatResponse> SendChatRequestAsync(ChatRequest request, CancellationToken cancellationToken = default);
    
    // 发送聊天请求（流式，抽象方法）
    public abstract IAsyncEnumerable<ChatResponseChunk> SendChatRequestStreamAsync(ChatRequest request, CancellationToken cancellationToken = default);
    
    // 验证API密钥（抽象方法）
    public abstract Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    
    // 创建HTTP客户端（受保护的方法）
    protected HttpClient CreateHttpClient();
}
```

## 数据模型

### ChatRequest

`ChatRequest`类表示发送给LLM服务的聊天请求：

```csharp
public class ChatRequest
{
    // 模型ID
    public string ModelId { get; set; }
    
    // 消息列表
    public List<ChatMessage> Messages { get; set; }
    
    // 系统提示
    public string SystemPrompt { get; set; }
    
    // 温度参数（0.0-2.0）
    public float Temperature { get; set; } = 1.0f;
    
    // 最大令牌数
    public int? MaxTokens { get; set; }
    
    // 其他参数
    public Dictionary<string, object> AdditionalParameters { get; set; }
}
```

### ChatResponse

`ChatResponse`类表示LLM服务返回的聊天响应：

```csharp
public class ChatResponse
{
    // 响应ID
    public string Id { get; set; }
    
    // 响应内容
    public string Content { get; set; }
    
    // 使用的模型
    public string Model { get; set; }
    
    // 使用的令牌数
    public TokenUsage TokenUsage { get; set; }
    
    // 完成原因
    public string FinishReason { get; set; }
}
```

## 扩展LLM提供商

要添加新的LLM提供商，需要执行以下步骤：

1. 创建继承自`BaseLLMProvider`的新类
2. 实现所有必要的抽象方法
3. 在`Models.cs`中添加新提供商的模型定义
4. 在`ProviderFactory.cs`中添加新提供商的创建逻辑
5. 在`ProviderType`枚举中添加新提供商类型

示例：

```csharp
public class NewProvider : BaseLLMProvider
{
    public NewProvider(ProviderConfig config, IHttpClientFactory httpClientFactory)
        : base(config, httpClientFactory)
    {
    }

    public override string GetProviderName() => "NewProvider";

    public override List<ModelInfo> GetSupportedModels()
    {
        return new List<ModelInfo>
        {
            new ModelInfo { Id = "new-model-1", Name = "New Model 1", MaxTokens = 4096 },
            new ModelInfo { Id = "new-model-2", Name = "New Model 2", MaxTokens = 8192 }
        };
    }

    // 实现其他抽象方法...
}
```

## 更多API文档

- [模型接口](./models.md) - 详细了解模型定义和参数 