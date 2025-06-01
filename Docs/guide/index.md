# LLM Agent 使用指南

欢迎使用LLM Agent，这是一个强大的多模型客户端，支持连接多种大语言模型服务提供商。本指南将帮助您快速上手并充分利用应用的各项功能。

## 什么是LLM Agent？

LLM Agent是一个基于.NET 8.0和Windows Forms开发的桌面应用程序，旨在提供统一的界面来访问和管理多种大语言模型服务。无论您是使用OpenAI、Anthropic、Google还是其他提供商的模型，LLM Agent都能提供一致的用户体验。

## 主要特性

- **多模型支持**: 支持OpenAI、Anthropic、Google等多家服务提供商的模型
- **流式响应**: 实时显示模型生成内容，提供类似打字机的流畅体验
- **Markdown格式**: 支持Markdown格式的消息展示，让AI回复更加美观易读
- **多轮对话**: 自动保存对话历史，保持对话的连贯性和上下文理解
- **灵活配置**: 支持自定义API密钥、服务器地址和系统提示词
- **安全可靠**: 所有密钥和敏感信息均保存在本地

## 快速开始

1. [安装应用程序](./installation.md)
2. [配置API密钥](./installation.md#配置api密钥)
3. [开始第一次对话](./usage.md#开始对话)

## 深入了解

- [UI界面说明](./ui-design.md)
- [交互逻辑](./interaction.md)
- [开发路线图](./roadmap.md)

## 系统要求

- 操作系统: Windows 10/11
- .NET 8.0 Runtime
- 最低配置: 4GB RAM, 1GHz CPU
- 推荐配置: 8GB RAM, 2GHz+ CPU
- 网络连接: 需要稳定的互联网连接访问API服务

## 常见问题

### 如何添加新的API提供商？

请参阅[添加API提供商](./usage.md#添加api提供商)章节。

### 如何备份我的聊天历史？

目前，聊天历史存储在本地SQLite数据库中。我们正在开发导出和备份功能，详情请查看[开发路线图](./roadmap.md)。

### 应用支持哪些模型？

LLM Agent支持多种主流大语言模型，包括但不限于：
- OpenAI: GPT-4, GPT-3.5-Turbo
- Anthropic: Claude 3 系列
- Google: Gemini 系列
- 以及更多国内外模型

详细支持列表请参阅[支持的模型](./usage.md#支持的模型)。 