using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using llm_agent.Models;
using llm_agent.Model;

namespace llm_agent.DAL
{
    public class DatabaseManager
    {
        private static readonly string DbName = "llm_agent.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        public DatabaseManager()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // 创建用户表
                string createUsersTableSql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id TEXT PRIMARY KEY,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        Salt TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        LastLoginAt TEXT,
                        IsAdmin INTEGER NOT NULL DEFAULT 0
                    );";

                using (var command = new SQLiteCommand(createUsersTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                // 检查Users表中是否存在IsAdmin字段
                bool isAdminExists = false;
                try
                {
                    using (var command = new SQLiteCommand("PRAGMA table_info(Users);", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["name"].ToString();
                                if (columnName == "IsAdmin")
                                {
                                    isAdminExists = true;
                                    break;
                                }
                            }
                        }
                    }

                    // 如果Users表存在但IsAdmin字段不存在，则添加该字段
                    if (!isAdminExists)
                    {
                        using (var command = new SQLiteCommand("ALTER TABLE Users ADD COLUMN IsAdmin INTEGER NOT NULL DEFAULT 0;", connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.Error.WriteLine($"检查Users表结构时出错: {ex.Message}");
                }

                // 创建模型表
                string createModelsTableSql = @"
                    CREATE TABLE IF NOT EXISTS Models (
                        Id TEXT PRIMARY KEY,
                        Name TEXT NOT NULL,
                        ProviderType TEXT NOT NULL,
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

                // 检查ChatSessions表中是否存在UserId字段
                bool userIdExists = false;
                try
                {
                    using (var command = new SQLiteCommand("PRAGMA table_info(ChatSessions);", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string columnName = reader["name"].ToString();
                                if (columnName == "UserId")
                                {
                                    userIdExists = true;
                                    break;
                                }
                            }
                        }
                    }

                    // 如果ChatSessions表存在但UserId字段不存在，则添加该字段
                    if (!userIdExists)
                    {
                        using (var command = new SQLiteCommand("ALTER TABLE ChatSessions ADD COLUMN UserId TEXT DEFAULT NULL REFERENCES Users(Id);", connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (SQLiteException)
                {
                    // 如果ChatSessions表不存在，则忽略错误，因为ChatRepository会创建该表
                }

                // 创建已登录用户表
                string createLoggedInUsersTableSql = @"
                    CREATE TABLE IF NOT EXISTS LoggedInUsers (
                        UserId TEXT PRIMARY KEY,
                        LastLoginAt TEXT NOT NULL,
                        FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                    );";

                using (var command = new SQLiteCommand(createLoggedInUsersTableSql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveModels(List<ModelInfo> models, string providerType)
        {
            if (models == null || models.Count == 0)
                return;

            using (var connection = new SQLiteConnection(ConnectionString))
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
                            INSERT INTO Models (Id, Name, ProviderType, ContextLength, TokenPrice, Enabled)
                            VALUES (@id, @name, @providerType, @contextLength, @tokenPrice, @enabled)";

                        foreach (var model in models)
                        {
                            using (var command = new SQLiteCommand(insertSql, connection, transaction))
                            {
                                command.Parameters.AddWithValue("@id", model.Id);
                                command.Parameters.AddWithValue("@name", model.Name);
                                command.Parameters.AddWithValue("@providerType", model.ProviderType.ToLower());
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
            List<ModelInfo> models = new List<ModelInfo>();

            using (var connection = new SQLiteConnection(ConnectionString))
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
                                reader["ProviderType"].ToString()
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