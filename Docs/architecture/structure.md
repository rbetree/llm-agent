# LLM Agent项目结构与设计说明

## 目录结构

项目采用经典三层架构设计，清晰分离UI、业务逻辑和数据访问层：

```
llm-agent/
├── UI/                 # 界面展示层
│   ├── Controls/       # 自定义控件
│   └── Forms/          # 窗体
│       ├── LlmAgentMainForm.cs          # 主窗体
│       ├── LlmAgentMainForm.Designer.cs # 主窗体设计器代码
│       ├── LlmAgentMainForm.resx        # 主窗体资源文件
│       └── ...
├── BLL/                # 业务逻辑层
│   ├── ChatManager.cs  # 聊天管理
│   ├── HistoryManager.cs # 历史记录管理
│   └── ...
├── DAL/                # 数据访问层
│   ├── ChatRepository.cs  # 聊天数据存取
│   └── ...
├── Model/              # 数据实体类
│   ├── ChatMessage.cs  # 聊天消息模型
│   ├── ChatSession.cs  # 会话模型
│   └── LLMModel.cs     # 模型定义
├── API/                # 第三方API接口
│   ├── Provider/       # LLM提供商
│   │   ├── ILLMProvider.cs  # 提供商接口
│   │   ├── ProviderFactory.cs 
│   │   ├── OpenAIProvider.cs
│   │   └── ...
│   └── ...
├── Common/             # 通用工具类
│   ├── Utils/          # 工具类
│   ├── Extensions/     # 扩展方法
│   ├── Exceptions/     # 自定义异常类
│   └── Program.cs      # 程序入口点
├── Docs/               # 项目文档
│   ├── 项目结构与设计说明.md # 本文档：项目架构与设计说明
│   ├── ui设计.md         # UI设计文档
│   └── 交互逻辑和UI说明.md # 交互逻辑说明
└── llm-agent.csproj    # 项目文件
```

## 命名空间结构

项目使用分层命名空间组织代码：

- `llm_agent` - 根命名空间
  - `llm_agent.UI` - 界面展示层
    - `llm_agent.UI.Forms` - 窗体实现
    - `llm_agent.UI.Controls` - 自定义控件
  - `llm_agent.BLL` - 业务逻辑层
  - `llm_agent.DAL` - 数据访问层
  - `llm_agent.Model` - 数据实体类
  - `llm_agent.API` - 第三方API接口
    - `llm_agent.API.Provider` - LLM服务提供商
  - `llm_agent.Common` - 通用工具和辅助类
    - `llm_agent.Common.Exceptions` - 自定义异常类

## 各层职责说明

### 1. UI层（界面展示层）

包含应用程序的界面和用户交互实现：
- 负责展示数据和接收用户输入
- 处理界面事件并调用业务逻辑层方法
- 不包含业务逻辑，仅负责界面呈现和初步输入验证
- 使用委托和事件实现与业务层通信

主要组件：
- `Forms/LlmAgentMainForm.cs` - 主窗体及事件处理
- `Controls/` - 自定义控件实现

### 2. BLL层（业务逻辑层）

实现应用程序的核心业务逻辑：
- 处理复杂业务规则和工作流
- 协调UI层和DAL层之间的数据交互
- 执行数据验证和处理
- 实现异步操作和错误处理

主要组件：
- `ChatManager.cs` - 管理聊天业务逻辑
- `HistoryManager.cs` - 历史记录管理

### 3. DAL层（数据访问层）

负责数据的持久化和访问：
- 封装所有数据库操作
- 提供统一的数据访问接口
- 隔离数据存储实现细节
- 实现事务管理和并发控制

主要组件：
- `ChatRepository.cs` - 聊天数据存取实现

### 4. Model层（数据实体类）

定义贯穿各层的数据实体：
- 数据结构定义
- 不包含业务逻辑
- 在各层间传递数据
- 包含数据验证属性

主要组件：
- `ChatMessage.cs` - 定义聊天消息和相关模型
- `ChatSession.cs` - 定义会话模型
- `LLMModel.cs` - 定义LLM模型相关信息

### 5. API层（第三方接口）

封装与外部服务的交互：
- 实现与各LLM服务商的通信
- 处理API调用和响应
- 统一异常处理和错误管理
- 支持异步操作模式

主要组件：
- `Provider/ILLMProvider.cs` - 定义提供商接口
- `Provider/ProviderFactory.cs` - 工厂模式创建提供商实例
- 各提供商实现类 - 根据不同API规范实现通信

### 6. Common层（通用工具）

提供各层共用的工具类和辅助方法：
- 通用工具函数
- 扩展方法
- 自定义异常类
- 程序入口点

主要组件：
- `Utils/` - 通用工具实现
- `Extensions/` - 扩展方法
- `Exceptions/` - 自定义异常定义
- `Program.cs` - 程序入口点

## 设计原则

1. **分层结构清晰**：
   - UI、业务逻辑、数据访问职责分明
   - 各层通过接口通信，降低耦合度
   - 遵循单一职责原则

2. **UI与业务逻辑分离**：
   - UI布局和控件定义通过设计器文件维护
   - 业务逻辑在BLL层实现，避免UI层包含复杂逻辑
   - UI变更应先更新设计文档，再修改代码
   - 使用LlmAgentMainForm.Designer.cs和LlmAgentMainForm.cs职责分离模式

3. **基于接口的提供商设计**：
   - 所有LLM提供商实现共同接口
   - 使用工厂模式创建具体实现
   - 允许运行时切换提供商

4. **数据实体贯穿架构**：
   - Model层定义的实体类贯穿所有层
   - 各层间通过Model传递数据

## 代码约定

1. **命名规范**：
   - 使用驼峰命名法命名私有字段，前缀使用下划线(_)
   - 使用Pascal命名法命名方法、属性和类
   - 使用var关键字声明局部变量，除非变量类型不明显
   
2. **控件访问规则**：
   - 使用Controls集合与索引或Controls.Find查找深层嵌套控件
   - 总是在访问控件前添加null检查
   - 避免直接访问控件，使用正确的控件路径
   - 使用类型强制转换前验证类型兼容性
   
3. **异步编程规范**：
   - 使用async/await关键字实现异步操作
   - 方法名后缀使用Async表示异步方法
   - 避免使用.Result或.Wait()阻塞异步操作
   - 正确处理异步操作中的异常
   - 确保UI线程安全更新

## 异常处理策略

1. **异常层次结构**：
   ```
   Exception (系统)
     ├── LLMAgentException (应用基础异常)
     │     ├── ApiException (API调用异常)
     │     ├── DataAccessException (数据访问异常)
     │     ├── ConfigurationException (配置异常)
     │     └── UserInputException (用户输入异常)
     └── 其他系统异常
   ```

2. **异常处理原则**：
   - 在适当的层次处理异常
   - 避免吞噬异常，确保异常信息传递
   - 使用自定义异常类型增加语义
   - 记录异常信息到日志
   - 向用户提供友好的错误信息 