# LLM代理应用架构概览

本文档提供了LLM代理应用的整体架构设计和关键组件说明。

## 应用架构分层

该应用程序采用标准的三层架构设计，各层职责明确分离：

```mermaid
flowchart TD
  UI["UI层<br>用户界面与交互"]
  BLL["业务逻辑层<br>业务规则与流程"]
  DAL["数据访问层<br>数据持久化与检索"]
  
  UI --> BLL
  BLL --> DAL
```

### UI层 (UI 目录)

- **职责**：提供用户界面和交互功能
- **主要组件**：
  - `Forms/` - 包含应用程序窗体
    - `LlmAgentMainForm.cs` - 主窗体
    - `ModelManagementForm.cs` - 模型管理窗体
    - `ModelTestForm.cs` - 模型测试窗体
    - `ChannelTestForm.cs` - 渠道测试窗体
  - `Controls/` - 自定义控件
    - `ChatSessionItem.cs` - 聊天会话项控件
    - `ChatMessageItem.cs` - 聊天消息项控件

### 业务逻辑层 (BLL 目录)

- **职责**：实现业务规则和逻辑，连接UI和数据层
- **主要组件**：
  - `ChatHistoryManager.cs` - 聊天历史管理
  - `ChannelManager.cs` - 渠道管理
  - `ChannelService.cs` - 渠道服务

### 数据访问层 (DAL 目录)

- **职责**：处理数据持久化和检索
- **主要组件**：
  - `ChatRepository.cs` - 聊天数据仓库
  - `DatabaseManager.cs` - 数据库管理
  - `ChannelRepository.cs` - 渠道数据仓库

### 数据模型 (Model 目录)

- **职责**：定义应用程序的数据结构
- **主要组件**：
  - `ChatSession.cs` - 聊天会话模型
  - `ChatMessage.cs` - 聊天消息模型
  - `Channel.cs` - 渠道模型
  - `Models.cs` - 模型信息定义

## 数据存储机制

应用程序使用多种存储机制来持久化不同类型的数据：

> 详细的数据存储架构、用户数据与应用数据分离方案、数据结构和技术栈，请参阅[数据存储](./data-storage.md)文档。

### 1. MySQL数据库存储

用于持久化聊天会话、消息、模型信息和渠道配置：

- **聊天数据**：
  - `ChatSessions` 表 - 存储会话基本信息
  - `ChatMessages` 表 - 存储聊天消息
  
- **模型数据**：
  - `Models` 表 - 存储各提供商支持的模型信息

- **渠道配置**：
  - `Channels` 表 - 存储渠道基本信息
  - `ChannelModels` 表 - 存储渠道支持的模型

### 2. 应用程序设置

用于存储用户首选项和应用配置：

- **机制**：使用 .NET 的 `Properties.Settings`
- **存储项**：
  - 上次选择的提供商
  - 上次选择的模型
  - 系统提示
  - 流式响应设置

## 数据模型及关系

### 核心数据模型

1. **聊天会话 (ChatSession)**
   ```mermaid
   classDiagram
     class ChatSession {
       +string Id
       +string Title
       +DateTime CreatedAt
       +DateTime UpdatedAt
       +List~ChatMessage~ Messages
     }
   ```

2. **聊天消息 (ChatMessage)**
   ```mermaid
   classDiagram
     class ChatMessage {
       +string Id
       +string Role
       +string Content
       +DateTime CreatedAt
       +DateTime UpdatedAt
       +string Timestamp
       +string ModelId
     }
   ```

3. **渠道 (Channel)**
   ```mermaid
   classDiagram
     class Channel {
       +string Id
       +string Name
       +string ProviderType
       +string ApiKey
       +string ApiHost
       +bool IsEnabled
       +bool UseStreamResponse
       +List~ModelInfo~ SupportedModels
       +DateTime CreatedAt
       +DateTime UpdatedAt
     }
   ```

4. **模型信息 (ModelInfo)**
   ```mermaid
   classDiagram
     class ModelInfo {
       +string Id
       +string Name
       +string ProviderType
       +int Category
       +int ContextLength
       +double TokenPrice
       +bool Enabled
     }
   ```

### 数据关系

```mermaid
erDiagram
    ChatSession ||--o{ ChatMessage : contains
    ChatSession }|--|| Channel : uses
    Channel ||--o{ ChannelModel : supports
```

- 一个聊天会话包含多条聊天消息
- 发送消息时使用特定渠道和模型
- 一个渠道支持多个模型

## 控件间数据交流

### 主窗体与业务逻辑层交互

主窗体 (`LlmAgentMainForm`) 持有各业务逻辑管理器的实例：

```mermaid
classDiagram
    class LlmAgentMainForm {
        -ChatHistoryManager _chatHistoryManager
        -ChannelManager _channelManager
        -ChannelService _channelService
        -ProviderFactory _providerFactory
    }
```

### 数据流动路径

#### 1. 发送聊天消息的数据流

```mermaid
flowchart LR
    A["发送按钮点击"] --> B["SendMessage()方法"]
    B --> C["ChatHistoryManager"]
    C --> D["ChatRepository"]
    B --> E["创建消息对象"]
    D --> F["MySQL数据库"]
    E --> G["调用LLM API获取回复"]
    G --> H["更新UI显示"]
```

#### 2. 添加新渠道的数据流

```mermaid
flowchart LR
    A["添加渠道按钮点击"] --> B["addChannelButton_Click"]
    B --> C["ChannelManager"]
    C --> D["MySQL数据库"]
    B --> E["创建Channel对象"]
    E --> F["更新UI显示新渠道"]
```

#### 3. 切换聊天会话的数据流

```mermaid
flowchart LR
    A["会话项点击"] --> B["SwitchToChat()"]
    B --> C["ChatHistoryManager"]
    C --> D["ChatRepository"]
    B --> E["更新UI显示会话内容"]
    D --> F["MySQL数据库"]
``` 