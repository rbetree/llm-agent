# 架构设计

本页面介绍LLM多模型客户端的整体架构设计。

## 架构概述

LLM多模型客户端采用经典的三层架构设计，清晰地分离了表示层、业务逻辑层和数据访问层。这种架构设计使得应用程序具有良好的可维护性、可扩展性和可测试性。

![三层架构](../public/images/architecture/three-tier-architecture.png)

### 三层架构

1. **表示层（UI）**：负责用户界面和用户交互
2. **业务逻辑层（BLL）**：包含应用程序的核心业务逻辑
3. **数据访问层（DAL）**：负责数据的存储和检索

## 项目结构

项目的目录结构反映了三层架构的设计理念：

```
llm-agent/
├── UI/                 # 表示层
│   ├── Controls/       # 自定义控件
│   └── Forms/          # 窗体
│       └── Test/       # 测试窗体
├── BLL/                # 业务逻辑层
├── DAL/                # 数据访问层
├── Model/              # 数据实体类
├── API/                # 第三方API接口
│   └── Provider/       # LLM提供商
├── Common/             # 通用工具类
│   ├── Utils/          # 工具类
│   ├── Extensions/     # 扩展方法
│   └── Exceptions/     # 自定义异常类
├── Docs/               # 项目文档
├── Properties/         # 项目属性和设置
├── bin/                # 编译输出目录
└── obj/                # 编译中间文件
```

## 核心组件

### 表示层（UI）

表示层包含所有用户界面相关的代码，主要由Windows Forms窗体和自定义控件组成：

