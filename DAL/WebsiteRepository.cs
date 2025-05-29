using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using llm_agent.Model;
using llm_agent.Common.Utils;

namespace llm_agent.DAL
{
    /// <summary>
    /// 网站数据访问类
    /// </summary>
    public class WebsiteRepository
    {
        private static readonly string DbName = "llm_agent.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        /// <summary>
        /// 初始化WebsiteRepository
        /// </summary>
        public WebsiteRepository()
        {
            // 确保数据库和表已初始化
            var databaseManager = new DatabaseManager();
        }

        #region AI网站管理

        /// <summary>
        /// 保存网站信息
        /// </summary>
        /// <param name="website">要保存的网站</param>
        public void SaveWebsite(AiWebsite website)
        {
            if (website == null) return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // 检查是否已存在
                var existingWebsite = GetWebsiteById(website.Id);

                if (existingWebsite != null)
                {
                    // 更新现有网站
                    UpdateWebsite(website, connection);
                }
                else
                {
                    // 插入新网站
                    InsertWebsite(website, connection);
                }
            }
        }

        /// <summary>
        /// 插入新网站
        /// </summary>
        private void InsertWebsite(AiWebsite website, SQLiteConnection connection)
        {
            string sql = @"
                INSERT INTO AiWebsites (Id, Name, Description, Url, IconUrl, Category, SortOrder, IsActive, CreatedAt, UpdatedAt, LastVisitedAt)
                VALUES (@id, @name, @description, @url, @iconUrl, @category, @sortOrder, @isActive, @createdAt, @updatedAt, @lastVisitedAt)";

            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", website.Id);
                command.Parameters.AddWithValue("@name", website.Name);
                command.Parameters.AddWithValue("@description", website.Description ?? string.Empty);
                command.Parameters.AddWithValue("@url", website.Url);
                command.Parameters.AddWithValue("@iconUrl", website.IconUrl ?? string.Empty);
                command.Parameters.AddWithValue("@category", website.Category ?? string.Empty);
                command.Parameters.AddWithValue("@sortOrder", website.SortOrder);
                command.Parameters.AddWithValue("@isActive", website.IsActive ? 1 : 0);
                command.Parameters.AddWithValue("@createdAt", website.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@updatedAt", website.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@lastVisitedAt", website.LastVisitedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新现有网站
        /// </summary>
        private void UpdateWebsite(AiWebsite website, SQLiteConnection connection)
        {
            website.UpdatedAt = DateTime.Now;

            string sql = @"
                UPDATE AiWebsites
                SET Name = @name, Description = @description, Url = @url, IconUrl = @iconUrl,
                    Category = @category, SortOrder = @sortOrder, IsActive = @isActive,
                    UpdatedAt = @updatedAt, LastVisitedAt = @lastVisitedAt
                WHERE Id = @id";

            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", website.Id);
                command.Parameters.AddWithValue("@name", website.Name);
                command.Parameters.AddWithValue("@description", website.Description ?? string.Empty);
                command.Parameters.AddWithValue("@url", website.Url);
                command.Parameters.AddWithValue("@iconUrl", website.IconUrl ?? string.Empty);
                command.Parameters.AddWithValue("@category", website.Category ?? string.Empty);
                command.Parameters.AddWithValue("@sortOrder", website.SortOrder);
                command.Parameters.AddWithValue("@isActive", website.IsActive ? 1 : 0);
                command.Parameters.AddWithValue("@updatedAt", website.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@lastVisitedAt", website.LastVisitedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据ID获取网站
        /// </summary>
        /// <param name="id">网站ID</param>
        /// <returns>网站信息</returns>
        public AiWebsite GetWebsiteById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM AiWebsites WHERE Id = @id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapWebsiteFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有网站
        /// </summary>
        /// <returns>网站列表</returns>
        public List<AiWebsite> GetAllWebsites()
        {
            var websites = new List<AiWebsite>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM AiWebsites WHERE IsActive = 1 ORDER BY SortOrder, Name";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            websites.Add(MapWebsiteFromReader(reader));
                        }
                    }
                }
            }

            return websites;
        }

