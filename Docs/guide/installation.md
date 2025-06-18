# 安装指南

本文将指导您完成LLM Agent的安装和初始配置过程。

## 系统要求

在安装LLM Agent之前，请确保您的系统满足以下要求：

- **操作系统**: Windows 10/11
- **.NET运行时**: .NET 8.0或更高版本
- **硬件要求**: 无
- **网络连接**: 需要稳定的互联网连接访问API服务

## 使用

1. 访问[GitHub发布页面](https://github.com/rbetree/llm-agent/releases)
2. 下载最新版本的压缩包:`Llm-Agent.zip`
3. 将ZIP文件解压到您选择的任何位置
4. 双击`llm-agent.exe`启动应用程序

## 对话前设置

### 配置API密钥

1. 启动LLM Agent应用程序
2. 点击左侧导航栏中的"渠道"选项
4. 点击"添加新渠道"
5. 填写以下信息:
   - 渠道名称: 为您的API配置命名
   - 提供商类型: 从下拉菜单中选择API提供商
   - API密钥: 输入您的API密钥
   - API主机: 如果使用自定义端点，请输入API主机地址
6. 点击"保存"按钮完成配置

::: tip 获取API密钥
- **OpenAI**: [OpenAI Platform](https://platform.openai.com/api-keys)
- **Azure OpenAI**: [Azure Portal](https://portal.azure.com/)
- **Anthropic**: [Anthropic Console](https://console.anthropic.com/keys)
- **Google Gemini**: [Google AI Studio](https://aistudio.google.com/app/apikey)
- **兼容OpenAI格式的服务**: 如[智谱AI](https://open.bigmodel.cn/)、[SiliconFlow](https://siliconflow.cn/)、[DeepSeek](https://platform.deepseek.com/api_keys)等
:::

### 验证API连接

1. 添加渠道后，选择该渠道
2. 点击"测试连接"按钮
3. 如果配置正确，您将看到"连接成功"的提示

## 更新

按照[#使用](#使用)重新安装最新应用

## 卸载

只需删除包含应用程序的文件夹即可。