using System;
using System.Collections.Generic;
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
            InitializeMySqlDatabase();
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
                        string createDatabaseSql = $"CREATE DATABASE IF NOT EXISTS `{DatabaseConfig.MySqlDatabase}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;";
                        using (var command = new MySqlCommand(createDatabaseSql, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"创建MySQL数据库时出错: {ex.Message}");
                        throw;
                    }
                }

                // 创建表
                CreateMySqlTables();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化MySQL数据库时出错: {ex.Message}");
                throw;
            }
        }

        private void CreateMySqlTables()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 创建ChatSessions表
                        string createChatSessionsTable = @"
                            CREATE TABLE IF NOT EXISTS `ChatSessions` (
                                `Id` VARCHAR(36) NOT NULL,
                                `Title` VARCHAR(255) NOT NULL,
                                `CreatedAt` DATETIME NOT NULL,
                                `UpdatedAt` DATETIME NOT NULL,
                                `OrderIndex` INT NOT NULL DEFAULT 0,
                                PRIMARY KEY (`Id`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createChatSessionsTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建ChatMessages表
                        string createChatMessagesTable = @"
                            CREATE TABLE IF NOT EXISTS `ChatMessages` (
                                `Id` INT AUTO_INCREMENT NOT NULL,
                                `SessionId` VARCHAR(36) NOT NULL,
                                `Role` VARCHAR(50) NOT NULL,
                                `Content` TEXT NOT NULL,
                                `Timestamp` DATETIME NOT NULL,
                                PRIMARY KEY (`Id`),
                                FOREIGN KEY (`SessionId`) REFERENCES `ChatSessions`(`Id`) ON DELETE CASCADE
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createChatMessagesTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建AiWebsites表
                        string createAiWebsitesTable = @"
                            CREATE TABLE IF NOT EXISTS `AiWebsites` (
                                `Id` VARCHAR(36) NOT NULL,
                                `Name` VARCHAR(255) NOT NULL,
                                `Description` TEXT NULL,
                                `Url` VARCHAR(1000) NOT NULL,
                                `IconUrl` VARCHAR(1000) NULL,
                                `Category` VARCHAR(100) NULL,
                                `SortOrder` INT NOT NULL DEFAULT 0,
                                `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
                                `CreatedAt` DATETIME NOT NULL,
                                `UpdatedAt` DATETIME NOT NULL,
                                `LastVisitedAt` DATETIME NULL,
                                PRIMARY KEY (`Id`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createAiWebsitesTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建WebsiteCredentials表
                        string createWebsiteCredentialsTable = @"
                            CREATE TABLE IF NOT EXISTS `WebsiteCredentials` (
                                `Id` VARCHAR(36) NOT NULL,
                                `WebsiteId` VARCHAR(36) NOT NULL,
                                `Username` VARCHAR(255) NULL,
                                `Password` VARCHAR(1000) NULL,
                                `Notes` TEXT NULL,
                                `CreatedAt` DATETIME NOT NULL,
                                `UpdatedAt` DATETIME NOT NULL,
                                PRIMARY KEY (`Id`),
                                FOREIGN KEY (`WebsiteId`) REFERENCES `AiWebsites`(`Id`) ON DELETE CASCADE
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createWebsiteCredentialsTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建Prompts表
                        string createPromptsTable = @"
                            CREATE TABLE IF NOT EXISTS `Prompts` (
                                `Id` VARCHAR(36) NOT NULL,
                                `Title` VARCHAR(255) NOT NULL,
                                `Content` TEXT NOT NULL,
                                `Category` VARCHAR(100) NOT NULL,
                                `CreatedAt` DATETIME NOT NULL,
                                `UpdatedAt` DATETIME NOT NULL,
                                `UsageCount` INT NOT NULL DEFAULT 0,
                                PRIMARY KEY (`Id`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createPromptsTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建Channels表
                        string createChannelsTable = @"
                            CREATE TABLE IF NOT EXISTS `Channels` (
                                `Id` VARCHAR(36) NOT NULL,
                                `Name` VARCHAR(255) NOT NULL,
                                `ProviderType` VARCHAR(50) NOT NULL,
                                `ApiKey` VARCHAR(1000) NULL,
                                `ApiHost` VARCHAR(255) NULL,
                                `IsEnabled` BOOLEAN NOT NULL DEFAULT TRUE,
                                `UseStreamResponse` BOOLEAN NOT NULL DEFAULT TRUE,
                                `CreatedAt` DATETIME NOT NULL,
                                `UpdatedAt` DATETIME NOT NULL,
                                PRIMARY KEY (`Id`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createChannelsTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建ChannelModels表
                        string createChannelModelsTable = @"
                            CREATE TABLE IF NOT EXISTS `ChannelModels` (
                                `ChannelId` VARCHAR(36) NOT NULL,
                                `ModelName` VARCHAR(255) NOT NULL,
                                PRIMARY KEY (`ChannelId`, `ModelName`),
                                FOREIGN KEY (`ChannelId`) REFERENCES `Channels`(`Id`) ON DELETE CASCADE
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createChannelModelsTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建Models表
                        string createModelsTable = @"
                            CREATE TABLE IF NOT EXISTS `Models` (
                                `Id` VARCHAR(100) NOT NULL,
                                `Name` VARCHAR(255) NOT NULL,
                                `Provider` VARCHAR(50) NOT NULL,
                                `Category` VARCHAR(50) NOT NULL,
                                `ContextLength` INT NULL,
                                `TokenPrice` DECIMAL(10,8) NULL,
                                `IsEnabled` BOOLEAN NOT NULL DEFAULT TRUE,
                                PRIMARY KEY (`Id`, `Provider`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createModelsTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        // 创建Settings表
                        string createSettingsTable = @"
                            CREATE TABLE IF NOT EXISTS `Settings` (
                                `Key` VARCHAR(100) NOT NULL,
                                `Value` TEXT NULL,
                                `Type` VARCHAR(50) NOT NULL,
                                `UpdatedAt` DATETIME NOT NULL,
                                PRIMARY KEY (`Key`)
                            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;";

                        using (var command = new MySqlCommand(createSettingsTable, connection, transaction))
                        {
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"创建MySQL表时出错: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public void SaveModels(List<ModelInfo> models, string provider)
        {
            if (models == null || models.Count == 0) return;

            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 删除该提供商的所有模型
                        string deleteSql = "DELETE FROM Models WHERE Provider = @provider";
                        using (var command = new MySqlCommand(deleteSql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@provider", provider);
                            command.ExecuteNonQuery();
                        }

                        // 插入新模型
                        foreach (var model in models)
                        {
                            string insertSql = @"
                                INSERT INTO Models (Id, Name, Provider, Category, ContextLength, TokenPrice, IsEnabled)
                                VALUES (@id, @name, @provider, @category, @contextLength, @tokenPrice, @isEnabled)";

                            using (var command = new MySqlCommand(insertSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", model.Id);
                                command.Parameters.AddWithValue("@name", model.Name);
                                command.Parameters.AddWithValue("@provider", provider);
                                command.Parameters.AddWithValue("@category", model.Category.ToString());
                                command.Parameters.AddWithValue("@contextLength", model.ContextLength.HasValue ? (object)model.ContextLength.Value : DBNull.Value);
                                command.Parameters.AddWithValue("@tokenPrice", model.TokenPrice.HasValue ? (object)Convert.ToDecimal(model.TokenPrice.Value) : DBNull.Value);
                                command.Parameters.AddWithValue("@isEnabled", model.Enabled);

                                command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"保存模型到MySQL时出错: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public List<ModelInfo> GetModels(string provider = null)
        {
            List<ModelInfo> models = new List<ModelInfo>();

            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                
                string sql = "SELECT * FROM Models";
                if (!string.IsNullOrEmpty(provider))
                {
                    sql += " WHERE Provider = @provider";
                }

                using (var command = new MySqlCommand(sql, connection))
                {
                    if (!string.IsNullOrEmpty(provider))
                    {
                        command.Parameters.AddWithValue("@provider", provider);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string id = reader["Id"].ToString();
                            string name = reader["Name"].ToString();
                            string modelProvider = reader["Provider"].ToString();
                            ModelCategory category = (ModelCategory)Enum.Parse(typeof(ModelCategory), reader["Category"].ToString());
                            int? contextLength = reader["ContextLength"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ContextLength"]) : null;
                            double? tokenPrice = reader["TokenPrice"] != DBNull.Value ? (double?)Convert.ToDouble(reader["TokenPrice"]) : null;
                            bool isEnabled = Convert.ToBoolean(reader["IsEnabled"]);

                            models.Add(new ModelInfo(id, name, modelProvider, category, contextLength, tokenPrice)
                            {
                                Enabled = isEnabled
                            });
                        }
                    }
                }
            }

            return models;
        }

        public void ClearModels(string provider = null)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.MySqlConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string sql = "DELETE FROM Models";
                        if (!string.IsNullOrEmpty(provider))
                        {
                            sql += " WHERE Provider = @provider";
                        }

                        using (var command = new MySqlCommand(sql, connection, transaction))
                        {
                            if (!string.IsNullOrEmpty(provider))
                            {
                                command.Parameters.AddWithValue("@provider", provider);
                            }

                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"从MySQL清除模型时出错: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        // 保留旧方法名作为别名，确保向后兼容
        public void SaveModelsToMySQL(List<ModelInfo> models, string provider)
        {
            SaveModels(models, provider);
        }

        public List<ModelInfo> GetModelsFromMySQL(string provider = null)
        {
            return GetModels(provider);
        }

        public void ClearModelsFromMySQL(string provider = null)
        {
            ClearModels(provider);
        }
    }
}