# 模型接口

本页面详细说明LLM多模型客户端中的模型定义和参数。

## 模型定义

### ModelInfo

`ModelInfo`类表示一个LLM模型的基本信息：

```csharp
public class ModelInfo
{
    // 模型ID
    public string Id { get; set; }
    
    // 模型名称
    public string Name { get; set; }
    
    // 模型描述
    public string Description { get; set; }
    
    // 最大令牌数
    public int MaxTokens { get; set; }
    
    // 是否支持流式响应
    public bool SupportsStreaming { get; set; } = true;
    
    // 是否支持系统提示
    public bool SupportsSystemPrompt { get; set; } = true;
    
    // 模型提供商类型
    public ProviderType ProviderType { get; set; }
    
    // 模型能力
    public ModelCapabilities Capabilities { get; set; }
}
```

### ModelCapabilities

`ModelCapabilities`枚举定义了模型的能力：

```csharp
[Flags]
public enum ModelCapabilities
{
    None = 0,
    TextGeneration = 1,
    ChatCompletion = 2,
    ImageGeneration = 4,
    CodeCompletion = 8,
    FunctionCalling = 16,
    All = TextGeneration | ChatCompletion | ImageGeneration | CodeCompletion | FunctionCalling
}
```

### ProviderType

`ProviderType`枚举定义了支持的LLM服务提供商类型：

```csharp
public enum ProviderType
{
    OpenAI,
    Anthropic,
    Google,
    Baidu,
    ZhipuAI,
    Custom
}
```

## 消息模型

### ChatMessage

`ChatMessage`类表示聊天对话中的一条消息：

```csharp
public class ChatMessage
{
    // 消息角色
    public ChatRole Role { get; set; }
    
    // 消息内容
    public string Content { get; set; }
    
    // 消息ID
    public string Id { get; set; }
    
    // 消息创建时间
    public DateTime CreatedAt { get; set; }
    
    // 构造函数
    public ChatMessage(ChatRole role, string content)
    {
        Role = role;
        Content = content;
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.Now;
    }
}
```

### ChatRole

`ChatRole`枚举定义了聊天消息的角色类型：

```csharp
public enum ChatRole
{
    System,
    User,
    Assistant,
    Function
}
```

### ChatResponseChunk

`ChatResponseChunk`类表示流式响应中的一个数据块：

```csharp
public class ChatResponseChunk
{
    // 块ID
    public string Id { get; set; }
    
    // 块内容
    public string Content { get; set; }
    
    // 是否是最后一个块
    public bool IsLast { get; set; }
    
    // 完成原因（仅在最后一个块中有效）
    public string FinishReason { get; set; }
    
    // 使用的模型
    public string Model { get; set; }
    
    // 使用的令牌数（仅在最后一个块中有效）
    public TokenUsage TokenUsage { get; set; }
}
```

### TokenUsage

`TokenUsage`类表示令牌使用情况：

```csharp
public class TokenUsage
{
    // 提示令牌数
    public int PromptTokens { get; set; }
    
    // 完成令牌数
    public int CompletionTokens { get; set; }
    
    // 总令牌数
    public int TotalTokens => PromptTokens + CompletionTokens;
}
```

## 配置模型

### ProviderConfig

`ProviderConfig`类表示LLM服务提供商的配置：

```csharp
public class ProviderConfig
{
    // API密钥
    public string ApiKey { get; set; }
    
    // API密钥2（部分提供商需要，如百度的Secret Key）
    public string ApiKey2 { get; set; }
    
    // 组织ID（部分提供商需要，如OpenAI）
    public string OrganizationId { get; set; }
    
    // API基础URL
    public string BaseUrl { get; set; }
    
    // 是否使用自定义API服务器
    public bool UseCustomServer { get; set; }
    
    // 自定义API服务器地址
    public string CustomServerUrl { get; set; }
    
    // 代理设置
    public ProxySettings ProxySettings { get; set; }
    
    // 超时设置（毫秒）
    public int TimeoutMs { get; set; } = 30000;
}
```

### ProxySettings

`ProxySettings`类表示代理服务器设置：

```csharp
public class ProxySettings
{
    // 是否使用代理
    public bool UseProxy { get; set; }
    
    // 代理服务器地址
    public string ProxyServer { get; set; }
    
    // 代理服务器端口
    public int ProxyPort { get; set; }
    
    // 是否需要认证
    public bool RequiresAuthentication { get; set; }
    
    // 代理用户名
    public string Username { get; set; }
    
    // 代理密码
    public string Password { get; set; }
}
```

## 预定义模型

应用预定义了各个服务提供商的常用模型，这些定义位于`Models.cs`文件中：

### OpenAI模型

```csharp
public static class OpenAIModels
{
    public static readonly ModelInfo GPT35Turbo = new ModelInfo
    {
        Id = "gpt-3.5-turbo",
        Name = "GPT-3.5 Turbo",
        Description = "最适合大多数任务的平衡模型",
        MaxTokens = 4096,
        ProviderType = ProviderType.OpenAI,
        Capabilities = ModelCapabilities.ChatCompletion | ModelCapabilities.FunctionCalling
    };
    
    public static readonly ModelInfo GPT4 = new ModelInfo
    {
        Id = "gpt-4",
        Name = "GPT-4",
        Description = "更强大的模型，适合复杂任务",
        MaxTokens = 8192,
        ProviderType = ProviderType.OpenAI,
        Capabilities = ModelCapabilities.All
    };
    
    // 其他OpenAI模型...
}
```

### Anthropic模型

```csharp
public static class AnthropicModels
{
    public static readonly ModelInfo Claude3Haiku = new ModelInfo
    {
        Id = "claude-3-haiku-20240307",
        Name = "Claude 3 Haiku",
        Description = "最快速的Claude模型",
        MaxTokens = 200000,
        ProviderType = ProviderType.Anthropic,
        Capabilities = ModelCapabilities.ChatCompletion
    };
    
    public static readonly ModelInfo Claude3Sonnet = new ModelInfo
    {
        Id = "claude-3-sonnet-20240229",
        Name = "Claude 3 Sonnet",
        Description = "平衡速度和智能的Claude模型",
        MaxTokens = 200000,
        ProviderType = ProviderType.Anthropic,
        Capabilities = ModelCapabilities.ChatCompletion
    };
    
    // 其他Anthropic模型...
}
```

## 自定义模型

您可以通过扩展`Models.cs`文件来添加自定义模型定义：

```csharp
public static class CustomModels
{
    public static readonly ModelInfo MyCustomModel = new ModelInfo
    {
        Id = "my-custom-model",
        Name = "My Custom Model",
        Description = "我的自定义模型",
        MaxTokens = 4096,
        ProviderType = ProviderType.Custom,
        Capabilities = ModelCapabilities.ChatCompletion
    };
}
```

然后，您需要在自定义的LLM提供商实现中返回这些模型：

```csharp
public override List<ModelInfo> GetSupportedModels()
{
    return new List<ModelInfo>
    {
        CustomModels.MyCustomModel
    };
} 