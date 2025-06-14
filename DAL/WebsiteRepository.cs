using System;
using System.Collections.Generic;
using System.IO;
using llm_agent.Model;
using llm_agent.Common.Utils;
using MySql.Data.MySqlClient;

namespace llm_agent.DAL
{
    /// <summary>
    /// 网站数据访问类
    /// </summary>
    public class WebsiteRepository
    {
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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
        private void InsertWebsite(AiWebsite website, MySqlConnection connection)
        {
            string sql = @"
                INSERT INTO AiWebsites (Id, Name, Description, Url, IconUrl, Category, SortOrder, IsActive, CreatedAt, UpdatedAt, LastVisitedAt)
                VALUES (@id, @name, @description, @url, @iconUrl, @category, @sortOrder, @isActive, @createdAt, @updatedAt, @lastVisitedAt)";

            using (var command = new MySqlCommand(sql, connection))
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
        private void UpdateWebsite(AiWebsite website, MySqlConnection connection)
        {
            website.UpdatedAt = DateTime.Now;

            string sql = @"
                UPDATE AiWebsites
                SET Name = @name, Description = @description, Url = @url, IconUrl = @iconUrl,
                    Category = @category, SortOrder = @sortOrder, IsActive = @isActive,
                    UpdatedAt = @updatedAt, LastVisitedAt = @lastVisitedAt
                WHERE Id = @id";

            using (var command = new MySqlCommand(sql, connection))
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                string sql = "SELECT * FROM AiWebsites WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
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

            try
            {
                using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
                {
                    connection.Open();

                    string sql = "SELECT * FROM AiWebsites WHERE IsActive = 1 ORDER BY SortOrder, Name";
                    using (var command = new MySqlCommand(sql, connection))
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
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"MySQL数据库连接失败: {ex.Message}",
                    "数据库连接错误",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                
                throw;
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                string sql = "SELECT * FROM AiWebsites WHERE Category = @category AND IsActive = 1 ORDER BY SortOrder, Name";
                using (var command = new MySqlCommand(sql, connection))
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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

                using (var command = new MySqlCommand(sql, connection))
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
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
                        using (var command = new MySqlCommand(sql, connection, transaction))
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
        /// 更新网站的最后访问时间
        /// </summary>
        /// <param name="id">网站ID</param>
        public void UpdateLastVisitedTime(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                string sql = "UPDATE AiWebsites SET LastVisitedAt = @lastVisitedAt WHERE Id = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@lastVisitedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 从数据读取器映射网站信息
        /// </summary>
        private AiWebsite MapWebsiteFromReader(System.Data.IDataReader reader)
        {
            return new AiWebsite
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
                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString()),
                LastVisitedAt = reader["LastVisitedAt"] != DBNull.Value ? DateTime.Parse(reader["LastVisitedAt"].ToString()) : (DateTime?)null
            };
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

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                // 检查是否已存在
                var existingCredential = GetWebsiteCredentialByWebsiteId(credential.WebsiteId);

                if (existingCredential != null)
                {
                    // 更新现有凭据
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
        private void InsertWebsiteCredential(WebsiteCredential credential, MySqlConnection connection)
        {
            string sql = @"
                INSERT INTO WebsiteCredentials (Id, WebsiteId, Username, Password, Notes, CreatedAt, UpdatedAt)
                VALUES (@id, @websiteId, @username, @password, @notes, @createdAt, @updatedAt)";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", credential.Id);
                command.Parameters.AddWithValue("@websiteId", credential.WebsiteId);
                command.Parameters.AddWithValue("@username", credential.Username ?? string.Empty);
                command.Parameters.AddWithValue("@password", EncryptionHelper.Encrypt(credential.Password) ?? string.Empty);
                command.Parameters.AddWithValue("@notes", credential.Notes ?? string.Empty);
                command.Parameters.AddWithValue("@createdAt", credential.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@updatedAt", credential.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 更新现有凭据
        /// </summary>
        private void UpdateWebsiteCredential(WebsiteCredential credential, MySqlConnection connection)
        {
            credential.UpdatedAt = DateTime.Now;

            string sql = @"
                UPDATE WebsiteCredentials
                SET Username = @username, Password = @password, Notes = @notes, UpdatedAt = @updatedAt
                WHERE WebsiteId = @websiteId";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@websiteId", credential.WebsiteId);
                command.Parameters.AddWithValue("@username", credential.Username ?? string.Empty);
                command.Parameters.AddWithValue("@password", EncryptionHelper.Encrypt(credential.Password) ?? string.Empty);
                command.Parameters.AddWithValue("@notes", credential.Notes ?? string.Empty);
                command.Parameters.AddWithValue("@updatedAt", credential.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 根据网站ID获取凭据
        /// </summary>
        /// <param name="websiteId">网站ID</param>
        /// <returns>凭据信息</returns>
        public WebsiteCredential GetWebsiteCredentialByWebsiteId(string websiteId)
        {
            if (string.IsNullOrEmpty(websiteId)) return null;

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                string sql = "SELECT * FROM WebsiteCredentials WHERE WebsiteId = @websiteId";
                using (var command = new MySqlCommand(sql, connection))
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
        private void DeleteWebsiteCredentials(string websiteId, MySqlConnection connection, MySqlTransaction transaction)
        {
            string sql = "DELETE FROM WebsiteCredentials WHERE WebsiteId = @websiteId";
            using (var command = new MySqlCommand(sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@websiteId", websiteId);
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 从数据读取器映射凭据信息
        /// </summary>
        private WebsiteCredential MapCredentialFromReader(System.Data.IDataReader reader)
        {
            return new WebsiteCredential
            {
                Id = reader["Id"].ToString(),
                WebsiteId = reader["WebsiteId"].ToString(),
                Username = reader["Username"].ToString(),
                Password = EncryptionHelper.Decrypt(reader["Password"].ToString()),
                Notes = reader["Notes"].ToString(),
                CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString())
            };
        }

        #endregion

        #region 分类管理

        /// <summary>
        /// 获取所有网站分类
        /// </summary>
        /// <returns>分类列表</returns>
        public List<string> GetAllCategories()
        {
            var categories = new List<string>();

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                string sql = "SELECT DISTINCT Category FROM AiWebsites WHERE Category IS NOT NULL AND Category != '' ORDER BY Category";
                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(reader["Category"].ToString());
                        }
                    }
                }
            }

            return categories;
        }

        #endregion

        #region 统计信息

        /// <summary>
        /// 获取网站总数
        /// </summary>
        /// <returns>网站数量</returns>
        public int GetWebsiteCount()
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                string sql = "SELECT COUNT(*) FROM AiWebsites WHERE IsActive = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        #endregion

        #region 组合查询

        /// <summary>
        /// 获取带凭据的网站
        /// </summary>
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
        /// 获取所有带凭据的网站
        /// </summary>
        public List<AiWebsite> GetAllWebsitesWithCredentials()
        {
            var websites = GetAllWebsites();
            foreach (var website in websites)
            {
                website.Credential = GetWebsiteCredentialByWebsiteId(website.Id);
            }
            return websites;
        }

        /// <summary>
        /// 搜索带凭据的网站
        /// </summary>
        public List<AiWebsite> SearchWebsitesWithCredentials(string searchText)
        {
            var websites = SearchWebsites(searchText);
            foreach (var website in websites)
            {
                website.Credential = GetWebsiteCredentialByWebsiteId(website.Id);
            }
            return websites;
        }

        /// <summary>
        /// 获取最近访问的网站
        /// </summary>
        /// <param name="count">返回数量</param>
        /// <returns>网站列表</returns>
        public List<AiWebsite> GetRecentlyVisitedWebsites(int count = 10)
        {
            var websites = new List<AiWebsite>();

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();

                string sql = @"
                    SELECT * FROM AiWebsites 
                    WHERE IsActive = 1 AND LastVisitedAt IS NOT NULL 
                    ORDER BY LastVisitedAt DESC 
                    LIMIT @count";

                using (var command = new MySqlCommand(sql, connection))
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

        #endregion

        #region 初始化数据

        /// <summary>
        /// 初始化默认网站
        /// </summary>
        public void InitializeDefaultWebsites()
        {
            if (GetWebsiteCount() > 0)
                return;

            var defaultWebsites = new List<AiWebsite>
            {
                new AiWebsite
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "ChatGPT",
                    Description = "OpenAI的ChatGPT官方网站",
                    Url = "https://chat.openai.com/",
                    IconUrl = "https://chat.openai.com/favicon.ico",
                    Category = "聊天机器人",
                    SortOrder = 0,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new AiWebsite
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Claude",
                    Description = "Anthropic的Claude AI助手",
                    Url = "https://claude.ai/",
                    IconUrl = "https://claude.ai/favicon.ico",
                    Category = "聊天机器人",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new AiWebsite
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Midjourney",
                    Description = "AI图像生成工具",
                    Url = "https://www.midjourney.com/",
                    IconUrl = "https://www.midjourney.com/favicon.ico",
                    Category = "图像生成",
                    SortOrder = 0,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new AiWebsite
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Stable Diffusion",
                    Description = "开源AI图像生成模型",
                    Url = "https://stablediffusionweb.com/",
                    IconUrl = "https://stablediffusionweb.com/favicon.ico",
                    Category = "图像生成",
                    SortOrder = 1,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            foreach (var website in defaultWebsites)
            {
                SaveWebsite(website);
            }
        }

        #endregion
    }
}
