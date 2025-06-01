# API参考

本文档提供了LLM Agent应用程序的API参考信息，包括各种模型提供商的接口规范和使用方法。

## 概述

LLM Agent支持多种大语言模型服务提供商的API，通过统一的接口抽象，简化了与不同模型的交互。所有提供商实现都基于`ILLMProvider`接口，确保一致的使用体验。

## 核心接口

### ILLMProvider

所有模型提供商必须实现的核心接口：

```csharp
public interface ILLMProvider
{
    string Name { get; }
    ProviderType ProviderType { get; }
    
    Task<List<ModelInfo>> GetAvailableModelsAsync();
    Task<ChatMessage> SendMessageAsync(ChatMessage message, ChatSession session, ModelInfo model, bool useStream = false);
    Task<ChatMessage> SendMessageStreamAsync(ChatMessage message, ChatSession session, ModelInfo model, Action<string> onPartialResponse);
    Task<bool> TestConnectionAsync();
}
```

### BaseLLMProvider

提供基本实现的抽象类：

```csharp
public abstract class BaseLLMProvider : ILLMProvider
{
    protected string ApiKey { get; }
    protected string ApiHost { get; }
    
    // 基本实现...
    
    // 子类必须实现的抽象方法
    protected abstract Task<ChatMessage> SendMessageInternalAsync(ChatMessage message, ChatSession session, ModelInfo model);
    protected abstract Task<ChatMessage> SendMessageStreamInternalAsync(ChatMessage message, ChatSession session, ModelInfo model, Action<string> onPartialResponse);
    protected abstract Task<bool> TestConnectionInternalAsync();
}
```

## 支持的提供商

LLM Agent支持以下模型提供商的API：

1. [OpenAI](./models.md#openai)
2. [Azure OpenAI](./models.md#azure-openai)
3. [Anthropic Claude](./models.md#anthropic-claude)
4. [Google Gemini](./models.md#google-gemini)
5. [ZhipuAI](./models.md#zhipuai)
6. [SiliconFlow](./models.md#siliconflow)

每个提供商都有特定的API参数和配置选项，详情请参阅[模型接口](./models.md)文档。

## 使用示例

### 基本使用

```csharp
// 获取提供商实例
var providerFactory = new ProviderFactory();
var provider = providerFactory.GetProvider(ProviderType.OpenAI, apiKey, apiHost);

// 获取可用模型
var models = await provider.GetAvailableModelsAsync();

// 发送消息
var response = await provider.SendMessageAsync(
    new ChatMessage { Role = "user", Content = "你好！" },
    new ChatSession { Id = Guid.NewGuid().ToString() },
    models.First(),
    useStream: false
);

Console.WriteLine(response.Content);
```

### 流式响应

```csharp
// 处理流式响应
await provider.SendMessageStreamAsync(
    new ChatMessage { Role = "user", Content = "讲个故事" },
    new ChatSession { Id = Guid.NewGuid().ToString() },
    models.First(),
    partialResponse => {
        // 处理部分响应
        Console.Write(partialResponse);
    }
);
```

## 错误处理

API调用可能会遇到各种错误，包括：

1. 认证错误 - API密钥无效
2. 请求限制 - 超出API速率限制
3. 模型错误 - 请求的模型不可用
4. 网络错误 - 连接问题

所有API错误都封装在`ApiException`类中，包含详细的错误信息：

```csharp
try
{
    var response = await provider.SendMessageAsync(...);
}
catch (ApiException ex)
{
    Console.WriteLine($"API错误: {ex.Message}");
    Console.WriteLine($"错误代码: {ex.ErrorCode}");
    Console.WriteLine($"提供商: {ex.ProviderType}");
}
```

## 自定义提供商

如果需要添加新的模型提供商，可以通过以下步骤实现：

1. 创建新的提供商类，继承`BaseLLMProvider`
2. 实现所有必要的抽象方法
3. 在`ProviderFactory`中注册新提供商
4. 添加相应的UI配置支持

详细实现指南请参阅[指南首页](../guide/)。

## API限制和最佳实践

使用LLM API时，请注意以下最佳实践：

1. **合理管理API密钥** - 不要在代码中硬编码API密钥
2. **实现速率限制** - 遵循提供商的API使用限制
3. **错误重试** - 对临时错误实现指数退避重试
4. **监控使用情况** - 跟踪API调用和成本
5. **缓存响应** - 对频繁查询的内容实现缓存

## 参考资源

- [OpenAI API文档](https://platform.openai.com/docs/api-reference)
- [Anthropic API文档](https://docs.anthropic.com/claude/reference/getting-started-with-the-api)
- [Google Gemini API文档](https://ai.google.dev/docs/gemini_api_overview)
- [ZhipuAI API文档](https://open.bigmodel.cn/dev/api) 