- **Forms/**：包含应用程序的主窗体和各种对话框
- **Controls/**：包含自定义控件，如聊天消息控件、模型选择控件等

表示层通过调用业务逻辑层的服务来执行业务操作，并将结果显示给用户。

### 业务逻辑层（BLL）

业务逻辑层包含应用程序的核心业务逻辑，负责处理用户请求、调用API接口、处理数据等：

- **ChatService**：处理聊天相关的业务逻辑
- **ModelService**：处理模型相关的业务逻辑
- **SettingsService**：处理应用程序设置相关的业务逻辑
- **PromptService**：处理提示词库相关的业务逻辑
- **WebsiteService**：处理AI网站管理相关的业务逻辑

业务逻辑层通过调用数据访问层的仓储来访问数据，通过调用API接口与第三方服务交互。

### 数据访问层（DAL）

数据访问层负责数据的存储和检索，包括本地数据库操作和文件系统操作：

- **Repository/**：包含各种数据仓储的实现
- **Database/**：包含数据库相关的代码，如数据库连接、初始化等
- **FileStorage/**：包含文件存储相关的代码

数据访问层通过仓储模式（Repository Pattern）提供统一的数据访问接口，隐藏底层数据源的细节。

### 模型层（Model）

模型层包含应用程序的数据实体类，这些类用于在不同层之间传递数据：

- **ChatMessage**：聊天消息实体类
- **ChatSession**：聊天会话实体类
- **ModelInfo**：模型信息实体类
- **ProviderConfig**：提供商配置实体类
- **UserSettings**：用户设置实体类

### API接口层（API）

API接口层负责与第三方LLM服务进行交互，包括请求构建、响应解析等：

- **Provider/**：包含各种LLM服务提供商的实现
- **Interfaces/**：包含API接口的定义
- **Models/**：包含API相关的数据模型

API接口层通过工厂模式（Factory Pattern）创建不同的LLM提供商实例，通过策略模式（Strategy Pattern）处理不同提供商的请求和响应。

### 通用工具层（Common）

通用工具层包含各种辅助类和工具方法，可以被其他层使用：

- **Utils/**：包含各种工具类，如加密工具、日志工具等
- **Extensions/**：包含各种扩展方法，如字符串扩展、集合扩展等
- **Exceptions/**：包含自定义异常类

## 设计模式

LLM多模型客户端使用了多种设计模式来提高代码的可维护性和可扩展性：

### 仓储模式（Repository Pattern）

仓储模式用于数据访问层，提供统一的数据访问接口，隐藏底层数据源的细节：

```csharp
public interface IChatSessionRepository
{
    Task<List<ChatSession>> GetAllSessionsAsync();
    Task<ChatSession> GetSessionByIdAsync(string id);
    Task<bool> AddSessionAsync(ChatSession session);
    Task<bool> UpdateSessionAsync(ChatSession session);
    Task<bool> DeleteSessionAsync(string id);
}
```

### 工厂模式（Factory Pattern）

工厂模式用于创建不同的LLM提供商实例：

```csharp
public class ProviderFactory
{
    public static ILLMProvider CreateProvider(ProviderType providerType, ProviderConfig config, IHttpClientFactory httpClientFactory)
    {
        return providerType switch
        {
            ProviderType.OpenAI => new OpenAIProvider(config, httpClientFactory),
            ProviderType.Anthropic => new AnthropicProvider(config, httpClientFactory),
            ProviderType.Google => new GoogleProvider(config, httpClientFactory),
            ProviderType.Baidu => new BaiduProvider(config, httpClientFactory),
            ProviderType.ZhipuAI => new ZhipuAIProvider(config, httpClientFactory),
            _ => throw new ArgumentException($"Unsupported provider type: {providerType}")
        };
    }
}
```

### 策略模式（Strategy Pattern）

策略模式用于处理不同LLM提供商的请求和响应：

```csharp
public interface ILLMProvider
{
    string GetProviderName();
    List<ModelInfo> GetSupportedModels();
    Task<ChatResponse> SendChatRequestAsync(ChatRequest request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ChatResponseChunk> SendChatRequestStreamAsync(ChatRequest request, CancellationToken cancellationToken = default);
    Task<bool> ValidateApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
}
```

### 单例模式（Singleton Pattern）

单例模式用于确保某些类只有一个实例，如设置服务：

```csharp
public class SettingsService
{
    private static readonly Lazy<SettingsService> _instance = new(() => new SettingsService());
    public static SettingsService Instance => _instance.Value;
    
    private SettingsService()
    {
        // 私有构造函数
    }
    
    // 其他方法...
}
```

### 观察者模式（Observer Pattern）

观察者模式用于实现事件驱动的通信，如聊天消息的实时更新：

```csharp
public class ChatService
{
    public event EventHandler<ChatMessageEventArgs> MessageReceived;
    
    protected virtual void OnMessageReceived(ChatMessageEventArgs e)
    {
        MessageReceived?.Invoke(this, e);
    }
    
    // 其他方法...
}
```

## 数据流

下图展示了应用程序中的主要数据流：

![数据流](../public/images/architecture/data-flow.png)

1. 用户通过UI发送聊天请求
2. UI层调用业务逻辑层的ChatService
3. ChatService调用API接口层的LLM提供商
4. LLM提供商向第三方服务发送请求
5. 第三方服务返回响应
6. LLM提供商解析响应并返回给ChatService
7. ChatService处理响应并通知UI层
8. UI层更新界面显示响应

## 扩展性

LLM多模型客户端的架构设计具有良好的扩展性，可以方便地添加新的功能和支持新的LLM服务提供商：

### 添加新的LLM提供商

1. 在`API/Provider`目录中创建新的提供商类，继承自`BaseLLMProvider`
2. 实现所有必要的抽象方法
3. 在`Models.cs`中添加新提供商的模型定义
4. 在`ProviderFactory.cs`中添加新提供商的创建逻辑
5. 在`ProviderType`枚举中添加新提供商类型

### 添加新的功能

1. 在Model层中添加新的数据实体类
2. 在DAL层中添加新的仓储接口和实现
3. 在BLL层中添加新的服务类
4. 在UI层中添加新的窗体或控件

## 更多架构文档

- [项目结构](./structure.md) - 详细了解项目的目录结构
- [技术栈](./tech-stack.md) - 了解项目使用的技术栈 