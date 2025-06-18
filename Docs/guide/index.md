# LLM Agent 指南

> 本指南将帮助您快速了解应用的各项功能

LLM Agent是一个基于.NET 8.0和Windows Forms开发的桌面应用程序，旨在提供统一的界面以多种方式来访问和管理多种大语言模型服务。

### 为什么开发LLM Agent：

开发的初衷是学习windows编程技术，作为课程设计成果

## 主要特性

- **多模型支持**: 支持OpenAI、Azure OpenAI、Anthropic、Google等多家服务提供商的模型，以及智谱AI、SiliconFlow等兼容OpenAI格式的服务
- **流式响应**: 实时显示模型生成内容，提供类似打字机的流畅体验
- **Markdown格式**: 支持Markdown格式的消息展示，让AI回复更加美观易读
- **灵活配置**: 支持自定义API密钥、服务器地址和系统提示词
- **安全可靠**: 所有密钥和敏感信息均保存在本地数据库

## 快速开始

1. [安装应用程序](./installation.md)
2. [配置API密钥](./installation.md#配置api密钥)
3. [开始第一次对话](./usage.md#开始对话)

## 深入了解

- [UI界面说明](/architecture/ui-design)
- [交互逻辑](/architecture/interaction)
- [技术设计概述](/architecture/)

## 应用要求

- 操作系统: Windows 10/11
- .NET 8.0 Runtime
- 网络连接: 需要稳定的互联网连接访问API服务

## 常见问题

### 如何添加新的API提供商？

请参阅[添加API提供商](./usage.md#添加api提供商)章节。

### 如何备份我的聊天历史？

目前，聊天历史存储在本地SQLite数据库中。我们正在开发导出和备份功能，详情请查看[技术设计概述](/architecture/)。

### 应用支持哪些模型？

- **OpenAI**: GPT-4、GPT-4 Turbo、GPT-4o、GPT-3.5 Turbo等
- **Azure OpenAI**: 企业级OpenAI服务的所有模型
- **Anthropic**: Claude 3 Opus、Claude 3 Sonnet、Claude 3 Haiku、Claude 2.1等
- **Google**: Gemini 1.5 Pro、Gemini 1.5 Flash、Gemini 1.0 Pro等
- **兼容OpenAI格式的服务**: 如智谱AI（GLM-4、GLM-4-Flash等）、SiliconFlow（Yi-Large、DeepSeek、Mistral等）、DeepSeek等，支持自定义API端点

详细支持列表请参阅[支持的模型](./usage.md#支持的模型)。 