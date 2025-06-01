# 模型接口

本文档详细介绍了LLM Agent支持的各种模型提供商的接口规范和使用方法。

## OpenAI

### 支持的模型

OpenAI提供商支持以下模型：

| 模型名称 | 类别 | 上下文长度 | 说明 |
|---------|------|-----------|------|
| gpt-4 | 聊天 | 8,192 | GPT-4基础模型 |
| gpt-4-turbo | 聊天 | 128,000 | GPT-4 Turbo版本，支持更长上下文 |
| gpt-4-vision | 聊天+视觉 | 128,000 | 支持图像理解的GPT-4 |
| gpt-3.5-turbo | 聊天 | 16,384 | GPT-3.5 Turbo版本 |
| text-embedding-ada-002 | 嵌入 | 8,191 | 文本嵌入模型 |

### 配置参数

OpenAI提供商需要以下配置参数：

```csharp
// 创建OpenAI提供商
var openAIProvider = new OpenAIProvider(
    apiKey: "sk-...",  // OpenAI API密钥
    apiHost: "https://api.openai.com"  // 可选，默认为官方API地址
);
```

### 请求参数

发送消息时支持以下参数：

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| temperature | float | 0.7 | 控制生成文本的随机性，0-2之间 |
| max_tokens | int | 1000 | 生成文本的最大长度 |
| top_p | float | 1.0 | 控制词汇选择的多样性 |
| frequency_penalty | float | 0.0 | 减少重复内容的惩罚系数 |
| presence_penalty | float | 0.0 | 鼓励引入新主题的系数 |

### 示例代码

```csharp
// 创建提供商
var provider = new OpenAIProvider(apiKey, apiHost);

// 获取可用模型
var models = await provider.GetAvailableModelsAsync();
var gpt4Model = models.FirstOrDefault(m => m.Name == "gpt-4");

// 创建聊天会话
var session = new ChatSession { Id = Guid.NewGuid().ToString() };

// 发送消息
var message = new ChatMessage { 
    Role = "user", 
    Content = "用简单的语言解释量子计算" 
};

// 设置参数
var parameters = new Dictionary<string, object>
{
    ["temperature"] = 0.5,
    ["max_tokens"] = 500
};

// 发送请求
var response = await provider.SendMessageAsync(message, session, gpt4Model, parameters);
```

## Azure OpenAI

### 支持的模型

Azure OpenAI支持与OpenAI相同的模型，但需要在Azure平台上部署。

### 配置参数

Azure OpenAI提供商需要以下配置参数：

```csharp
// 创建Azure OpenAI提供商
var azureOpenAIProvider = new AzureOpenAIProvider(
    apiKey: "your-azure-api-key",
    apiHost: "https://your-resource-name.openai.azure.com",
    deploymentId: "your-deployment-id"  // Azure部署ID
);
```

### 请求参数

与OpenAI相同的参数，另外增加：

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| deployment_id | string | null | Azure部署ID，可在请求时覆盖配置值 |

## Anthropic Claude

### 支持的模型

Anthropic提供商支持以下Claude系列模型：

| 模型名称 | 类别 | 上下文长度 | 说明 |
|---------|------|-----------|------|
| claude-3-opus | 聊天 | 200,000 | Claude 3最强大版本 |
| claude-3-sonnet | 聊天 | 200,000 | Claude 3平衡版本 |
| claude-3-haiku | 聊天 | 200,000 | Claude 3快速版本 |
| claude-2 | 聊天 | 100,000 | Claude 2模型 |

### 配置参数

Anthropic提供商需要以下配置参数：

```csharp
// 创建Anthropic提供商
var anthropicProvider = new AnthropicProvider(
    apiKey: "sk-ant-...",  // Anthropic API密钥
    apiHost: "https://api.anthropic.com"  // 可选，默认为官方API地址
);
```

### 请求参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| temperature | float | 0.7 | 控制生成文本的随机性 |
| max_tokens | int | 1000 | 生成文本的最大长度 |
| top_p | float | 1.0 | 控制词汇选择的多样性 |
| top_k | int | null | 限制词汇选择范围 |

## Google Gemini

### 支持的模型

Google提供商支持以下Gemini系列模型：

