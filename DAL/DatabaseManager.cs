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