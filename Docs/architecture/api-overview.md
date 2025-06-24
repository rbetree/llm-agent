# API参考

本文档提供了LLM Agent应用程序的API参考信息，包括各种模型提供商的接口规范和使用方法。

## 概述

LLM Agent支持多种大语言模型服务提供商的API，通过统一的抽象类，简化了与不同模型的交互。所有提供商实现都基于`BaseLLMProvider`抽象类，确保一致的使用体验。

## 核心接口

### BaseLLMProvider

所有模型提供商必须继承的基础抽象类：

```csharp
public abstract class BaseLLMProvider
{
    protected string ApiKey { get; set; }
    protected string ApiHost { get; set; }
    protected string CurrentModel { get; set; }

    // 抽象方法，子类必须实现
    public abstract List<string> GetSupportedModels();
    public abstract Task<string> ChatAsync(List<ChatMessage> messages, string modelId);
    public virtual async IAsyncEnumerable<string> StreamChatAsync(List<ChatMessage> messages, string modelId, CancellationToken cancellationToken = default);

    // 通用方法
    public virtual void UpdateApiKey(string apiKey);
    public virtual void UpdateApiHost(string apiHost);
    public abstract ProviderType GetProviderType();
}
```



## 支持的提供商

LLM Agent支持以下模型提供商的API：

### 原生支持的提供商
1. **OpenAI** - 支持GPT系列模型
2. **Azure OpenAI** - 企业级OpenAI服务
3. **Anthropic Claude** - 支持Claude系列模型
4. **Google Gemini** - 支持Gemini系列模型

### OpenAI兼容服务
通过OpenAI提供商类型支持的兼容服务：
- **SiliconFlow** - 支持Claude、Yi-Large、Mistral等模型
- **智谱AI** - 支持GLM系列模型
- **DeepSeek** - 支持DeepSeek系列模型
- **其他兼容OpenAI格式的服务**

每个提供商都有特定的API参数和配置选项。

## 使用示例

### 基本使用

```csharp
// 获取提供商实例
var providerFactory = new ProviderFactory();
var provider = providerFactory.GetProvider(ProviderType.OpenAI);

// 设置API密钥和主机
provider.UpdateApiKey("your-api-key");
provider.UpdateApiHost("https://api.openai.com/v1");

// 获取支持的模型
var models = provider.GetSupportedModels();

// 构建消息列表
var messages = new List<ChatMessage>
{
    new ChatMessage { Role = ChatRole.User, Content = "你好！" }
};

// 发送消息
var response = await provider.ChatAsync(messages, models.First());

Console.WriteLine(response);
```

### 流式响应

```csharp
// 构建消息列表
var messages = new List<ChatMessage>
{
    new ChatMessage { Role = ChatRole.User, Content = "讲个故事" }
};

// 处理流式响应
await foreach (var chunk in provider.StreamChatAsync(messages, models.First()))
{
    // 处理部分响应
    Console.Write(chunk);
}
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
- [SiliconFlow API文档](https://siliconflow.cn/docs/api)