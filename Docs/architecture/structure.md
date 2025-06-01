# 项目结构

本页面详细说明LLM多模型客户端的项目目录结构和各个组件的作用。

## 目录结构概览

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

## 表示层（UI）

表示层包含所有用户界面相关的代码，主要由Windows Forms窗体和自定义控件组成。

### UI/Forms/

包含应用程序的主窗体和各种对话框：

- **MainForm.cs**: 应用程序的主窗体，包含左侧导航栏、中间内容列表和右侧主内容区
- **SettingsForm.cs**: 设置窗体，用于配置应用程序的各种设置
- **ChatForm.cs**: 聊天窗体，用于显示聊天对话
- **ModelSelectionForm.cs**: 模型选择窗体，用于选择要使用的LLM模型
- **PromptLibraryForm.cs**: 提示词库窗体，用于管理提示词
- **AiWebsiteForm.cs**: AI网站管理窗体，用于管理AI网站
- **AboutForm.cs**: 关于窗体，显示应用程序的版本信息和作者信息

### UI/Controls/

包含自定义控件，用于在窗体中显示特定的UI元素：

- **ChatMessageControl.cs**: 聊天消息控件，用于显示单条聊天消息
- **ChatSessionControl.cs**: 聊天会话控件，用于显示会话列表项
- **ModelItemControl.cs**: 模型项控件，用于显示模型列表项
- **PromptItemControl.cs**: 提示词项控件，用于显示提示词列表项
- **WebsiteCardControl.cs**: 网站卡片控件，用于显示AI网站卡片
- **MarkdownViewer.cs**: Markdown查看器控件，用于渲染Markdown格式的文本
- **LoadingIndicator.cs**: 加载指示器控件，用于显示加载状态
- **NotificationControl.cs**: 通知控件，用于显示通知消息

### UI/Forms/Test/

包含用于测试的窗体：

- **ApiTestForm.cs**: API测试窗体，用于测试与LLM服务的API交互
- **UiTestForm.cs**: UI测试窗体，用于测试自定义控件

## 业务逻辑层（BLL）

业务逻辑层包含应用程序的核心业务逻辑，负责处理用户请求、调用API接口、处理数据等。

### BLL/Services/

包含各种服务类，用于实现业务逻辑：

- **ChatService.cs**: 聊天服务，处理聊天相关的业务逻辑
- **ModelService.cs**: 模型服务，处理模型相关的业务逻辑
- **SettingsService.cs**: 设置服务，处理应用程序设置相关的业务逻辑
- **PromptService.cs**: 提示词服务，处理提示词库相关的业务逻辑
- **WebsiteService.cs**: 网站服务，处理AI网站管理相关的业务逻辑
- **AuthService.cs**: 认证服务，处理API密钥验证相关的业务逻辑
- **LogService.cs**: 日志服务，处理应用程序日志记录

### BLL/Managers/

包含各种管理器类，用于管理特定的业务对象：

- **ChatSessionManager.cs**: 聊天会话管理器，管理聊天会话的创建、保存、加载等
- **ProviderManager.cs**: 提供商管理器，管理LLM服务提供商的创建和使用
- **ExportManager.cs**: 导出管理器，处理聊天会话的导出功能

## 数据访问层（DAL）

数据访问层负责数据的存储和检索，包括本地数据库操作和文件系统操作。

### DAL/Repository/

包含各种数据仓储的实现，用于访问数据：

- **ChatSessionRepository.cs**: 聊天会话仓储，处理聊天会话的存储和检索
- **ModelRepository.cs**: 模型仓储，处理模型信息的存储和检索
- **SettingsRepository.cs**: 设置仓储，处理应用程序设置的存储和检索
- **PromptRepository.cs**: 提示词仓储，处理提示词的存储和检索
- **WebsiteRepository.cs**: 网站仓储，处理AI网站信息的存储和检索

### DAL/Database/

包含数据库相关的代码，如数据库连接、初始化等：

- **DatabaseContext.cs**: 数据库上下文，提供对数据库的访问
- **DatabaseInitializer.cs**: 数据库初始化器，负责创建和初始化数据库
- **DatabaseMigration.cs**: 数据库迁移，处理数据库结构的升级

### DAL/FileStorage/

包含文件存储相关的代码：

- **FileStorageService.cs**: 文件存储服务，处理文件的读写操作
- **ExportService.cs**: 导出服务，处理文件导出功能

## 模型层（Model）

模型层包含应用程序的数据实体类，这些类用于在不同层之间传递数据。

### Model/Chat/

包含聊天相关的数据实体类：

- **ChatMessage.cs**: 聊天消息实体类，表示一条聊天消息
- **ChatSession.cs**: 聊天会话实体类，表示一个聊天会话
- **ChatRole.cs**: 聊天角色枚举，定义了消息的角色类型（系统、用户、助手等）

### Model/Provider/

包含提供商相关的数据实体类：

