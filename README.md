# <img src="Docs/public/images/logo.png" alt="llm-agent" width="36" height="36" style="vertical-align: middle;"> llm-agent

一个支持多个大语言模型（LLM）服务商的Windows桌面客户端应用程序，基于.NET WinForms开发。通过统一的界面访问不同的AI大语言模型，进行自然语言对话，提升AI交互体验。

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/License-MIT-blue?style=flat-square)](LICENSE)
[![Windows](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat-square&logo=windows)](https://github.com/rbetree/llm-agent)

## 文档

 [在线文档](https://rbetree.github.io/llm-agent/)

## 应用截图

![聊天页](Resources\聊天页.png)

## 功能

### 支持的模型
- **OpenAI**: GPT-3.5, GPT-4系列
- **Anthropic**: Claude系列
- **Google**: Gemini系列
- **百度**: 文心一言
- **智谱AI**: GLM系列
- **其他兼容服务**: 支持OpenAI格式兼容的服务(如DeepSeek等)

### 主要功能
- **流式响应**: 实时显示模型生成内容（打字机效果）
- **系统Prompt**: 自定义System Prompt指导模型回答风格和角色
- **多轮对话**: 自动保存对话历史，保持对话连贯性
- **会话管理**: 支持新建、切换、保存、导入和清空会话
- **提示词库**: 管理和使用预设提示词模板，快速应用到对话中
- **用户登录**: 支持用户账号登录和权限管理
- **AIWeb浏览**: 访问AI模型提供商的网页界面和资源
- **渠道设置**: 配置不同AI服务提供商的接入参数
- **个性化设置**: 调整应用全局设置和个性化选项
- **API配置**: 支持自定义API密钥和服务器地址
- **独立配置**: 为不同模型提供商保存单独的配置
- **Markdown支持**: 美观的聊天消息展示，支持Markdown格式

## 技术栈

- **开发语言**: C# / .NET 8.0
- **UI框架**: Windows Forms
- **数据处理**: System.Text.Json
- **数据存储**: System.Data.SQLite
- **网络通信**: System.Net.Http
- **构建工具**: Visual Studio 2022
- **Markdown渲染**: Markdig
- **Web浏览**: Microsoft.Web.WebView2
- **自定义控件**: 聊天界面、提示词卡片、会话管理等

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
│   └── Adapters/       # 适配器
├── Docs/               # 项目文档
│   ├── .vitepress/     # VitePress配置
│   ├── architecture/   # 架构文档
│   ├── api/            # API文档
│   ├── guide/          # 使用指南
│   └── public/         # 静态资源
│       └── images/     # 图片资源
├── Properties/         # 项目属性和设置
├── bin/                # 编译输出目录
└── obj/                # 编译中间文件
```

## 快速入门

1. **下载应用**：从[Releases](https://github.com/rbetree/llm-agent/releases)页面下载最新版本
2. **安装**：解压缩文件或运行安装程序
3. **配置API密钥**：
   - 从左侧面板选择LLM提供商
   - 输入对应的API密钥和服务器地址
4. **开始对话**：
   - 选择模型
   - 在输入框中输入问题
   - 点击"发送"或按Ctrl+Enter

## 使用指南

### 安装与下载

#### 预编译版本

您可以从 [Releases](https://github.com/rbetree/llm-agent/releases) 页面下载最新的预编译版本。

#### 从源码构建

1. 安装前提条件：
   - 最新版 [Visual Studio](https://visualstudio.microsoft.com/) (建议2022或更高版本)
   - .NET 8.0 SDK

2. 构建步骤：
   ```bash
   # 克隆仓库
   git clone https://github.com/rbetree/llm-agent.git
   
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
- **百度文心一言**: [百度智能云](https://console.bce.baidu.com/)
- **智谱AI**: [智谱AI开放平台](https://open.bigmodel.cn/)
- **其他OpenAI兼容服务**: 请访问相关服务商网站获取API密钥

### 注意事项

- 各LLM服务商可能对API调用有频率和用量限制
- API密钥保存在本地内存中，应用关闭后不会保留
- 中国用户访问海外API可能需要适当的网络环境

### 开发环境搭建

1. 安装最新版 [Visual Studio](https://visualstudio.microsoft.com/) (建议2022或更高版本)
2. 安装 .NET 8.0 SDK
3. 克隆本仓库
4. 使用Visual Studio打开解决方案文件 `llm-agent.sln`
5. 恢复NuGet包
6. 编译并运行项目

## 许可证

本项目采用 [MIT 许可证](LICENSE) 进行许可。