using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using llm_agent.Model;

namespace llm_agent.DAL
{
    /// <summary>
    /// 用户数据访问层，处理用户相关的数据库操作
    /// </summary>
    public class UserRepository
    {
        private static readonly string DbName = "llm_agent.db";
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        /// <summary>
        /// 初始化UserRepository
        /// </summary>
        public UserRepository()
        {
            // 确保数据库和表已初始化
            var databaseManager = new DatabaseManager();
        }

        /// <summary>
        /// 创建新用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码明文</param>
        /// <param name="isAdmin">是否为管理员</param>
        /// <returns>创建的用户对象</returns>
        public User CreateUser(string username, string password, bool isAdmin = false)
        {
            // 检查用户名是否已存在
            if (GetUserByUsername(username) != null)
            {
                throw new InvalidOperationException($"用户名 '{username}' 已存在");
            }

            // 生成盐值和密码哈希
            string salt = GenerateSalt();
            string passwordHash = HashPassword(password, salt);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = username,
                PasswordHash = passwordHash,
                Salt = salt,
                CreatedAt = DateTime.Now,
                IsAdmin = isAdmin
            };

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string insertSql = @"
                    INSERT INTO Users (Id, Username, PasswordHash, Salt, CreatedAt, LastLoginAt, IsAdmin)
                    VALUES (@id, @username, @passwordHash, @salt, @createdAt, @lastLoginAt, @isAdmin)";

                using (var command = new SQLiteCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@salt", user.Salt);
                    command.Parameters.AddWithValue("@createdAt", user.CreatedAt.ToString("o"));
                    command.Parameters.AddWithValue("@lastLoginAt", user.LastLoginAt.HasValue ? user.LastLoginAt.Value.ToString("o") : DBNull.Value);
                    command.Parameters.AddWithValue("@isAdmin", user.IsAdmin ? 1 : 0);

                    command.ExecuteNonQuery();
                }
            }

            return user;
        }

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户对象，如果不存在则返回null</returns>
        public User GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Users WHERE Id = @userId";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
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
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据用户名获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户对象，如果不存在则返回null</returns>
        public User GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Users WHERE Username = @username";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
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
                            };
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns>用户列表</returns>
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT * FROM Users ORDER BY Username";

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

            return users;
        }

        /// <summary>
        /// 更新用户登录时间
        /// </summary>
        /// <param name="userId">用户ID</param>
        public void UpdateLastLoginTime(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "UPDATE Users SET LastLoginAt = @lastLoginAt WHERE Id = @userId";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@lastLoginAt", DateTime.Now.ToString("o"));
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 更新用户密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="newPassword">新密码明文</param>
        public void UpdatePassword(string userId, string newPassword)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
                return;

            var user = GetUserById(userId);
            if (user == null)
                throw new InvalidOperationException($"用户ID '{userId}' 不存在");

            // 生成新的盐值和密码哈希
            string salt = GenerateSalt();
            string passwordHash = HashPassword(newPassword, salt);

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "UPDATE Users SET PasswordHash = @passwordHash, Salt = @salt WHERE Id = @userId";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);
                    command.Parameters.AddWithValue("@salt", salt);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户ID</param>
        public void DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Users WHERE Id = @userId";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 验证用户密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码明文</param>
        /// <returns>验证成功返回用户对象，失败返回null</returns>
        public User ValidateUser(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null)
                return null;

            string hashedPassword = HashPassword(password, user.Salt);
            if (hashedPassword == user.PasswordHash)
            {
                return user;
            }

            return null;
        }

        /// <summary>
        /// 生成随机盐值
        /// </summary>
        /// <returns>盐值字符串</returns>
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// 使用盐值对密码进行哈希
        /// </summary>
        /// <param name="password">密码明文</param>
        /// <param name="salt">盐值</param>
        /// <returns>哈希后的密码</returns>
        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            
            byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);
            
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}