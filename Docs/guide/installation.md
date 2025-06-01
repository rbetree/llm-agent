# 安装指南

本页面将指导您如何安装和设置LLM多模型客户端。

## 系统要求

在安装LLM多模型客户端之前，请确保您的系统满足以下要求：

- **操作系统**: Windows 10或Windows 11
- **运行时**: .NET 8.0 Runtime（如果没有安装，安装程序会提示您安装）
- **内存**: 至少4GB RAM
- **存储空间**: 至少100MB可用空间
- **网络**: 稳定的互联网连接

## 安装方法

### 方法一：使用安装程序（推荐）

1. 从[GitHub Releases](https://github.com/rbetree/llm-agent/releases)页面下载最新版本的安装程序（`LLM-Client-Setup.exe`）
2. 双击安装程序，按照向导进行安装
3. 如果您的系统没有安装.NET 8.0 Runtime，安装程序会提示您安装
4. 完成安装后，在开始菜单或桌面上找到LLM多模型客户端图标启动应用

### 方法二：便携版

1. 从[GitHub Releases](https://github.com/rbetree/llm-agent/releases)页面下载最新版本的便携版压缩包（`LLM-Client-Portable.zip`）
2. 将压缩包解压到您选择的位置
3. 确保您的系统已安装.NET 8.0 Runtime
4. 双击`LLM-Client.exe`启动应用

## 首次启动配置

首次启动应用时，您需要进行一些基本配置：

1. 选择您想要使用的LLM服务商
2. 配置API密钥（请参考[API密钥获取](#api密钥获取)部分）
3. 可选：配置代理设置（如果需要）
4. 可选：自定义界面主题和其他设置

## API密钥获取

要使用各种LLM服务，您需要获取相应的API密钥：

- **OpenAI**: 访问[OpenAI Platform](https://platform.openai.com/api-keys)创建API密钥
- **Anthropic**: 访问[Anthropic Console](https://console.anthropic.com/keys)申请API密钥
- **Google Gemini**: 访问[Google AI Studio](https://aistudio.google.com/app/apikey)获取API密钥
- **百度文心一言**: 访问[百度智能云](https://console.bce.baidu.com/)获取API Key和Secret Key
- **智谱AI**: 访问[智谱AI开放平台](https://open.bigmodel.cn/)获取API Key和API Secret
- **其他OpenAI兼容服务**: 请访问相关服务商网站获取API密钥

获取API密钥后，在应用的设置界面中填入相应的密钥即可开始使用。

## 故障排除

如果您在安装或使用过程中遇到问题，请尝试以下步骤：

1. 确保您的系统满足最低系统要求
2. 检查您是否已安装最新版本的.NET 8.0 Runtime
3. 确认您的API密钥是否正确
4. 检查您的网络连接是否正常
5. 尝试重启应用程序

如果问题仍然存在，请在[GitHub Issues](https://github.com/rbetree/llm-agent/issues)页面提交问题报告。 