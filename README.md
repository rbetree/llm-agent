# LLM多模型客户端

这是一个支持多个大语言模型（LLM）服务商的Windows桌面客户端应用程序，基于.NET WinForms开发。该应用程序允许用户通过统一的界面访问不同的AI大语言模型，进行自然语言对话。

## 功能特点

- 支持多个LLM服务商：
  - OpenAI (GPT-3.5, GPT-4等)
  - Anthropic (Claude系列)
  - Google Gemini
  - 百度文心一言
  - 智谱AI (GLM系列)
  - 其他OpenAI格式兼容的服务 (如Yi, DeepSeek等，可作为OpenAI类型配置)
- 简洁直观的用户界面
- 支持配置API密钥和API服务器地址
- 支持为不同模型提供商保存单独的配置
- 支持多轮对话和对话历史
- 支持流式响应（打字机效果）
- 支持系统提示（System Prompt）
- 聊天会话管理（新建、切换、保存、导入、清空）
- 格式化的聊天消息显示

## 应用截图

> *请在此处添加应用程序的界面截图*
> 
> *建议添加主界面、设置界面和聊天界面等关键截图*

## 技术栈

- C# / .NET 8.0
- Windows Forms
- System.Text.Json
- System.Net.Http

## 开发环境搭建

1. 安装最新版 [Visual Studio](https://visualstudio.microsoft.com/) (建议2022或更高版本)
2. 安装 .NET 8.0 SDK
3. 克隆本仓库：`git clone https://github.com/[用户名]/llm-agent.git`
4. 使用Visual Studio打开解决方案文件 `llm-agent.sln`
5. 恢复NuGet包
6. 编译并运行项目

## 安装与下载

### 预编译版本

> *在此处提供最新版本的下载链接或发布页面链接*
> 
> 示例：您可以从 [Releases](https://github.com/[用户名]/llm-agent/releases) 页面下载最新的预编译版本。

### 从源码构建

1. 按照上述"开发环境搭建"部分准备环境
2. 在Visual Studio中选择"Release"配置
3. 构建解决方案
4. 编译后的文件位于 `bin/Release/net8.0/` 目录中

## 如何使用

1. 启动应用程序
2. 从左侧面板选择LLM服务提供商
3. 配置API密钥和API服务器地址（如需要）
4. 选择想要使用的模型
5. （可选）点击"系统提示"按钮设置系统提示
6. 在底部输入框中输入您的问题
7. 点击"发送"按钮或按Ctrl+Enter发送消息

## 特别功能

- **流式响应**：默认启用，可通过工具栏上的"切换流式响应"按钮开关，启用时会实时显示模型生成内容
- **系统提示**：点击工具栏上的"系统提示"按钮设置，可用于指导模型的回答风格、角色、约束等
- **多轮对话**：自动保存对话历史并发送给模型，使对话具有连贯性
- **会话管理**：可以保存、导入会话，便于保存重要对话或在不同设备间共享

## API密钥获取方法

- **OpenAI**: 访问 [OpenAI Platform](https://platform.openai.com/api-keys) 创建API密钥
- **Anthropic**: 访问 [Anthropic Console](https://console.anthropic.com/keys) 申请API密钥
- **Google Gemini**: 访问 [Google AI Studio](https://aistudio.google.com/app/apikey) 创建API密钥
- **百度文心一言**: 访问 [百度智能云](https://console.bce.baidu.com/) 申请API密钥，格式为"API Key:Secret Key"
- **智谱AI**: 访问 [智谱AI开放平台](https://open.bigmodel.cn/) 申请API密钥，格式为"API Key:API Secret"
- **其他OpenAI兼容服务**: 请访问相关服务商网站获取API密钥，并配置为OpenAI类型

## 项目文档

项目结构及设计说明文档位于 `Docs/` 目录下：
- [项目结构与设计说明](Docs/项目结构与设计说明.md) - 详细的项目架构和设计原则说明
- [UI设计](Docs/ui设计.md) - UI设计详细文档 
- [交互逻辑和UI说明](Docs/交互逻辑和UI说明.md) - 应用交互流程与UI结构说明
- [待实现功能](Docs/待实现功能.md) - 计划中的功能列表

## 扩展支持新的LLM提供商

如果您想添加对新的LLM提供商的支持，请按照以下步骤操作：

1. 创建一个继承自`BaseLLMProvider`的新类
2. 实现所有必要的抽象方法
3. 在`Models.cs`中添加新提供商的模型定义
4. 在`ProviderFactory.cs`中添加新提供商的创建逻辑
5. 在`ProviderType`枚举中添加新提供商类型

## 项目状态与路线图

当前版本：v0.1.0 (开发中)

### 计划功能

> *请在此处列出计划中的主要功能和改进*
> 
> 示例：
> - [ ] 支持图像生成功能
> - [ ] 支持语音输入和输出
> - [ ] 本地模型支持
> - [ ] 多语言界面

完整的待实现功能列表请参考 [待实现功能](Docs/待实现功能.md)。

## 贡献指南

我们欢迎所有形式的贡献，包括但不限于：

- 提交bug报告和功能请求
- 提交代码改进PR
- 改进文档
- 添加对新LLM提供商的支持

### 贡献流程

1. Fork本仓库
2. 创建您的特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交您的更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 创建一个PR (Pull Request)

## 注意事项

- 各LLM服务商可能对API调用有频率和用量限制，请查阅各自的API文档
- 所有API密钥都保存在本地内存中，应用关闭后不会保留
- 使用流式响应功能可能会增加API调用的费用，但会提供更好的用户体验
- 中国用户访问海外API可能需要适当的网络环境 

## 许可证

本项目采用 [MIT 许可证](LICENSE) 进行许可。

## 鸣谢

- 感谢所有为本项目做出贡献的开发者
- 特别感谢提供API服务的各大LLM服务商 