# LLM代理应用架构概览

本文档提供了LLM代理应用的整体架构设计和关键组件说明。

## 应用架构分层

该应用程序采用标准的三层架构设计，各层职责明确分离：

```
┌─────────────┐
│    UI层     │ 用户界面与交互
├─────────────┤
│  业务逻辑层  │ 业务规则与流程
├─────────────┤
│  数据访问层  │ 数据持久化与检索
└─────────────┘
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

### 1. SQLite数据库存储

用于持久化聊天会话、消息和模型信息：

- **聊天数据**：
  - `ChatSessions` 表 - 存储会话基本信息
  - `ChatMessages` 表 - 存储聊天消息
  
- **模型数据**：
  - `Models` 表 - 存储各提供商支持的模型信息

### 2. JSON文件存储

用于持久化渠道配置：

- **文件位置**：`%AppData%\LlmAgent\channels.json`
- **管理类**：`ChannelManager` 负责序列化和反序列化

### 3. 应用程序设置

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
   ```
   - Id: 唯一标识符
   - Title: 会话标题
   - CreatedAt: 创建时间
   - UpdatedAt: 更新时间
   - Messages: 聊天消息列表
   ```

2. **聊天消息 (ChatMessage)**
   ```
   - Id: 唯一标识符
   - Role: 消息角色 (用户/助手/系统)
   - Content: 消息内容
   - CreatedAt: 创建时间
   - UpdatedAt: 更新时间
   - Timestamp: 时间戳
   - ModelId: 使用的模型ID
   ```

3. **渠道 (Channel)**
   ```
   - Id: 唯一标识符
   - Name: 渠道名称
   - ProviderType: 提供商类型
   - ApiKey: API密钥
   - ApiHost: API主机地址
   - IsEnabled: 是否启用
   - UseStreamResponse: 是否使用流式响应
   - SupportedModels: 支持的模型列表
   - CreatedAt: 创建时间
   - UpdatedAt: 更新时间
   ```

4. **模型信息 (ModelInfo)**
   ```
   - Id: 唯一标识符
   - Name: 模型名称
   - ProviderType: 提供商类型
   - Category: 模型类别 (聊天/嵌入/图像生成)
   - ContextLength: 上下文长度
   - TokenPrice: 令牌价格
   - Enabled: 是否启用
   ```

### 数据关系

```
ChatSession 1────*  ChatMessage
    |
    | (使用)
    ↓
Channel ────* ModelInfo
```

- 一个聊天会话包含多条聊天消息
- 发送消息时使用特定渠道和模型
- 一个渠道支持多个模型

## 控件间数据交流

### 主窗体与业务逻辑层交互

主窗体 (`LlmAgentMainForm`) 持有各业务逻辑管理器的实例：

```
LlmAgentMainForm
  ├── _chatHistoryManager : ChatHistoryManager
  ├── _channelManager : ChannelManager
  ├── _channelService : ChannelService 
  └── _providerFactory : ProviderFactory
```

### 数据流动路径

#### 1. 发送聊天消息的数据流

```
┌─────────────┐          ┌─────────────────────┐          ┌─────────────────┐          ┌────────────────┐
│ 发送按钮点击 │──────────▶│  SendMessage()方法   │──────────▶│ ChatHistoryManager │──────────▶│ ChatRepository │
└─────────────┘          └─────────────────────┘          └─────────────────┘          └────────────────┘
                             │                                                                    │
                             ▼                                                                    ▼
                         ┌───────────┐                                                      ┌──────────┐
                         │创建消息对象│                                                      │SQLite数据库│
                         └───────────┘                                                      └──────────┘
                             │
                             ▼
                         ┌───────────────┐
                         │调用LLM API获取回复│
                         └───────────────┘
                             │
                             ▼
                         ┌───────────────┐
                         │   更新UI显示   │
                         └───────────────┘
```

#### 2. 添加新渠道的数据流

```
┌─────────────────┐       ┌──────────────────────┐       ┌────────────────┐       ┌────────────┐
│ 添加渠道按钮点击  │──────▶│ addChannelButton_Click │──────▶│ ChannelManager │──────▶│ JSON文件存储 │
└─────────────────┘       └──────────────────────┘       └────────────────┘       └────────────┘
                              │
                              ▼
                         ┌──────────────┐
                         │创建Channel对象│
                         └──────────────┘
                              │
                              ▼
                         ┌────────────────┐
                         │更新UI显示新渠道 │
                         └────────────────┘
```

#### 3. 切换聊天会话的数据流

```
┌─────────────┐       ┌───────────────┐       ┌─────────────────┐       ┌────────────────┐
│ 会话项点击  │──────▶│ SwitchToChat() │──────▶│ChatHistoryManager│──────▶│ ChatRepository │
└─────────────┘       └───────────────┘       └─────────────────┘       └────────────────┘
                           │                                                     │
                           ▼                                                     ▼
                      ┌──────────────────┐                                  ┌──────────┐
                      │更新UI显示会话内容 │                                  │SQLite数据库│
                      └──────────────────┘                                  └──────────┘
``` 