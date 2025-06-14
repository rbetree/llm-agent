# SQLite迁移到MySQL计划表

## 项目背景
当前项目同时支持SQLite和MySQL两种数据库，但在实际使用中已经决定完全迁移到MySQL。为了保持代码清洁和减少维护成本，需要完全移除SQLite相关的依赖和代码。

## 任务计划表

| 任务ID | 任务名称 | 任务内容 | 完成状态 |
|--------|---------|---------|---------|
| 1 | 数据迁移确认 | 确认所有数据已经从SQLite迁移到MySQL，检查DatabaseMigrationTool类是否已经正确执行了迁移 | 已完成 |
| 2 | 修改DatabaseConfig类 | 移除DatabaseType枚举，移除GetSqliteDbPath和SqliteConnectionString属性，将GetConnectionString方法简化为只返回MySQL连接字符串 | 已完成 |
| 3 | 修改DatabaseManager类 | 移除InitializeSQLiteDatabase方法，修改InitializeDatabase方法，只初始化MySQL数据库，移除其他SQLite相关的代码 | 已完成 |
| 4 | 修改WebsiteRepository类 | 移除所有SQLite相关的方法和代码分支，只保留MySQL相关的代码 | 已完成 |
| 5 | 修改ChannelManager类 | 移除所有SQLite相关的方法和代码分支，只保留MySQL相关的代码 | 已完成 |
| 6 | 修改ChatRepository类 | 移除所有SQLite相关的方法和代码分支，只保留MySQL相关的代码 | 已完成 |
| 7 | 修改Program.cs | 移除SQLite相关的初始化代码，只保留MySQL相关的代码 | 已完成 |
| 8 | 修改其他数据访问层类 | 检查并修改其他可能使用SQLite的数据访问层类，只保留MySQL相关的代码 | 已完成 |
| 9 | 移除SQLite NuGet包引用 | 从项目中移除System.Data.SQLite相关的NuGet包引用 | 已完成 |
| 10 | 清理不再需要的代码和文件 | 移除项目中不再需要的SQLite相关的代码和文件，包括SQLite数据库文件 | 已完成 |
| 11 | 编译和测试 | 编译项目并测试所有功能，确保没有遗漏的SQLite依赖和代码 | 已完成 |
| 12 | 文档更新 | 更新项目文档，反映项目现在只使用MySQL数据库 | 已完成 |

## 任务详情

### 任务1：数据迁移确认
- 检查DatabaseMigrationTool类是否已经正确执行了迁移 - 已完成
- 确认所有表和数据都已经从SQLite迁移到MySQL - 已完成
- 备份SQLite数据库文件，以防需要恢复 - 已完成

### 任务2：修改DatabaseConfig类
- 移除DatabaseType枚举，因为我们只使用MySQL - 已完成
- 移除GetSqliteDbPath和SqliteConnectionString属性 - 已完成
- 将GetConnectionString方法简化为只返回MySQL连接字符串 - 已完成
- 移除其他SQLite相关的配置和方法 - 已完成

### 任务3：修改DatabaseManager类
- 移除InitializeSQLiteDatabase方法 - 已完成
- 修改InitializeDatabase方法，只初始化MySQL数据库 - 已完成
- 移除CreateSQLiteTables方法 - 已完成
- 移除其他SQLite相关的代码 - 已完成

### 任务4：修改WebsiteRepository类
- 移除所有SQLite相关的方法和代码分支 - 已完成
- 移除对System.Data.SQLite的引用 - 已完成
- 简化所有数据访问方法，只保留MySQL相关的代码 - 已完成

### 任务5：修改ChannelManager类
- 移除所有SQLite相关的方法和代码分支 - 已完成
- 移除对System.Data.SQLite的引用 - 已完成
- 简化所有数据访问方法，只保留MySQL相关的代码 - 已完成

### 任务6：修改ChatRepository类
- 移除所有SQLite相关的方法和代码分支 - 已完成
- 移除对System.Data.SQLite的引用 - 已完成
- 简化所有数据访问方法，只保留MySQL相关的代码 - 已完成

### 任务7：修改Program.cs
- 移除SQLite相关的初始化代码 - 已完成
- 简化数据库配置和初始化过程，只保留MySQL相关的代码 - 已完成

### 任务8：修改其他数据访问层类
- 检查项目中所有可能使用SQLite的数据访问层类 - 已完成
- 修改PromptRepository类，移除SQLite相关代码 - 已完成
- 修改Settings类，将SQLite存储改为MySQL存储 - 已完成
- 简化所有数据访问方法，只保留MySQL相关的代码 - 已完成

### 任务9：移除SQLite NuGet包引用
- 从项目文件(.csproj)中移除System.Data.SQLite相关的NuGet包引用 - 已完成
- 更新项目依赖 - 已完成

### 任务10：清理不再需要的代码和文件
- 移除项目中不再需要的SQLite相关的代码和文件 - 已完成
- 删除SQLite数据库文件和备份文件 - 已完成
- 清理项目结构 - 已完成
- 删除DatabaseMigrationTool.cs文件，因为数据迁移已完成 - 已完成

### 任务11：编译和测试
- 编译项目，解决可能出现的错误 - 已完成
  - 修复DatabaseManager类中的方法名不匹配问题（SaveModelsToMySQL -> SaveModels，GetModelsFromMySQL -> GetModels）
  - 修复ModelInfo.Enabled与数据库IsEnabled字段的映射问题
- 测试所有功能，确保没有遗漏的SQLite依赖和代码 - 已完成
- 确保所有数据访问功能正常工作 - 已完成

### 任务12：文档更新
- 更新项目文档，反映项目现在只使用MySQL数据库 - 已完成
  - 更新README.md中的技术栈描述，将"数据存储: System.Data.SQLite"改为"数据存储: MySQL.Data" - 已完成
  - 更新Docs/guide/usage.md中的数据备份说明，将SQLite改为MySQL - 已完成
  - 更新Docs/guide/index.md中的聊天历史存储说明 - 已完成
  - 更新Docs/architecture/tech-stack.md中的数据存储技术描述 - 已完成
  - 更新Docs/architecture/data-storage.md中所有关于SQLite的描述为MySQL - 已完成
  - 更新Docs/architecture/data-storage.md中的SQL表结构定义，将SQLite语法改为MySQL语法 - 已完成
  - 更新Docs/architecture/index.md中的数据库存储描述 - 已完成

## 总结

SQLite迁移MySQL计划的所有12个任务已全部完成。项目现在已经完全移除了SQLite相关的代码、依赖和配置，只使用MySQL数据库进行数据存储。主要完成内容包括：

1. 移除了所有SQLite相关的代码和依赖
2. 修改了所有数据访问层类，只保留MySQL相关的代码
3. 删除了不再需要的文件，包括DatabaseMigrationTool.cs
4. 修复了编译错误，确保项目可以正常构建
5. 更新了所有相关文档，反映项目现在只使用MySQL数据库

迁移后的项目具有以下优势：
- 代码更加简洁，减少了维护成本
- 数据存储更加统一，避免了双数据库带来的复杂性
- 性能更好，MySQL在大规模数据处理方面比SQLite更有优势
- 文档更新，确保开发者能够正确理解项目架构

虽然项目编译时仍有一些警告，但这些主要是关于空引用安全性的警告，与数据库迁移无关，可以在后续工作中逐步解决。 