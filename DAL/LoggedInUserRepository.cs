using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using llm_agent.Model;

namespace llm_agent.DAL
{
    /// <summary>
    /// 已登录用户数据访问层，处理已登录用户相关的数据库操作
    /// </summary>
    public class LoggedInUserRepository
    {
        private static readonly string DbName = "llm_agent.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        /// <summary>
        /// 初始化LoggedInUserRepository
        /// </summary>
        public LoggedInUserRepository()
        {
            // 确保数据库和表已初始化
            var databaseManager = new DatabaseManager();
        }

        /// <summary>
        /// 添加已登录用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否添加成功</returns>
        public bool AddLoggedInUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string insertSql = @"
                        INSERT OR REPLACE INTO LoggedInUsers (UserId, LastLoginAt)
                        VALUES (@userId, @lastLoginAt)";

                    using (var command = new SQLiteCommand(insertSql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@lastLoginAt", DateTime.Now.ToString("o"));
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"添加已登录用户时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 移除已登录用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveLoggedInUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string deleteSql = "DELETE FROM LoggedInUsers WHERE UserId = @userId";

                    using (var command = new SQLiteCommand(deleteSql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"移除已登录用户时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 清除所有已登录用户
        /// </summary>
        /// <returns>是否清除成功</returns>
        public bool ClearLoggedInUsers()
        {
            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string deleteSql = "DELETE FROM LoggedInUsers";

                    using (var command = new SQLiteCommand(deleteSql, connection))
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"清除已登录用户时出错: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取所有已登录用户
        /// </summary>
        /// <returns>已登录用户列表</returns>
        public List<User> GetLoggedInUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT u.* 
                        FROM Users u
                        INNER JOIN LoggedInUsers l ON u.Id = l.UserId
                        ORDER BY l.LastLoginAt DESC";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    Id = reader["Id"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    PasswordHash = reader["PasswordHash"].ToString(),
                                    Salt = reader["Salt"].ToString(),
                                    CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString()),
                                    LastLoginAt = reader.IsDBNull(reader.GetOrdinal("LastLoginAt")) ? 
                                        (DateTime?)null : DateTime.Parse(reader["LastLoginAt"].ToString()),
                                    IsAdmin = reader.IsDBNull(reader.GetOrdinal("IsAdmin")) ? 
                                        false : Convert.ToBoolean(Convert.ToInt32(reader["IsAdmin"]))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"获取已登录用户时出错: {ex.Message}");
            }

            return users;
        }

        /// <summary>
        /// 检查用户是否已登录
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>是否已登录</returns>
        public bool IsUserLoggedIn(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            try
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    string sql = "SELECT COUNT(*) FROM LoggedInUsers WHERE UserId = @userId";

                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"检查用户是否已登录时出错: {ex.Message}");
                return false;
            }
        }
    }
} 