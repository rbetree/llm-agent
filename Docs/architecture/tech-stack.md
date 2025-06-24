# 技术栈

LLM Agent项目采用了一系列现代技术和框架，以下是项目的核心技术栈说明。

## 开发技术栈

- **.NET 8.0**: 最新的.NET平台，提供高性能、跨平台的开发环境
- **Windows Forms**: 用于构建Windows桌面应用程序的UI框架
- **C#**: 项目开发语言，提供强大的类型安全和表达能力
- **SQLite**: 轻量级嵌入式数据库，用于存储聊天历史和模型信息

## 第三方API集成

- **REST API客户端**: 用于与各LLM服务提供商通信
- **HTTP客户端工厂**: 实现高效的HTTP连接复用
- **异步API调用**: 确保UI响应性和高效的网络通信

## 核心库和框架

- **System.Text.Json**: 用于JSON序列化和反序列化
- **System.Data.SQLite.Core**: SQLite数据库访问
- **System.Net.Http**: HTTP客户端实现
- **System.Security.Cryptography.ProtectedData**: 用于敏感数据加密存储
- **Markdig**: Markdown解析和渲染库，用于美化聊天消息显示
- **Microsoft.Web.WebView2**: 内置浏览器控件，用于AI网站浏览功能

## 设计模式应用

项目中应用了多种设计模式，确保代码的可维护性和可扩展性：

1. **工厂模式**: 
   - `ProviderFactory` 用于创建不同的LLM提供商实例
   - 根据配置动态选择合适的提供商实现

2. **仓储模式**:
   - `ChatRepository` 封装数据访问逻辑
   - 提供统一的数据操作接口

3. **单例模式**:
   - `DatabaseManager` 确保数据库连接的唯一性
   - `ChannelManager` 管理渠道配置的全局访问

4. **策略模式**:
   - 不同的LLM提供商实现相同的接口
   - 运行时可以无缝切换不同的提供商

## 开发工具链

- **Visual Studio 2022**: 主要开发IDE，支持.NET 8.0开发
- **Git**: 版本控制系统
- **GitHub**: 代码托管平台
- **GitHub Actions**: CI/CD自动化，用于自动构建和发布
- **GitHub Pages**: 文档网站托管
- **NuGet**: 包管理器，管理第三方依赖
- **VitePress**: 文档生成工具，用于构建项目文档网站