- **ModelInfo.cs**: 模型信息实体类，表示一个LLM模型的基本信息
- **ProviderConfig.cs**: 提供商配置实体类，表示一个LLM服务提供商的配置
- **ProviderType.cs**: 提供商类型枚举，定义了支持的LLM服务提供商类型

### Model/Settings/

包含设置相关的数据实体类：

- **UserSettings.cs**: 用户设置实体类，表示用户的应用程序设置
- **AppSettings.cs**: 应用程序设置实体类，表示应用程序的全局设置
- **ProxySettings.cs**: 代理设置实体类，表示网络代理设置

### Model/Prompt/

包含提示词相关的数据实体类：

- **Prompt.cs**: 提示词实体类，表示一个提示词
- **PromptCategory.cs**: 提示词分类实体类，表示提示词的分类

### Model/Website/

包含AI网站相关的数据实体类：

- **AiWebsite.cs**: AI网站实体类，表示一个AI网站
- **WebsiteCredential.cs**: 网站凭据实体类，表示访问AI网站所需的凭据

## API接口层（API）

API接口层负责与第三方LLM服务进行交互，包括请求构建、响应解析等。

### API/Provider/

包含各种LLM服务提供商的实现：

- **OpenAIProvider.cs**: OpenAI提供商，处理与OpenAI API的交互
- **AnthropicProvider.cs**: Anthropic提供商，处理与Anthropic API的交互
- **GoogleProvider.cs**: Google提供商，处理与Google Gemini API的交互
- **BaiduProvider.cs**: 百度提供商，处理与百度文心一言API的交互
- **ZhipuAIProvider.cs**: 智谱AI提供商，处理与智谱AI API的交互
- **BaseLLMProvider.cs**: LLM提供商基类，提供通用功能
- **ProviderFactory.cs**: 提供商工厂，负责创建不同的LLM提供商实例

### API/Interfaces/

包含API接口的定义：

- **ILLMProvider.cs**: LLM提供商接口，定义了与LLM服务交互的基本方法
- **IModelProvider.cs**: 模型提供商接口，定义了获取模型信息的方法
- **IApiKeyValidator.cs**: API密钥验证器接口，定义了验证API密钥的方法

### API/Models/

包含API相关的数据模型：

- **ChatRequest.cs**: 聊天请求模型，表示发送给LLM服务的请求
- **ChatResponse.cs**: 聊天响应模型，表示从LLM服务接收的响应
- **ChatResponseChunk.cs**: 聊天响应块模型，表示流式响应中的一个数据块
- **TokenUsage.cs**: 令牌使用情况模型，表示API调用中使用的令牌数

## 通用工具层（Common）

通用工具层包含各种辅助类和工具方法，可以被其他层使用。

### Common/Utils/

包含各种工具类：

- **EncryptionUtil.cs**: 加密工具，用于加密和解密敏感信息
- **LogUtil.cs**: 日志工具，用于记录应用程序日志
- **JsonUtil.cs**: JSON工具，用于JSON序列化和反序列化
- **HttpUtil.cs**: HTTP工具，用于发送HTTP请求
- **MarkdownUtil.cs**: Markdown工具，用于Markdown格式的处理
- **ValidationUtil.cs**: 验证工具，用于验证输入数据

### Common/Extensions/

包含各种扩展方法：

- **StringExtensions.cs**: 字符串扩展，提供字符串处理的扩展方法
- **CollectionExtensions.cs**: 集合扩展，提供集合处理的扩展方法
- **DateTimeExtensions.cs**: 日期时间扩展，提供日期时间处理的扩展方法
- **EnumExtensions.cs**: 枚举扩展，提供枚举处理的扩展方法
- **ControlExtensions.cs**: 控件扩展，提供Windows Forms控件的扩展方法

### Common/Exceptions/

包含自定义异常类：

- **ApiException.cs**: API异常，表示API调用过程中发生的异常
- **ValidationException.cs**: 验证异常，表示数据验证失败
- **ConfigurationException.cs**: 配置异常，表示配置错误
- **DatabaseException.cs**: 数据库异常，表示数据库操作失败

## 其他目录

### Properties/

包含项目属性和设置：

- **AssemblyInfo.cs**: 程序集信息，定义了应用程序的版本、作者等信息
- **Resources.resx**: 资源文件，包含应用程序使用的字符串和其他资源
- **Settings.settings**: 设置文件，包含应用程序的配置设置

### Docs/

包含项目文档：

- **README.md**: 项目说明文档
- **CHANGELOG.md**: 变更日志
- **LICENSE**: 许可证文件
- **API.md**: API文档
- **UserGuide.md**: 用户指南

### bin/ 和 obj/

这些目录包含编译输出和中间文件，通常不需要直接操作：

- **bin/**: 包含编译后的二进制文件和依赖项
- **obj/**: 包含编译过程中生成的中间临时文件 