| 模型名称 | 类别 | 上下文长度 | 说明 |
|---------|------|-----------|------|
| gemini-pro | 聊天 | 32,768 | Gemini Pro文本模型 |
| gemini-pro-vision | 聊天+视觉 | 32,768 | 支持图像理解的Gemini Pro |
| gemini-ultra | 聊天 | 32,768 | Gemini Ultra高级模型 |

### 配置参数

Google Gemini提供商需要以下配置参数：

```csharp
// 创建Google提供商
var googleProvider = new GoogleProvider(
    apiKey: "your-google-api-key",
    apiHost: "https://generativelanguage.googleapis.com"  // 可选，默认为官方API地址
);
```

### 请求参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| temperature | float | 0.7 | 控制生成文本的随机性 |
| max_output_tokens | int | 1000 | 生成文本的最大长度 |
| top_p | float | 0.95 | 控制词汇选择的多样性 |
| top_k | int | 40 | 限制词汇选择范围 |

## ZhipuAI

### 支持的模型

智谱AI提供商支持以下模型：

| 模型名称 | 类别 | 上下文长度 | 说明 |
|---------|------|-----------|------|
| glm-4 | 聊天 | 128,000 | GLM-4大规模预训练模型 |
| glm-3-turbo | 聊天 | 32,000 | GLM-3 Turbo版本 |
| cogview-3 | 图像生成 | - | 文生图模型 |

### 配置参数

ZhipuAI提供商需要以下配置参数：

```csharp
// 创建ZhipuAI提供商
var zhipuProvider = new ZhipuProvider(
    apiKey: "your-zhipu-api-key",
    apiHost: "https://open.bigmodel.cn/api/paas/v4"  // 可选，默认为官方API地址
);
```

### 请求参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| temperature | float | 0.7 | 控制生成文本的随机性 |
| top_p | float | 0.7 | 控制词汇选择的多样性 |
| max_tokens | int | 1500 | 生成文本的最大长度 |

## SiliconFlow

### 支持的模型

硅基流动提供商支持以下模型：

| 模型名称 | 类别 | 上下文长度 | 说明 |
|---------|------|-----------|------|
| silicon-flow-1 | 聊天 | 16,000 | 硅基流动基础模型 |
| silicon-flow-2 | 聊天 | 32,000 | 硅基流动增强模型 |

### 配置参数

SiliconFlow提供商需要以下配置参数：

```csharp
// 创建SiliconFlow提供商
var siliconProvider = new SiliconFlowProvider(
    apiKey: "your-silicon-api-key",
    apiHost: "https://api.siliconflow.com"  // 可选，默认为官方API地址
);
```

### 请求参数

| 参数 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| temperature | float | 0.7 | 控制生成文本的随机性 |
| max_tokens | int | 1000 | 生成文本的最大长度 |
| top_p | float | 0.9 | 控制词汇选择的多样性 |

## 通用参数处理

LLM Agent通过`ModelParameters`类统一管理不同提供商的参数：

```csharp
// 创建通用参数对象
var parameters = new ModelParameters
{
    Temperature = 0.7f,
    MaxTokens = 1000,
    TopP = 0.9f
};

// 转换为特定提供商参数
var openAIParams = parameters.ToOpenAIParameters();
var anthropicParams = parameters.ToAnthropicParameters();
var googleParams = parameters.ToGoogleParameters();
```

## 错误处理

各提供商可能返回不同的错误格式，LLM Agent将它们统一封装为`ApiException`：

```csharp
try
{
    var response = await provider.SendMessageAsync(...);
}
catch (ApiException ex) when (ex.ErrorCode == "rate_limit_exceeded")
{
    // 处理速率限制错误
    Console.WriteLine("API请求过于频繁，请稍后再试");
}
catch (ApiException ex) when (ex.ErrorCode == "invalid_api_key")
{
    // 处理认证错误
    Console.WriteLine("API密钥无效，请检查配置");
}
catch (ApiException ex)
{
    // 处理其他API错误
    Console.WriteLine($"API错误: {ex.Message}");
}
```

## 流式响应处理

所有提供商都支持流式响应，使用方式如下：

```csharp
// 处理流式响应
await provider.SendMessageStreamAsync(
    message,
    session,
    model,
    partialResponse => {
        // 处理部分响应
        Console.Write(partialResponse);
        
        // 可以在UI上实时显示
        UpdateUI(partialResponse);
    }
);
``` 