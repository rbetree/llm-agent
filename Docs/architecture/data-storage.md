# 数据存储架构

本文档详细介绍项目的数据存储架构设计、用户数据与应用数据分离方案、数据结构

### 主要技术栈

- **SQLite数据库**：轻量级嵌入式关系型数据库
  - 优势：零配置、自包含、跨平台、性能优良
  - 用途：存储结构化数据（聊天历史、模型信息、渠道配置等）

- **.NET Settings API**：应用程序设置管理机制
  - 优势：与.NET框架紧密集成、使用简便
  - 用途：存储用户首选项和应用配置

### 技术选择理由

- **为何选择SQLite**：作为嵌入式数据库，SQLite无需额外服务器进程，适合桌面应用；同时提供了完整的SQL功能，支持复杂查询和事务处理。通过统一使用SQLite存储所有结构化数据，简化了数据管理和备份流程。

- **为何使用Settings API**：对于简单的用户偏好设置，.NET内置的设置机制提供了便捷的访问和持久化方案。

## 用户数据与应用数据分离

### 用户数据

用户特定的、需要隔离保存的数据：

1. **聊天历史**
   - 存储位置：SQLite数据库（ChatSessions和ChatMessages表）
   - 数据特点：用户生成的内容，隐私敏感
   - 访问方式：通过ChatRepository访问

2. **渠道配置**
   - 存储位置：SQLite数据库（Channels和ChannelModels表）
   - 数据特点：包含API密钥等敏感信息
   - 访问方式：通过ChannelManager访问

3. **用户偏好设置**
   - 存储位置：SQLite数据库（UserSettings表）
   - 数据特点：用户个性化配置
   - 访问方式：通过Settings类访问

4. **网站凭据**
   - 存储位置：SQLite数据库（WebsiteCredentials表）
   - 数据特点：高度敏感的认证信息，需加密存储
   - 访问方式：通过WebsiteRepository访问，使用EncryptionHelper加解密

### 应用数据

应用程序通用的、与用户无关的数据：

1. **模型信息**
   - 存储位置：SQLite数据库（Models表）
   - 数据特点：应用程序通用配置，所有用户共享
   - 访问方式：通过DatabaseManager访问

2. **提示词模板**
   - 存储位置：SQLite数据库（Prompts表）
   - 数据特点：预定义的模板，可被所有用户使用
   - 访问方式：通过PromptRepository访问

3. **AI网站目录**
   - 存储位置：SQLite数据库（AiWebsites表）
   - 数据特点：公共资源目录
   - 访问方式：通过WebsiteRepository访问

### 分离实现机制

- **存储位置分离**：用户数据存储在用户目录，应用数据存储在应用目录
- **加密保护**：敏感用户数据（如API密钥、网站凭据）使用加密存储

## 数据库表结构设计

### 核心数据表

#### 1. ChatSessions表
```sql
CREATE TABLE ChatSessions (
    Id TEXT PRIMARY KEY,
    Title TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    OrderIndex INTEGER DEFAULT 0
);
```

#### 2. ChatMessages表
```sql
CREATE TABLE ChatMessages (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    SessionId TEXT NOT NULL,
    Role TEXT NOT NULL,
    Content TEXT NOT NULL,
    Timestamp TEXT NOT NULL,
    FOREIGN KEY (SessionId) REFERENCES ChatSessions(Id) ON DELETE CASCADE
);
```

#### 3. Models表
```sql
CREATE TABLE Models (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    ProviderType TEXT NOT NULL,
    ContextLength INTEGER,
    TokenPrice REAL,
    Enabled INTEGER NOT NULL DEFAULT 1
);
```

#### 4. Users表
```sql
CREATE TABLE Users (
    Id TEXT PRIMARY KEY,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    Salt TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    LastLoginAt TEXT,
    IsAdmin INTEGER NOT NULL DEFAULT 0
);
```

#### 5. LoggedInUsers表
```sql
CREATE TABLE LoggedInUsers (
    UserId TEXT PRIMARY KEY,
    LastLoginAt TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
```

#### 6. UserSettings表
```sql
CREATE TABLE UserSettings (
    Key TEXT PRIMARY KEY,
    Value TEXT
);
```

#### 7. Prompts表
```sql
CREATE TABLE Prompts (
    Id TEXT PRIMARY KEY,
    Title TEXT NOT NULL,
    Content TEXT NOT NULL,
    Category TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    UsageCount INTEGER DEFAULT 0
);
```

#### 8. AiWebsites表
```sql
CREATE TABLE AiWebsites (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    Description TEXT,
    Url TEXT NOT NULL,
    IconUrl TEXT,
    Category TEXT,
    SortOrder INTEGER DEFAULT 0,
    IsActive INTEGER DEFAULT 1,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    LastVisitedAt TEXT
);
```

#### 9. WebsiteCredentials表
```sql
CREATE TABLE WebsiteCredentials (
    Id TEXT PRIMARY KEY,
    WebsiteId TEXT NOT NULL,
    Username TEXT,
    Password TEXT,
    Notes TEXT,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    FOREIGN KEY (WebsiteId) REFERENCES AiWebsites(Id) ON DELETE CASCADE
);
```

#### 10. Channels表
```sql
CREATE TABLE Channels (
    Id TEXT PRIMARY KEY,
    Name TEXT NOT NULL,
    ProviderType TEXT NOT NULL,
    ApiKey TEXT,
    ApiHost TEXT,
    IsEnabled INTEGER NOT NULL DEFAULT 1,
    UseStreamResponse INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL
);
```

#### 11. ChannelModels表
```sql
CREATE TABLE ChannelModels (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ChannelId TEXT NOT NULL,
    ModelName TEXT NOT NULL,
    FOREIGN KEY (ChannelId) REFERENCES Channels(Id) ON DELETE CASCADE
);
```

## 数据访问层设计

项目采用仓储模式实现数据访问，提供统一的接口并封装数据操作细节：

### 主要仓储类

1. **DatabaseManager**
   - 职责：数据库初始化、表创建、通用操作
   - 关键方法：`InitializeDatabase()`、`SaveModels()`

2. **ChatRepository**
   - 职责：聊天会话和消息的CRUD操作
   - 关键方法：`SaveChatSession()`、`LoadChatSession()`、`GetAllSessions()`

3. **ChannelManager**
   - 职责：渠道配置的读写和持久化
   - 关键方法：`GetAllChannels()`、`AddChannel()`、`UpdateChannel()`

4. **WebsiteRepository**
   - 职责：AI网站信息和凭据管理
   - 关键方法：`SaveWebsite()`、`GetWebsiteById()`、`GetWebsiteCredentials()`

5. **PromptRepository**
   - 职责：提示词模板管理
   - 关键方法：`SavePrompt()`、`GetPromptById()`、`GetPromptsByCategory()`

### 数据流设计

1. **分层访问模式**
   - UI层 → 业务逻辑层 → 数据访问层 → 数据存储
   - 确保关注点分离和职责单一

2. **事务处理**
   - 使用SQLite事务确保数据一致性
   - 示例：聊天会话和消息的同步保存

3. **异常处理**
   - 捕获并转换底层存储异常
   - 提供友好的错误信息和恢复机制

## 数据关系模型

- 一个聊天会话包含多条聊天消息（一对多）
- 一个渠道可以支持多个模型（一对多）
- 一个AI网站可以有多个凭据记录（一对多）