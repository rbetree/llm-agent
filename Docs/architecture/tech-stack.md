# 技术栈

本页面详细说明LLM多模型客户端使用的技术栈和依赖项。

## 核心技术

### 开发语言

- **C#**: 主要开发语言，版本 10.0
- **.NET**: 基于 .NET 8.0 框架开发

### UI框架

- **Windows Forms**: 用于构建桌面应用程序的UI
- **自定义控件**: 基于Windows Forms开发的自定义控件

### 数据处理

- **System.Text.Json**: 用于JSON序列化和反序列化
- **SQLite**: 轻量级嵌入式数据库，用于本地数据存储
- **Entity Framework Core**: ORM框架，用于数据库操作

### 网络通信

- **System.Net.Http**: 用于HTTP请求和响应
- **HttpClient**: 用于与LLM服务API进行通信
- **IHttpClientFactory**: 用于创建和管理HttpClient实例

### 异步编程

- **Task-based Asynchronous Pattern (TAP)**: 基于任务的异步编程模式
- **IAsyncEnumerable**: 用于处理异步流数据（如流式聊天响应）
- **CancellationToken**: 用于取消异步操作

## 依赖库

### 核心依赖

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Microsoft.EntityFrameworkCore.Sqlite | 8.0.0 | SQLite数据库访问 |
| Microsoft.Extensions.Http | 8.0.0 | HTTP客户端工厂 |
| Microsoft.Extensions.DependencyInjection | 8.0.0 | 依赖注入 |
| Microsoft.Extensions.Configuration | 8.0.0 | 配置管理 |
| Microsoft.Extensions.Logging | 8.0.0 | 日志记录 |

### UI相关

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Markdig | 0.33.0 | Markdown渲染 |
| WinForms.DataVisualization | 1.8.0 | 数据可视化控件 |
| ModernWpfUI | 0.9.6 | 现代化UI控件 |

### API集成

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Newtonsoft.Json | 13.0.3 | JSON处理（部分API需要） |
| System.IdentityModel.Tokens.Jwt | 7.0.3 | JWT处理 |
| Microsoft.AspNetCore.WebUtilities | 8.0.0 | Web工具类 |

### 安全性

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Microsoft.AspNetCore.Cryptography.KeyDerivation | 8.0.0 | 密钥派生 |
| System.Security.Cryptography.ProtectedData | 8.0.0 | 数据保护 |

### 工具类

| 依赖项 | 版本 | 用途 |
|-------|------|------|
| Serilog | 3.1.1 | 结构化日志 |
| Polly | 8.2.0 | 弹性和瞬态故障处理 |
| CommunityToolkit.Mvvm | 8.2.2 | MVVM模式支持 |
| Humanizer | 2.14.1 | 字符串和数字人性化处理 |

## 开发工具

### IDE

- **Visual Studio 2022**: 主要开发环境，版本 17.8 或更高
- **Visual Studio Code**: 辅助开发环境，用于编辑文档等

### 构建工具

- **MSBuild**: .NET项目构建系统
- **NuGet**: 包管理器，用于管理依赖项

### 版本控制

- **Git**: 分布式版本控制系统
- **GitHub**: 代码托管平台

### 测试工具

- **MSTest**: 单元测试框架
- **Moq**: 模拟框架，用于单元测试
- **FluentAssertions**: 流畅的断言库，用于单元测试

## 架构模式

### 应用架构

- **三层架构**: 表示层、业务逻辑层、数据访问层
- **仓储模式**: 用于数据访问层，提供统一的数据访问接口
- **工厂模式**: 用于创建LLM提供商实例
- **策略模式**: 用于处理不同LLM提供商的请求和响应
- **单例模式**: 用于确保某些类只有一个实例
- **观察者模式**: 用于实现事件驱动的通信

### 编程范式

- **面向对象编程 (OOP)**: 主要编程范式
- **异步编程**: 使用async/await进行异步操作
- **LINQ**: 用于数据查询
- **扩展方法**: 用于扩展现有类型的功能

## 第三方API

### LLM服务

| 服务提供商 | API版本 | 文档链接 |
|----------|---------|---------|
| OpenAI | v1 | [OpenAI API](https://platform.openai.com/docs/api-reference) |
| Anthropic | v1 | [Anthropic API](https://docs.anthropic.com/claude/reference) |
| Google | v1 | [Google Gemini API](https://ai.google.dev/docs) |
| 百度 | v1 | [百度文心一言API](https://cloud.baidu.com/doc/WENXINWORKSHOP/index.html) |
| 智谱AI | v1 | [智谱AI API](https://open.bigmodel.cn/dev/api) |

## 开发规范

### 代码规范

- **命名规范**: 遵循.NET命名规范
  - 类名、方法名: PascalCase
  - 私有字段: _camelCase
  - 局部变量: camelCase
  - 常量: ALL_CAPS
- **注释规范**: 使用XML文档注释
- **异常处理**: 使用try-catch块处理异常，并记录日志

### 项目结构规范

- **文件组织**: 按功能模块组织文件
- **命名空间**: 使用层次化的命名空间
- **依赖关系**: 遵循依赖倒置原则，高层模块不依赖低层模块

### 版本控制规范

- **分支模型**: 使用Git Flow分支模型
- **提交消息**: 使用约定式提交规范
- **版本号**: 使用语义化版本号

## 性能优化

### UI性能

- **异步加载**: 使用异步方法加载数据，避免阻塞UI线程
- **虚拟化**: 对长列表使用虚拟化技术，减少内存占用
- **延迟加载**: 对不立即需要的资源使用延迟加载

### 数据性能

- **缓存**: 对频繁访问的数据使用内存缓存
- **批量操作**: 对数据库操作使用批量操作，减少IO次数
- **异步IO**: 使用异步IO操作，提高响应速度

### 网络性能

- **连接池**: 使用HttpClient连接池，复用HTTP连接
- **压缩**: 对HTTP请求和响应使用压缩
- **超时控制**: 对网络请求设置合理的超时时间

## 安全性

### 数据安全

- **加密存储**: 敏感数据（如API密钥）使用加密存储
- **安全擦除**: 内存中的敏感数据使用后安全擦除
- **最小权限**: 遵循最小权限原则，只访问必要的数据

### 网络安全

- **HTTPS**: 所有网络请求使用HTTPS
- **证书验证**: 验证服务器证书
- **防止中间人攻击**: 实现证书固定（Certificate Pinning）

### 输入验证

- **参数验证**: 验证所有用户输入
- **防止注入**: 使用参数化查询，防止SQL注入
- **输出编码**: 对输出进行适当编码，防止XSS攻击 