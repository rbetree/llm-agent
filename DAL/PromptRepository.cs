using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using llm_agent.Model;
using MySql.Data.MySqlClient;

namespace llm_agent.DAL
{
    /// <summary>
    /// 提示词数据访问类
    /// </summary>
    public class PromptRepository
    {
        /// <summary>
        /// 初始化PromptRepository
        /// </summary>
        public PromptRepository()
        {
            // 数据库初始化由DatabaseManager处理
        }

        /// <summary>
        /// 保存提示词
        /// </summary>
        /// <param name="prompt">要保存的提示词</param>
        public void SavePrompt(Prompt prompt)
        {
            if (prompt == null) return;

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 检查是否为新提示词
                        bool isNewPrompt = false;
                        using (var command = new MySqlCommand("SELECT COUNT(*) FROM `Prompts` WHERE `Id` = @id", connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", prompt.Id);
                            isNewPrompt = Convert.ToInt32(command.ExecuteScalar()) == 0;
                        }

                        string sql;
                        if (isNewPrompt)
                        {
                            sql = @"
                                INSERT INTO `Prompts` (`Id`, `Title`, `Content`, `Category`, `CreatedAt`, `UpdatedAt`, `UsageCount`)
                                VALUES (@id, @title, @content, @category, @createdAt, @updatedAt, @usageCount)";
                        }
                        else
                        {
                            sql = @"
                                UPDATE `Prompts` SET
                                `Title` = @title,
                                `Content` = @content,
                                `Category` = @category,
                                `UpdatedAt` = @updatedAt,
                                `UsageCount` = @usageCount
                                WHERE `Id` = @id";
                        }

                        using (var command = new MySqlCommand(sql, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@id", prompt.Id);
                            command.Parameters.AddWithValue("@title", prompt.Title);
                            command.Parameters.AddWithValue("@content", prompt.Content);
                            command.Parameters.AddWithValue("@category", prompt.Category);
                            command.Parameters.AddWithValue("@createdAt", prompt.CreatedAt);
                            command.Parameters.AddWithValue("@updatedAt", prompt.UpdatedAt);
                            command.Parameters.AddWithValue("@usageCount", prompt.UsageCount);
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
        /// 获取指定ID的提示词
        /// </summary>
        /// <param name="promptId">提示词ID</param>
        /// <returns>提示词对象，如果不存在则返回null</returns>
        public Prompt GetPrompt(string promptId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM `Prompts` WHERE `Id` = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", promptId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Prompt
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                Category = reader["Category"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                UsageCount = Convert.ToInt32(reader["UsageCount"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有提示词
        /// </summary>
        /// <returns>提示词列表</returns>
        public List<Prompt> GetAllPrompts()
        {
            List<Prompt> prompts = new List<Prompt>();

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM `Prompts` ORDER BY `Category`, `Title`";

                using (var command = new MySqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prompts.Add(new Prompt
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                Category = reader["Category"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                UsageCount = Convert.ToInt32(reader["UsageCount"])
                            });
                        }
                    }
                }
            }

            return prompts;
        }

        /// <summary>
        /// 按分类获取提示词
        /// </summary>
        /// <param name="category">分类名称</param>
        /// <returns>提示词列表</returns>
        public List<Prompt> GetPromptsByCategory(string category)
        {
            List<Prompt> prompts = new List<Prompt>();

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT * FROM `Prompts` WHERE `Category` = @category ORDER BY `Title`";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@category", category);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prompts.Add(new Prompt
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                Category = reader["Category"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                UsageCount = Convert.ToInt32(reader["UsageCount"])
                            });
                        }
                    }
                }
            }

            return prompts;
        }

        /// <summary>
        /// 搜索提示词
        /// </summary>
        /// <param name="searchText">搜索文本</param>
        /// <returns>匹配的提示词列表</returns>
        public List<Prompt> SearchPrompts(string searchText)
        {
            List<Prompt> prompts = new List<Prompt>();

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = @"
                    SELECT * FROM `Prompts`
                    WHERE `Title` LIKE @searchText OR `Content` LIKE @searchText OR `Category` LIKE @searchText
                    ORDER BY `Category`, `Title`";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@searchText", $"%{searchText}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prompts.Add(new Prompt
                            {
                                Id = reader["Id"].ToString(),
                                Title = reader["Title"].ToString(),
                                Content = reader["Content"].ToString(),
                                Category = reader["Category"].ToString(),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                UpdatedAt = Convert.ToDateTime(reader["UpdatedAt"]),
                                UsageCount = Convert.ToInt32(reader["UsageCount"])
                            });
                        }
                    }
                }
            }

            return prompts;
        }

        /// <summary>
        /// 删除提示词
        /// </summary>
        /// <param name="promptId">提示词ID</param>
        public void DeletePrompt(string promptId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "DELETE FROM `Prompts` WHERE `Id` = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", promptId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 增加提示词使用次数
        /// </summary>
        /// <param name="promptId">提示词ID</param>
        public void IncrementUsageCount(string promptId)
        {
            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "UPDATE `Prompts` SET `UsageCount` = `UsageCount` + 1 WHERE `Id` = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", promptId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 获取所有分类
        /// </summary>
        /// <returns>分类列表</returns>
        public List<string> GetAllCategories()
        {
            List<string> categories = new List<string>();

            using (var connection = new MySqlConnection(DatabaseConfig.GetConnectionString()))
            {
                connection.Open();
                string sql = "SELECT DISTINCT `Category` FROM `Prompts` ORDER BY `Category`";

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
    }
}