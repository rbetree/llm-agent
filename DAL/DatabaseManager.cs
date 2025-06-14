using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.IO;
using llm_agent.Models;
using llm_agent.Model;
using MySql.Data.MySqlClient;

namespace llm_agent.DAL
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (DatabaseConfig.DatabaseType == DatabaseType.MySQL)
            {
                InitializeMySqlDatabase();
            }
            else
            {
                InitializeSqliteDatabase();
            }
        }

        private void InitializeMySqlDatabase()
        {
            try
            {
                // 检查数据库是否存在，如果不存在则创建
                using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString.Replace($"Database={DatabaseConfig.MySqlDatabase};", "")))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (MySqlException ex)
                    {
                        // 处理连接错误
                        string errorMessage = $"无法连接到MySQL服务器: {ex.Message}\n\n" +
                                            $"请检查以下设置:\n" +
                                            $"- MySQL服务是否已启动\n" +
                                            $"- 服务器地址: {DatabaseConfig.MySqlServer}\n" +
                                            $"- 端口: {DatabaseConfig.MySqlPort}\n" +
                                            $"- 用户名: {DatabaseConfig.MySqlUsername}\n" +
                                            $"- 密码是否正确\n\n" +
                                            $"应用程序将切换到SQLite模式。";
                        
                        System.Windows.Forms.MessageBox.Show(
                            errorMessage,
                            "MySQL连接失败",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error);
                        
                        // 切换到SQLite模式
                        DatabaseConfig.DatabaseType = DatabaseType.SQLite;
                        return;
                    }
                    
                    string createDatabaseSql = $"CREATE DATABASE IF NOT EXISTS `{DatabaseConfig.MySqlDatabase}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                    using (var command = new MySqlCommand(createDatabaseSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                // 连接到创建的数据库并创建表
                using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (MySqlException ex)
                    {
                        // 处理连接到指定数据库的错误
                        string errorMessage = $"无法连接到MySQL数据库 '{DatabaseConfig.MySqlDatabase}': {ex.Message}\n\n" +
                                            $"应用程序将切换到SQLite模式。";
                        
                        System.Windows.Forms.MessageBox.Show(
                            errorMessage,
                            "MySQL数据库连接失败",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error);
                        
                        // 切换到SQLite模式
                        DatabaseConfig.DatabaseType = DatabaseType.SQLite;
                        return;
                    }

                    // 创建模型表
                    string createModelsTableSql = @"
                        CREATE TABLE IF NOT EXISTS `Models` (
                            `Id` VARCHAR(255) PRIMARY KEY,
                            `Name` VARCHAR(255) NOT NULL,
                            `ProviderType` VARCHAR(50) NOT NULL,
                            `Category` INT NOT NULL,
                            `ContextLength` INT NULL,
                            `TokenPrice` DECIMAL(10,6) NULL,
                            `Enabled` TINYINT NOT NULL DEFAULT 1
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createModelsTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 创建提示词表
                    string createPromptsTableSql = @"
                        CREATE TABLE IF NOT EXISTS `Prompts` (
                            `Id` VARCHAR(255) PRIMARY KEY,
                            `Title` VARCHAR(255) NOT NULL,
                            `Content` TEXT NOT NULL,
                            `Category` VARCHAR(50) NOT NULL,
                            `CreatedAt` DATETIME NOT NULL,
                            `UpdatedAt` DATETIME NOT NULL,
                            `UsageCount` INT DEFAULT 0
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createPromptsTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 创建AI网站表
                    string createAiWebsitesTableSql = @"
                        CREATE TABLE IF NOT EXISTS `AiWebsites` (
                            `Id` VARCHAR(255) PRIMARY KEY,
                            `Name` VARCHAR(255) NOT NULL,
                            `Description` TEXT,
                            `Url` VARCHAR(1024) NOT NULL,
                            `IconUrl` VARCHAR(1024),
                            `Category` VARCHAR(50),
                            `SortOrder` INT DEFAULT 0,
                            `IsActive` TINYINT DEFAULT 1,
                            `CreatedAt` DATETIME NOT NULL,
                            `UpdatedAt` DATETIME NOT NULL,
                            `LastVisitedAt` DATETIME
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createAiWebsitesTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 创建网站凭据表
                    string createWebsiteCredentialsTableSql = @"
                        CREATE TABLE IF NOT EXISTS `WebsiteCredentials` (
                            `Id` VARCHAR(255) PRIMARY KEY,
                            `WebsiteId` VARCHAR(255) NOT NULL,
                            `Username` VARCHAR(255),
                            `Password` VARCHAR(255),
                            `Notes` TEXT,
                            `CreatedAt` DATETIME NOT NULL,
                            `UpdatedAt` DATETIME NOT NULL,
                            FOREIGN KEY (`WebsiteId`) REFERENCES `AiWebsites`(`Id`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createWebsiteCredentialsTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 创建渠道表
                    string createChannelsTableSql = @"
                        CREATE TABLE IF NOT EXISTS `Channels` (
                            `Id` VARCHAR(255) PRIMARY KEY,
                            `Name` VARCHAR(255) NOT NULL,
                            `ProviderType` VARCHAR(50) NOT NULL,
                            `ApiKey` VARCHAR(255),
                            `ApiHost` VARCHAR(255),
                            `IsEnabled` TINYINT NOT NULL DEFAULT 1,
                            `UseStreamResponse` TINYINT NOT NULL DEFAULT 1,
                            `CreatedAt` DATETIME NOT NULL,
                            `UpdatedAt` DATETIME NOT NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createChannelsTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 创建渠道模型表
                    string createChannelModelsTableSql = @"
                        CREATE TABLE IF NOT EXISTS `ChannelModels` (
                            `Id` INT AUTO_INCREMENT PRIMARY KEY,
                            `ChannelId` VARCHAR(255) NOT NULL,
                            `ModelName` VARCHAR(255) NOT NULL,
                            FOREIGN KEY (`ChannelId`) REFERENCES `Channels`(`Id`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createChannelModelsTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 创建聊天会话表
                    string createSessionTableSql = @"
                        CREATE TABLE IF NOT EXISTS `ChatSessions` (
                            `Id` VARCHAR(255) PRIMARY KEY,
                            `Title` VARCHAR(255) NOT NULL,
                            `CreatedAt` DATETIME NOT NULL,
                            `UpdatedAt` DATETIME NOT NULL,
                            `OrderIndex` INT DEFAULT 0
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createSessionTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    // 创建聊天消息表
                    string createMessagesTableSql = @"
                        CREATE TABLE IF NOT EXISTS `ChatMessages` (
                            `Id` INT AUTO_INCREMENT PRIMARY KEY,
                            `SessionId` VARCHAR(255) NOT NULL,
                            `Role` VARCHAR(50) NOT NULL,
                            `Content` TEXT NOT NULL,
                            `Timestamp` DATETIME NOT NULL,
                            FOREIGN KEY (`SessionId`) REFERENCES `ChatSessions`(`Id`) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var command = new MySqlCommand(createMessagesTableSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"初始化MySQL数据库失败: {ex.Message}\n\n应用程序将切换到SQLite模式。";
                System.Windows.Forms.MessageBox.Show(
                    errorMessage,
                    "MySQL初始化失败",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                
                // 切换到SQLite模式
                DatabaseConfig.DatabaseType = DatabaseType.SQLite;
                
                // 初始化SQLite数据库作为备用
                InitializeSqliteDatabase();
            }
        }

        private void InitializeSqliteDatabase()
        {
            if (!File.Exists(DatabaseConfig.GetSqliteDbPath()))
            {
                SQLiteConnection.CreateFile(DatabaseConfig.GetSqliteDbPath());
            }

            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();

                // 创建模型表
                string createModelsTableSql = @"
                    CREATE TABLE IF NOT EXISTS Models (
                        Id TEXT PRIMARY KEY,
                        Name TEXT NOT NULL,
                        ProviderType TEXT NOT NULL,
                        Category INTEGER NOT NULL,
                        ContextLength INTEGER,
                        TokenPrice REAL,
                        Enabled INTEGER NOT NULL DEFAULT 1
                    );";

                using (var command = new SQLiteCommand(createModelsTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // 创建提示词表
                string createPromptsTableSql = @"
                    CREATE TABLE IF NOT EXISTS Prompts (
                        Id TEXT PRIMARY KEY,
                        Title TEXT NOT NULL,
                        Content TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NOT NULL,
                        UsageCount INTEGER DEFAULT 0
                    );";

                using (var command = new SQLiteCommand(createPromptsTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // 创建AI网站表
                string createAiWebsitesTableSql = @"
                    CREATE TABLE IF NOT EXISTS AiWebsites (
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
                    );";

                using (var command = new SQLiteCommand(createAiWebsitesTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // 创建网站凭据表
                string createWebsiteCredentialsTableSql = @"
                    CREATE TABLE IF NOT EXISTS WebsiteCredentials (
                        Id TEXT PRIMARY KEY,
                        WebsiteId TEXT NOT NULL,
                        Username TEXT,
                        Password TEXT,
                        Notes TEXT,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NOT NULL,
                        FOREIGN KEY (WebsiteId) REFERENCES AiWebsites(Id) ON DELETE CASCADE
                    );";

                using (var command = new SQLiteCommand(createWebsiteCredentialsTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // 创建渠道表
                string createChannelsTableSql = @"
                    CREATE TABLE IF NOT EXISTS Channels (
                        Id TEXT PRIMARY KEY,
                        Name TEXT NOT NULL,
                        ProviderType TEXT NOT NULL,
                        ApiKey TEXT,
                        ApiHost TEXT,
                        IsEnabled INTEGER NOT NULL DEFAULT 1,
                        UseStreamResponse INTEGER NOT NULL DEFAULT 1,
                        CreatedAt TEXT NOT NULL,
                        UpdatedAt TEXT NOT NULL
                    );";

                using (var command = new SQLiteCommand(createChannelsTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // 创建渠道模型表
                string createChannelModelsTableSql = @"
                    CREATE TABLE IF NOT EXISTS ChannelModels (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        ChannelId TEXT NOT NULL,
                        ModelName TEXT NOT NULL,
                        FOREIGN KEY (ChannelId) REFERENCES Channels(Id) ON DELETE CASCADE
                    );";

                using (var command = new SQLiteCommand(createChannelModelsTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveModels(List<ModelInfo> models, string providerType)
        {
            if (models == null || models.Count == 0)
                return;

            if (DatabaseConfig.DatabaseType == DatabaseType.MySQL)
            {
                SaveModelsToMySql(models, providerType);
            }
            else
            {
                SaveModelsToSqlite(models, providerType);
            }
        }

        private void SaveModelsToMySql(List<ModelInfo> models, string providerType)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 删除该提供商的所有现有模型记录
                        string deleteSql = "DELETE FROM `Models` WHERE `ProviderType` = @providerType";
                        using (var command = new MySqlCommand(deleteSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@providerType", providerType.ToLower());
                            command.ExecuteNonQuery();
                        }

                        // 插入新的模型记录
                        string insertSql = @"
                            INSERT INTO `Models` (`Id`, `Name`, `ProviderType`, `Category`, `ContextLength`, `TokenPrice`, `Enabled`)
                            VALUES (@id, @name, @providerType, @category, @contextLength, @tokenPrice, @enabled)";

                        foreach (var model in models)
                        {
                            using (var command = new MySqlCommand(insertSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", model.Id);
                                command.Parameters.AddWithValue("@name", model.Name);
                                command.Parameters.AddWithValue("@providerType", model.ProviderType.ToLower());
                                command.Parameters.AddWithValue("@category", (int)model.Category);
                                command.Parameters.AddWithValue("@contextLength", model.ContextLength.HasValue ? (object)model.ContextLength.Value : DBNull.Value);
                                command.Parameters.AddWithValue("@tokenPrice", model.TokenPrice.HasValue ? (object)model.TokenPrice.Value : DBNull.Value);
                                command.Parameters.AddWithValue("@enabled", model.Enabled ? 1 : 0);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void SaveModelsToSqlite(List<ModelInfo> models, string providerType)
        {
            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 删除该提供商的所有现有模型记录
                        string deleteSql = "DELETE FROM Models WHERE ProviderType = @providerType";
                        using (var command = new SQLiteCommand(deleteSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@providerType", providerType.ToLower());
                            command.ExecuteNonQuery();
                        }

                        // 插入新的模型记录
                        string insertSql = @"
                            INSERT INTO Models (Id, Name, ProviderType, Category, ContextLength, TokenPrice, Enabled)
                            VALUES (@id, @name, @providerType, @category, @contextLength, @tokenPrice, @enabled)";

                        foreach (var model in models)
                        {
                            using (var command = new SQLiteCommand(insertSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", model.Id);
                                command.Parameters.AddWithValue("@name", model.Name);
                                command.Parameters.AddWithValue("@providerType", model.ProviderType.ToLower());
                                command.Parameters.AddWithValue("@category", (int)model.Category);
                                command.Parameters.AddWithValue("@contextLength", model.ContextLength.HasValue ? (object)model.ContextLength.Value : DBNull.Value);
                                command.Parameters.AddWithValue("@tokenPrice", model.TokenPrice.HasValue ? (object)model.TokenPrice.Value : DBNull.Value);
                                command.Parameters.AddWithValue("@enabled", model.Enabled ? 1 : 0);
                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<ModelInfo> GetModels(string providerType)
        {
            return DatabaseConfig.DatabaseType == DatabaseType.MySQL ? 
                GetModelsFromMySql(providerType) : 
                GetModelsFromSqlite(providerType);
        }

        private List<ModelInfo> GetModelsFromMySql(string providerType)
        {
            List<ModelInfo> models = new List<ModelInfo>();

            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM `Models` WHERE `ProviderType` = @providerType";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@providerType", providerType.ToLower());

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = new ModelInfo(
                                reader["Id"].ToString(),
                                reader["Name"].ToString(),
                                reader["ProviderType"].ToString(),
                                (ModelCategory)Convert.ToInt32(reader["Category"])
                            );

                            if (!reader.IsDBNull(reader.GetOrdinal("ContextLength")))
                                model.ContextLength = Convert.ToInt32(reader["ContextLength"]);

                            if (!reader.IsDBNull(reader.GetOrdinal("TokenPrice")))
                                model.TokenPrice = Convert.ToDouble(reader["TokenPrice"]);

                            model.Enabled = Convert.ToBoolean(Convert.ToInt32(reader["Enabled"]));

                            models.Add(model);
                        }
                    }
                }
            }

            return models;
        }

        private List<ModelInfo> GetModelsFromSqlite(string providerType)
        {
            List<ModelInfo> models = new List<ModelInfo>();

            using (var connection = new SQLiteConnection(DatabaseConfig.SqliteConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Models WHERE ProviderType = @providerType";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@providerType", providerType.ToLower());

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var model = new ModelInfo(
                                reader["Id"].ToString(),
                                reader["Name"].ToString(),
                                reader["ProviderType"].ToString(),
                                (ModelCategory)Convert.ToInt32(reader["Category"])
                            );

                            if (!reader.IsDBNull(reader.GetOrdinal("ContextLength")))
                                model.ContextLength = Convert.ToInt32(reader["ContextLength"]);

                            if (!reader.IsDBNull(reader.GetOrdinal("TokenPrice")))
                                model.TokenPrice = Convert.ToDouble(reader["TokenPrice"]);

                            model.Enabled = Convert.ToBoolean(Convert.ToInt32(reader["Enabled"]));

                            models.Add(model);
                        }
                    }
                }
            }

            return models;
        }
    }
}