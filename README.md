# LLM多模型客户端

一个支持多个大语言模型（LLM）服务商的Windows桌面客户端应用程序，基于.NET WinForms开发。通过统一的界面访问不同的AI大语言模型，进行自然语言对话。

## 目录

- [UI预览](#ui预览)
- [技术栈](#技术栈)
- [功能](#功能)
- [项目结构](#项目结构)
- [使用指南](#使用指南)
  - [安装与下载](#安装与下载)
  - [基本使用](#基本使用)
  - [API密钥获取](#api密钥获取)
- [扩展开发](#扩展开发)
- [贡献指南](#贡献指南)
- [许可证](#许可证)
- [鸣谢](#鸣谢)

## UI预览

> *请在此处添加应用程序的界面截图*
> 
> *建议添加：*
> - 主界面（对话界面）
> - 设置界面
> - 模型选择界面

## 技术栈

- **开发语言**: C# / .NET 8.0
- **UI框架**: Windows Forms
- **数据处理**: System.Text.Json
- **网络通信**: System.Net.Http
- **构建工具**: Visual Studio 2022+

## 功能

### 多模型支持
- **OpenAI**: GPT-3.5, GPT-4系列
- **Anthropic**: Claude系列
- **Google**: Gemini系列
- **百度**: 文心一言
- **智谱AI**: GLM系列
- **其他兼容服务**: 支持OpenAI格式兼容的服务(如Yi, DeepSeek等)

### 核心功能
- **流式响应**: 实时显示模型生成内容（打字机效果）
- **系统提示**: 自定义System Prompt指导模型回答风格和角色
- **多轮对话**: 自动保存对话历史，保持对话连贯性
- **会话管理**: 支持新建、切换、保存、导入和清空会话
- **API配置**: 支持自定义API密钥和服务器地址
- **独立配置**: 为不同模型提供商保存单独的配置
- **格式化显示**: 美观的聊天消息展示，支持Markdown格式

## 项目结构

项目采用经典三层架构设计，清晰分离UI、业务逻辑和数据访问层：

```
llm-agent/
├── UI/                 # 界面展示层
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

### 特殊文件夹说明

- **bin/**: 编译后的二进制文件输出目录，包含可执行文件和依赖项
- **obj/**: 编译过程中生成的中间临时文件，通常不需要直接操作
- **Properties/**: 包含项目属性、设置和资源文件，如AssemblyInfo.cs和应用程序配置

详细的项目架构和设计原则说明请参考 [项目结构与设计说明](Docs/项目结构与设计说明.md)。

## 使用指南

### 安装与下载

#### 预编译版本

> *在此处提供最新版本的下载链接或发布页面链接*
> 
> 您可以从 [Releases](https://github.com/[用户名]/llm-agent/releases) 页面下载最新的预编译版本。

#### 从源码构建

1. 安装前提条件：
   - 最新版 [Visual Studio](https://visualstudio.microsoft.com/) (建议2022或更高版本)
   - .NET 8.0 SDK

2. 构建步骤：
   ```bash
   # 克隆仓库
   git clone https://github.com/[用户名]/llm-agent.git
   
   # 使用Visual Studio打开解决方案
   # 或使用命令行构建
   cd llm-agent
   dotnet build -c Release

   # 或使用命令行运行
   cd llm-agent
   dotnet run
   ```

3. 编译后的文件位于 `bin/Release/net8.0/` 目录中

### 基本使用

1. 启动应用程序
2. 从左侧面板选择LLM服务提供商
3. 配置API密钥和API服务器地址
4. 选择想要使用的模型
5. (可选) 设置系统提示(System Prompt)
6. 在底部输入框中输入您的问题
7. 点击"发送"按钮或按Ctrl+Enter发送消息

### API密钥获取

- **OpenAI**: [OpenAI Platform](https://platform.openai.com/api-keys)
- **Anthropic**: [Anthropic Console](https://console.anthropic.com/keys)
- **Google Gemini**: [Google AI Studio](https://aistudio.google.com/app/apikey)
- **百度文心一言**: [百度智能云](https://console.bce.baidu.com/) (格式: "API Key:Secret Key")
- **智谱AI**: [智谱AI开放平台](https://open.bigmodel.cn/) (格式: "API Key:API Secret")
- **其他OpenAI兼容服务**: 请访问相关服务商网站获取API密钥

## 扩展开发

### 添加新的LLM提供商

1. 创建继承自`BaseLLMProvider`的新类
2. 实现所有必要的抽象方法
3. 在`Models.cs`中添加新提供商的模型定义
4. 在`ProviderFactory.cs`中添加新提供商的创建逻辑
5. 在`ProviderType`枚举中添加新提供商类型

### 开发环境搭建

1. 安装最新版 [Visual Studio](https://visualstudio.microsoft.com/) (建议2022或更高版本)
2. 安装 .NET 8.0 SDK
3. 克隆本仓库
4. 使用Visual Studio打开解决方案文件 `llm-agent.sln`
5. 恢复NuGet包
6. 编译并运行项目

## 贡献指南

我们欢迎所有形式的贡献，包括但不限于：提交bug报告、功能请求、代码改进、文档改进等。

### 贡献流程

1. Fork本仓库
2. 创建您的特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交您的更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建一个PR (Pull Request)

## 注意事项

- 各LLM服务商可能对API调用有频率和用量限制
- API密钥保存在本地内存中，应用关闭后不会保留
- 中国用户访问海外API可能需要适当的网络环境

## 许可证

本项目采用 [MIT 许可证](LICENSE) 进行许可。

## 鸣谢

- 感谢所有为本项目做出贡献的开发者
- 特别感谢提供API服务的各大LLM服务商 