        /// <summary>
        /// 根据分类获取网站
        /// </summary>
        /// <param name="category">分类名称</param>
        /// <returns>网站列表</returns>
        public List<AiWebsite> GetWebsitesByCategory(string category)
        {
            var websites = new List<AiWebsite>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM AiWebsites WHERE Category = @category AND IsActive = 1 ORDER BY SortOrder, Name";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@category", category ?? string.Empty);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            websites.Add(MapWebsiteFromReader(reader));
                        }
                    }
                }
            }

            return websites;
        }

        /// <summary>
        /// 搜索网站
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>匹配的网站列表</returns>
        public List<AiWebsite> SearchWebsites(string searchText)
        {
            var websites = new List<AiWebsite>();

            if (string.IsNullOrWhiteSpace(searchText))
                return GetAllWebsites();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT * FROM AiWebsites
                    WHERE IsActive = 1 AND (
                        Name LIKE @searchText OR
                        Description LIKE @searchText OR
                        Url LIKE @searchText OR
                        Category LIKE @searchText
                    )
                    ORDER BY SortOrder, Name";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@searchText", $"%{searchText}%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            websites.Add(MapWebsiteFromReader(reader));
                        }
                    }
                }
            }

            return websites;
        }

        /// <summary>
        /// 删除网站
        /// </summary>
        /// <param name="id">网站ID</param>
        public void DeleteWebsite(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 先删除相关的凭据
                        DeleteWebsiteCredentials(id, connection, transaction);

                        // 再删除网站
                        string sql = "DELETE FROM AiWebsites WHERE Id = @id";
                        using (var command = new SQLiteCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
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

        /// <summary>
        /// 更新网站访问时间
        /// </summary>
        /// <param name="id">网站ID</param>
        public void UpdateLastVisitedTime(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "UPDATE AiWebsites SET LastVisitedAt = @lastVisitedAt, UpdatedAt = @updatedAt WHERE Id = @id";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    var now = DateTime.Now;
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@lastVisitedAt", now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("@updatedAt", now.ToString("yyyy-MM-dd HH:mm:ss"));

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 从数据读取器映射网站对象
        /// </summary>
        private AiWebsite MapWebsiteFromReader(SQLiteDataReader reader)
        {
            var website = new AiWebsite
            {
                Id = reader["Id"].ToString(),
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                Url = reader["Url"].ToString(),
                IconUrl = reader["IconUrl"].ToString(),
                Category = reader["Category"].ToString(),
                SortOrder = Convert.ToInt32(reader["SortOrder"]),
                IsActive = Convert.ToBoolean(Convert.ToInt32(reader["IsActive"])),
                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString())
            };

            if (!reader.IsDBNull(reader.GetOrdinal("LastVisitedAt")))
            {
                website.LastVisitedAt = DateTime.Parse(reader["LastVisitedAt"].ToString());
            }

            return website;
        }

        #endregion

        #region 网站凭据管理

        /// <summary>
        /// 保存网站凭据
        /// </summary>
        /// <param name="credential">要保存的凭据</param>
        public void SaveWebsiteCredential(WebsiteCredential credential)
        {
            if (credential == null) return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                // 检查是否已存在
                var existingCredential = GetWebsiteCredentialByWebsiteId(credential.WebsiteId);

                if (existingCredential != null)
                {
                    // 更新现有凭据
                    credential.Id = existingCredential.Id; // 保持原有ID
                    UpdateWebsiteCredential(credential, connection);
                }
                else
                {
                    // 插入新凭据
                    InsertWebsiteCredential(credential, connection);
                }
            }
        }

        /// <summary>
        /// 插入新凭据
        /// </summary>
        private void InsertWebsiteCredential(WebsiteCredential credential, SQLiteConnection connection)
        {
            string sql = @"
                INSERT INTO WebsiteCredentials (Id, WebsiteId, Username, Password, Notes, CreatedAt, UpdatedAt)
                VALUES (@id, @websiteId, @username, @password, @notes, @createdAt, @updatedAt)";

            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", credential.Id);
                command.Parameters.AddWithValue("@websiteId", credential.WebsiteId);
                command.Parameters.AddWithValue("@username", EncryptionHelper.Encrypt(credential.Username));
                command.Parameters.AddWithValue("@password", EncryptionHelper.Encrypt(credential.Password));
                command.Parameters.AddWithValue("@notes", EncryptionHelper.Encrypt(credential.Notes));
                command.Parameters.AddWithValue("@createdAt", credential.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@updatedAt", credential.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新现有凭据
        /// </summary>
        private void UpdateWebsiteCredential(WebsiteCredential credential, SQLiteConnection connection)
        {
            credential.UpdatedAt = DateTime.Now;

            string sql = @"
                UPDATE WebsiteCredentials
                SET Username = @username, Password = @password, Notes = @notes, UpdatedAt = @updatedAt
                WHERE Id = @id";

            using (var command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", credential.Id);
                command.Parameters.AddWithValue("@username", EncryptionHelper.Encrypt(credential.Username));
                command.Parameters.AddWithValue("@password", EncryptionHelper.Encrypt(credential.Password));
                command.Parameters.AddWithValue("@notes", EncryptionHelper.Encrypt(credential.Notes));
                command.Parameters.AddWithValue("@updatedAt", credential.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据网站ID获取凭据
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>网站凭据</returns>
        public WebsiteCredential GetWebsiteCredentialByWebsiteId(string websiteId)
        {
            if (string.IsNullOrEmpty(websiteId)) return null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM WebsiteCredentials WHERE WebsiteId = @websiteId";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@websiteId", websiteId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapCredentialFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 删除网站凭据
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        private void DeleteWebsiteCredentials(string websiteId, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            string sql = "DELETE FROM WebsiteCredentials WHERE WebsiteId = @websiteId";
            using (var command = new SQLiteCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@websiteId", websiteId);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 从数据读取器映射凭据对象
        /// </summary>
        private WebsiteCredential MapCredentialFromReader(SQLiteDataReader reader)
        {
            var credential = new WebsiteCredential
            {
                Id = reader["Id"].ToString(),
                WebsiteId = reader["WebsiteId"].ToString(),
                Username = EncryptionHelper.Decrypt(reader["Username"].ToString()),
                Password = EncryptionHelper.Decrypt(reader["Password"].ToString()),
                Notes = EncryptionHelper.Decrypt(reader["Notes"].ToString()),
                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString())
            };

            return credential;
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns>分类列表</returns>
        public List<string> GetAllCategories()
        {
            var categories = new List<string>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT DISTINCT Category FROM AiWebsites WHERE IsActive = 1 AND Category IS NOT NULL AND Category != '' ORDER BY Category";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var category = reader["Category"].ToString();
                            if (!string.IsNullOrWhiteSpace(category))
                            {
                                categories.Add(category);
                            }
                        }
                    }
                }
            }

            return categories;
        }

        /// <summary>
        /// 获取网站数量统计
        /// </summary>
        /// <returns>网站数量</returns>
        public int GetWebsiteCount()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM AiWebsites WHERE IsActive = 1";
                using (var command = new SQLiteCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// 获取带凭据的网站完整信息
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>包含凭据的网站信息</returns>
        public AiWebsite GetWebsiteWithCredential(string websiteId)
        {
            var website = GetWebsiteById(websiteId);
            if (website != null)
            {
                website.Credential = GetWebsiteCredentialByWebsiteId(websiteId);
            }
            return website;
        }

        /// <summary>
        /// 获取最近访问的网站
        /// </summary>
        /// <param name="count">返回数量</param>
        /// <returns>最近访问的网站列表</returns>
        public List<AiWebsite> GetRecentlyVisitedWebsites(int count = 10)
        {
            var websites = new List<AiWebsite>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = @"
                    SELECT * FROM AiWebsites
                    WHERE IsActive = 1 AND LastVisitedAt IS NOT NULL
                    ORDER BY LastVisitedAt DESC
                    LIMIT @count";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@count", count);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            websites.Add(MapWebsiteFromReader(reader));
                        }
                    }
                }
            }

            return websites;
        }

        /// <summary>
        /// 初始化默认网站数据
        /// </summary>
        public void InitializeDefaultWebsites()
        {
            var defaultWebsites = new List<AiWebsite>
            {
                new AiWebsite("ChatGPT", "https://chat.openai.com", "OpenAI的ChatGPT官方网站", "对话AI"),
                new AiWebsite("Claude", "https://claude.ai", "Anthropic的Claude AI助手", "对话AI"),
                new AiWebsite("Gemini", "https://gemini.google.com", "Google的Gemini AI助手", "对话AI"),
                new AiWebsite("文心一言", "https://yiyan.baidu.com", "百度的文心一言AI助手", "对话AI"),
                new AiWebsite("通义千问", "https://tongyi.aliyun.com", "阿里巴巴的通义千问AI助手", "对话AI"),
                new AiWebsite("智谱清言", "https://chatglm.cn", "智谱AI的ChatGLM助手", "对话AI"),
                new AiWebsite("Midjourney", "https://www.midjourney.com", "AI图像生成工具", "图像生成"),
                new AiWebsite("DALL-E", "https://labs.openai.com", "OpenAI的图像生成工具", "图像生成"),
                new AiWebsite("Stable Diffusion", "https://stablediffusionweb.com", "开源AI图像生成工具", "图像生成"),
                new AiWebsite("GitHub Copilot", "https://github.com/features/copilot", "AI代码助手", "编程工具"),
                new AiWebsite("Cursor", "https://cursor.sh", "AI代码编辑器", "编程工具"),
                new AiWebsite("Perplexity", "https://www.perplexity.ai", "AI搜索引擎", "搜索工具"),
                new AiWebsite("You.com", "https://you.com", "AI搜索引擎", "搜索工具"),
                new AiWebsite("Hugging Face", "https://huggingface.co", "AI模型和数据集平台", "AI平台"),
                new AiWebsite("Replicate", "https://replicate.com", "AI模型运行平台", "AI平台")
            };

            // 设置排序顺序
            for (int i = 0; i < defaultWebsites.Count; i++)
            {
                defaultWebsites[i].SortOrder = i + 1;
            }

            // 保存默认网站
            foreach (var website in defaultWebsites)
            {
                try
                {
                    SaveWebsite(website);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"初始化默认网站 {website.Name} 失败: {ex.Message}");
                }
            }
        }

        #endregion
    }
}
