using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;
using llm_agent.Model;
using llm_agent.Models;
using System.Threading.Tasks;

namespace llm_agent.DAL
{
    /// <summary>
    /// 数据库迁移工具，用于从SQLite迁移数据到MySQL
    /// </summary>
    public class DatabaseMigrationTool
    {
        private readonly string _sqliteConnectionString;
        private readonly string _mysqlConnectionString;
        private readonly string _sqliteDbPath;

        /// <summary>
        /// 初始化数据库迁移工具
        /// </summary>
        public DatabaseMigrationTool()
        {
            _sqliteDbPath = DatabaseConfig.GetSqliteDbPath();
            _sqliteConnectionString = DatabaseConfig.SqliteConnectionString;
            _mysqlConnectionString = DatabaseConfig.MySqlConnectionString;
        }

        /// <summary>
        /// 检查是否需要迁移数据
        /// </summary>
        /// <returns>是否需要迁移</returns>
        public bool NeedsMigration()
        {
            // 如果当前使用MySQL但SQLite文件存在且有数据，则需要迁移
            if (DatabaseConfig.DatabaseType == DatabaseType.MySQL && File.Exists(_sqliteDbPath))
            {
                try
                {
                    using (var connection = new SQLiteConnection(_sqliteConnectionString))
                    {
                        connection.Open();
                        
                        // 检查是否有任何表中有数据
                        string[] tables = { "ChatSessions", "AiWebsites", "Prompts", "Channels" };
                        foreach (var table in tables)
                        {
                            string sql = $"SELECT COUNT(*) FROM {table}";
                            using (var command = new SQLiteCommand(sql, connection))
                            {
                                try
                                {
                                    int count = Convert.ToInt32(command.ExecuteScalar());
                                    if (count > 0)
                                    {
                                        return true;
                                    }
                                }
                                catch
                                {
                                    // 表可能不存在，继续检查下一个表
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // SQLite文件可能损坏或无法访问
                    return false;
                }
            }
            
            return false;
        }

        /// <summary>
        /// 执行数据迁移
        /// </summary>
        /// <returns>迁移结果</returns>
        public async Task<MigrationResult> MigrateAsync()
        {
            var result = new MigrationResult();

            try
            {
                // 确保MySQL数据库和表已初始化
                var databaseManager = new DatabaseManager();

                // 迁移聊天会话和消息
                result.ChatSessionsMigrated = await MigrateChatSessionsAsync();

                // 迁移提示词
                result.PromptsMigrated = await MigratePromptsAsync();

                // 迁移网站和凭据
                result.WebsitesMigrated = await MigrateWebsitesAsync();

                // 迁移渠道配置
                result.ChannelsMigrated = await MigrateChannelsAsync();

                // 迁移成功后，可以重命名SQLite文件作为备份
                if (File.Exists(_sqliteDbPath))
                {
                    string backupPath = _sqliteDbPath + ".backup";
                    if (File.Exists(backupPath))
                    {
                        File.Delete(backupPath);
                    }
                    File.Move(_sqliteDbPath, backupPath);
                    result.BackupCreated = true;
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 迁移聊天会话和消息
        /// </summary>
        private async Task<int> MigrateChatSessionsAsync()
        {
            int migratedCount = 0;

            using (var sqliteConnection = new SQLiteConnection(_sqliteConnectionString))
            using (var mysqlConnection = new MySqlConnection(_mysqlConnectionString))
            {
                await sqliteConnection.OpenAsync();
                await mysqlConnection.OpenAsync();

                // 获取所有聊天会话
                string sessionSql = "SELECT * FROM ChatSessions";
                using (var command = new SQLiteCommand(sessionSql, sqliteConnection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string sessionId = reader["Id"].ToString();
                        string title = reader["Title"].ToString();
                        DateTime createdAt = DateTime.Parse(reader["CreatedAt"].ToString());
                        DateTime updatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());
                        int orderIndex = Convert.ToInt32(reader["OrderIndex"]);

                        // 插入到MySQL
                        string insertSessionSql = @"
                            INSERT INTO ChatSessions (Id, Title, CreatedAt, UpdatedAt, OrderIndex)
                            VALUES (@id, @title, @createdAt, @updatedAt, @orderIndex)
                            ON DUPLICATE KEY UPDATE 
                                Title = @title, 
                                UpdatedAt = @updatedAt, 
                                OrderIndex = @orderIndex";

                        using (var mysqlCommand = new MySqlCommand(insertSessionSql, mysqlConnection))
                        {
                            mysqlCommand.Parameters.AddWithValue("@id", sessionId);
                            mysqlCommand.Parameters.AddWithValue("@title", title);
                            mysqlCommand.Parameters.AddWithValue("@createdAt", createdAt);
                            mysqlCommand.Parameters.AddWithValue("@updatedAt", updatedAt);
                            mysqlCommand.Parameters.AddWithValue("@orderIndex", orderIndex);

                            await mysqlCommand.ExecuteNonQueryAsync();
                        }

                        // 迁移该会话的消息
                        await MigrateChatMessagesAsync(sessionId, sqliteConnection, mysqlConnection);

                        migratedCount++;
                    }
                }
            }

            return migratedCount;
        }

        /// <summary>
        /// 迁移聊天消息
        /// </summary>
        private async Task MigrateChatMessagesAsync(string sessionId, SQLiteConnection sqliteConnection, MySqlConnection mysqlConnection)
        {
            // 获取会话的所有消息
            string messageSql = "SELECT * FROM ChatMessages WHERE SessionId = @sessionId ORDER BY Id";
            using (var command = new SQLiteCommand(messageSql, sqliteConnection))
            {
                command.Parameters.AddWithValue("@sessionId", sessionId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string role = reader["Role"].ToString();
                        string content = reader["Content"].ToString();
                        DateTime timestamp = DateTime.Parse(reader["Timestamp"].ToString());

                        // 插入到MySQL
                        string insertMessageSql = @"
                            INSERT INTO ChatMessages (SessionId, Role, Content, Timestamp)
                            VALUES (@sessionId, @role, @content, @timestamp)";

                        using (var mysqlCommand = new MySqlCommand(insertMessageSql, mysqlConnection))
                        {
                            mysqlCommand.Parameters.AddWithValue("@sessionId", sessionId);
                            mysqlCommand.Parameters.AddWithValue("@role", role);
                            mysqlCommand.Parameters.AddWithValue("@content", content);
                            mysqlCommand.Parameters.AddWithValue("@timestamp", timestamp);

                            await mysqlCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 迁移提示词
        /// </summary>
        private async Task<int> MigratePromptsAsync()
        {
            int migratedCount = 0;

            using (var sqliteConnection = new SQLiteConnection(_sqliteConnectionString))
            using (var mysqlConnection = new MySqlConnection(_mysqlConnectionString))
            {
                await sqliteConnection.OpenAsync();
                await mysqlConnection.OpenAsync();

                // 获取所有提示词
                string promptSql = "SELECT * FROM Prompts";
                using (var command = new SQLiteCommand(promptSql, sqliteConnection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string id = reader["Id"].ToString();
                        string title = reader["Title"].ToString();
                        string content = reader["Content"].ToString();
                        string category = reader["Category"].ToString();
                        DateTime createdAt = DateTime.Parse(reader["CreatedAt"].ToString());
                        DateTime updatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());
                        int usageCount = Convert.ToInt32(reader["UsageCount"]);

                        // 插入到MySQL
                        string insertPromptSql = @"
                            INSERT INTO Prompts (Id, Title, Content, Category, CreatedAt, UpdatedAt, UsageCount)
                            VALUES (@id, @title, @content, @category, @createdAt, @updatedAt, @usageCount)
                            ON DUPLICATE KEY UPDATE 
                                Title = @title, 
                                Content = @content, 
                                Category = @category, 
                                UpdatedAt = @updatedAt, 
                                UsageCount = @usageCount";

                        using (var mysqlCommand = new MySqlCommand(insertPromptSql, mysqlConnection))
                        {
                            mysqlCommand.Parameters.AddWithValue("@id", id);
                            mysqlCommand.Parameters.AddWithValue("@title", title);
                            mysqlCommand.Parameters.AddWithValue("@content", content);
                            mysqlCommand.Parameters.AddWithValue("@category", category);
                            mysqlCommand.Parameters.AddWithValue("@createdAt", createdAt);
                            mysqlCommand.Parameters.AddWithValue("@updatedAt", updatedAt);
                            mysqlCommand.Parameters.AddWithValue("@usageCount", usageCount);

                            await mysqlCommand.ExecuteNonQueryAsync();
                        }

                        migratedCount++;
                    }
                }
            }

            return migratedCount;
        }

        /// <summary>
        /// 迁移网站和凭据
        /// </summary>
        private async Task<int> MigrateWebsitesAsync()
        {
            int migratedCount = 0;

            using (var sqliteConnection = new SQLiteConnection(_sqliteConnectionString))
            using (var mysqlConnection = new MySqlConnection(_mysqlConnectionString))
            {
                await sqliteConnection.OpenAsync();
                await mysqlConnection.OpenAsync();

                // 获取所有网站
                string websiteSql = "SELECT * FROM AiWebsites";
                using (var command = new SQLiteCommand(websiteSql, sqliteConnection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string id = reader["Id"].ToString();
                        string name = reader["Name"].ToString();
                        string description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString();
                        string url = reader["Url"].ToString();
                        string iconUrl = reader["IconUrl"] == DBNull.Value ? null : reader["IconUrl"].ToString();
                        string category = reader["Category"] == DBNull.Value ? null : reader["Category"].ToString();
                        int sortOrder = Convert.ToInt32(reader["SortOrder"]);
                        bool isActive = Convert.ToBoolean(reader["IsActive"]);
                        DateTime createdAt = DateTime.Parse(reader["CreatedAt"].ToString());
                        DateTime updatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());
                        DateTime? lastVisitedAt = reader["LastVisitedAt"] == DBNull.Value ? null : (DateTime?)DateTime.Parse(reader["LastVisitedAt"].ToString());

                        // 插入到MySQL
                        string insertWebsiteSql = @"
                            INSERT INTO AiWebsites (Id, Name, Description, Url, IconUrl, Category, SortOrder, IsActive, CreatedAt, UpdatedAt, LastVisitedAt)
                            VALUES (@id, @name, @description, @url, @iconUrl, @category, @sortOrder, @isActive, @createdAt, @updatedAt, @lastVisitedAt)
                            ON DUPLICATE KEY UPDATE 
                                Name = @name, 
                                Description = @description, 
                                Url = @url, 
                                IconUrl = @iconUrl, 
                                Category = @category, 
                                SortOrder = @sortOrder, 
                                IsActive = @isActive, 
                                UpdatedAt = @updatedAt, 
                                LastVisitedAt = @lastVisitedAt";

                        using (var mysqlCommand = new MySqlCommand(insertWebsiteSql, mysqlConnection))
                        {
                            mysqlCommand.Parameters.AddWithValue("@id", id);
                            mysqlCommand.Parameters.AddWithValue("@name", name);
                            mysqlCommand.Parameters.AddWithValue("@description", description ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@url", url);
                            mysqlCommand.Parameters.AddWithValue("@iconUrl", iconUrl ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@category", category ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@sortOrder", sortOrder);
                            mysqlCommand.Parameters.AddWithValue("@isActive", isActive ? 1 : 0);
                            mysqlCommand.Parameters.AddWithValue("@createdAt", createdAt);
                            mysqlCommand.Parameters.AddWithValue("@updatedAt", updatedAt);
                            mysqlCommand.Parameters.AddWithValue("@lastVisitedAt", lastVisitedAt ?? (object)DBNull.Value);

                            await mysqlCommand.ExecuteNonQueryAsync();
                        }

                        // 迁移网站凭据
                        await MigrateWebsiteCredentialsAsync(id, sqliteConnection, mysqlConnection);

                        migratedCount++;
                    }
                }
            }

            return migratedCount;
        }

        /// <summary>
        /// 迁移网站凭据
        /// </summary>
        private async Task MigrateWebsiteCredentialsAsync(string websiteId, SQLiteConnection sqliteConnection, MySqlConnection mysqlConnection)
        {
            // 获取网站的凭据
            string credentialSql = "SELECT * FROM WebsiteCredentials WHERE WebsiteId = @websiteId";
            using (var command = new SQLiteCommand(credentialSql, sqliteConnection))
            {
                command.Parameters.AddWithValue("@websiteId", websiteId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        string id = reader["Id"].ToString();
                        string username = reader["Username"] == DBNull.Value ? null : reader["Username"].ToString();
                        string password = reader["Password"] == DBNull.Value ? null : reader["Password"].ToString();
                        string notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"].ToString();
                        DateTime createdAt = DateTime.Parse(reader["CreatedAt"].ToString());
                        DateTime updatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());

                        // 插入到MySQL
                        string insertCredentialSql = @"
                            INSERT INTO WebsiteCredentials (Id, WebsiteId, Username, Password, Notes, CreatedAt, UpdatedAt)
                            VALUES (@id, @websiteId, @username, @password, @notes, @createdAt, @updatedAt)
                            ON DUPLICATE KEY UPDATE 
                                Username = @username, 
                                Password = @password, 
                                Notes = @notes, 
                                UpdatedAt = @updatedAt";

                        using (var mysqlCommand = new MySqlCommand(insertCredentialSql, mysqlConnection))
                        {
                            mysqlCommand.Parameters.AddWithValue("@id", id);
                            mysqlCommand.Parameters.AddWithValue("@websiteId", websiteId);
                            mysqlCommand.Parameters.AddWithValue("@username", username ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@password", password ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@notes", notes ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@createdAt", createdAt);
                            mysqlCommand.Parameters.AddWithValue("@updatedAt", updatedAt);

                            await mysqlCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 迁移渠道配置
        /// </summary>
        private async Task<int> MigrateChannelsAsync()
        {
            int migratedCount = 0;

            using (var sqliteConnection = new SQLiteConnection(_sqliteConnectionString))
            using (var mysqlConnection = new MySqlConnection(_mysqlConnectionString))
            {
                await sqliteConnection.OpenAsync();
                await mysqlConnection.OpenAsync();

                // 检查Channels表是否存在
                bool channelsTableExists = false;
                string checkTableSql = "SELECT name FROM sqlite_master WHERE type='table' AND name='Channels'";
                using (var command = new SQLiteCommand(checkTableSql, sqliteConnection))
                {
                    var result = await command.ExecuteScalarAsync();
                    channelsTableExists = result != null;
                }

                if (!channelsTableExists)
                {
                    return 0; // 表不存在，不需要迁移
                }

                // 获取所有渠道
                string channelSql = "SELECT * FROM Channels";
                using (var command = new SQLiteCommand(channelSql, sqliteConnection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string id = reader["Id"].ToString();
                        string name = reader["Name"].ToString();
                        string providerType = reader["ProviderType"].ToString();
                        string apiKey = reader["ApiKey"] == DBNull.Value ? null : reader["ApiKey"].ToString();
                        string apiHost = reader["ApiHost"] == DBNull.Value ? null : reader["ApiHost"].ToString();
                        bool isEnabled = Convert.ToBoolean(reader["IsEnabled"]);
                        bool useStreamResponse = Convert.ToBoolean(reader["UseStreamResponse"]);
                        DateTime createdAt = DateTime.Parse(reader["CreatedAt"].ToString());
                        DateTime updatedAt = DateTime.Parse(reader["UpdatedAt"].ToString());

                        // 插入到MySQL
                        string insertChannelSql = @"
                            INSERT INTO Channels (Id, Name, ProviderType, ApiKey, ApiHost, IsEnabled, UseStreamResponse, CreatedAt, UpdatedAt)
                            VALUES (@id, @name, @providerType, @apiKey, @apiHost, @isEnabled, @useStreamResponse, @createdAt, @updatedAt)
                            ON DUPLICATE KEY UPDATE 
                                Name = @name, 
                                ProviderType = @providerType, 
                                ApiKey = @apiKey, 
                                ApiHost = @apiHost, 
                                IsEnabled = @isEnabled, 
                                UseStreamResponse = @useStreamResponse, 
                                UpdatedAt = @updatedAt";

                        using (var mysqlCommand = new MySqlCommand(insertChannelSql, mysqlConnection))
                        {
                            mysqlCommand.Parameters.AddWithValue("@id", id);
                            mysqlCommand.Parameters.AddWithValue("@name", name);
                            mysqlCommand.Parameters.AddWithValue("@providerType", providerType);
                            mysqlCommand.Parameters.AddWithValue("@apiKey", apiKey ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@apiHost", apiHost ?? (object)DBNull.Value);
                            mysqlCommand.Parameters.AddWithValue("@isEnabled", isEnabled ? 1 : 0);
                            mysqlCommand.Parameters.AddWithValue("@useStreamResponse", useStreamResponse ? 1 : 0);
                            mysqlCommand.Parameters.AddWithValue("@createdAt", createdAt);
                            mysqlCommand.Parameters.AddWithValue("@updatedAt", updatedAt);

                            await mysqlCommand.ExecuteNonQueryAsync();
                        }

                        // 迁移渠道模型
                        await MigrateChannelModelsAsync(id, sqliteConnection, mysqlConnection);

                        migratedCount++;
                    }
                }
            }

            return migratedCount;
        }

        /// <summary>
        /// 迁移渠道模型
        /// </summary>
        private async Task MigrateChannelModelsAsync(string channelId, SQLiteConnection sqliteConnection, MySqlConnection mysqlConnection)
        {
            // 检查ChannelModels表是否存在
            bool channelModelsTableExists = false;
            string checkTableSql = "SELECT name FROM sqlite_master WHERE type='table' AND name='ChannelModels'";
            using (var command = new SQLiteCommand(checkTableSql, sqliteConnection))
            {
                var result = await command.ExecuteScalarAsync();
                channelModelsTableExists = result != null;
            }

            if (!channelModelsTableExists)
            {
                return; // 表不存在，不需要迁移
            }

            // 获取渠道的模型
            string modelSql = "SELECT * FROM ChannelModels WHERE ChannelId = @channelId";
            using (var command = new SQLiteCommand(modelSql, sqliteConnection))
            {
                command.Parameters.AddWithValue("@channelId", channelId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        string modelName = reader["ModelName"].ToString();

                        // 插入到MySQL
                        string insertModelSql = @"
                            INSERT INTO ChannelModels (ChannelId, ModelName)
                            VALUES (@channelId, @modelName)";

                        using (var mysqlCommand = new MySqlCommand(insertModelSql, mysqlConnection))
                        {
                            mysqlCommand.Parameters.AddWithValue("@channelId", channelId);
                            mysqlCommand.Parameters.AddWithValue("@modelName", modelName);

                            await mysqlCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 数据迁移结果
    /// </summary>
    public class MigrationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int ChatSessionsMigrated { get; set; }
        public int PromptsMigrated { get; set; }
        public int WebsitesMigrated { get; set; }
        public int ChannelsMigrated { get; set; }
        public bool BackupCreated { get; set; }

        public override string ToString()
        {
            if (Success)
            {
                return $"迁移成功！迁移了 {ChatSessionsMigrated} 个聊天会话、{PromptsMigrated} 个提示词、{WebsitesMigrated} 个网站和 {ChannelsMigrated} 个渠道。" +
                       (BackupCreated ? " 已创建SQLite数据库备份。" : "");
            }
            else
            {
                return $"迁移失败：{ErrorMessage}";
            }
        }
    }
} 